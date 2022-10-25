using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace AssetInventory
{
    public static class AssetStore
    {
        private const string URLPurchases = "https://packages-v2.unity.com/-/api/purchases";
        private const string URLTokenInfo = "https://api.unity.com/v1/oauth2/tokeninfo?access_token=";
        private const string URLUserInfo = "https://api.unity.com/v1/users";
        private const string URLAssetDetails = "https://packages-v2.unity.com/-/api/product";
        private const string URLAssetDownload = "https://packages-v2.unity.com/-/api/legacy-package-download-info";
        private const int PageSize = 100; // more is not supported by Asset Store

        private static ListRequest _listRequest;
        private static SearchRequest _searchRequest;
        private static List<PackageInfo> _allPackages;
        private static PackageCollection _projectPackages;
        private static List<string> _failedPackages;
        private static string _currentSearch;
        private static bool _offlineMode;

        public static bool CancellationRequested { get; set; }

        public static async Task<AssetPurchases> RetrievePurchases()
        {
            AssetInventory.CurrentMain = "Phase 1/3: Fetching purchases";
            AssetInventory.MainCount = 1;
            AssetInventory.MainProgress = 1;
            int progressId = MetaProgress.Start("Fetching purchases");

            string token = CloudProjectSettings.accessToken;
            AssetPurchases result = await AssetUtils.FetchAPIData<AssetPurchases>($"{URLPurchases}?offset=0&limit={PageSize}", token);

            // if more results than page size retrieve rest as well and merge
            // doing all requests in parallel is not possible with Unity's web client since they can only run on the main thread
            if (result != null && result.total > PageSize)
            {
                int pageCount = AssetUtils.GetPageCount(result.total, PageSize) - 1;
                AssetInventory.MainCount = pageCount + 1;
                for (int i = 1; i <= pageCount; i++)
                {
                    AssetInventory.MainProgress = i + 1;
                    MetaProgress.Report(progressId, i + 1, pageCount + 1, string.Empty);
                    if (CancellationRequested) break;

                    AssetPurchases pageResult = await AssetUtils.FetchAPIData<AssetPurchases>($"{URLPurchases}?offset={i * PageSize}&limit={PageSize}", token);
                    result.results.AddRange(pageResult.results);
                }
            }

            AssetInventory.CurrentMain = null;
            MetaProgress.Remove(progressId);

            return result;
        }

        public static async Task<AssetDetails> RetrieveAssetDetails(int id, string eTag = null)
        {
            string token = CloudProjectSettings.accessToken;
            string newEtag = eTag;
            AssetDetails result = await AssetUtils.FetchAPIData<AssetDetails>($"{URLAssetDetails}/{id}", token, eTag, newCacheTag => newEtag = newCacheTag);
            if (result != null) result.ETag = newEtag;

            return result;
        }

        public static async Task<DownloadInfo> RetrieveAssetDownloadInfo(int id)
        {
            string token = CloudProjectSettings.accessToken;
            DownloadInfoResult result = await AssetUtils.FetchAPIData<DownloadInfoResult>($"{URLAssetDownload}/{id}", token);

            // special handling of "." also in AssetStoreDownloadInfo 
            if (result?.result?.download != null)
            {
                result.result.download.filename_safe_category_name = result.result.download.filename_safe_category_name.Replace(".", string.Empty);
                result.result.download.filename_safe_package_name = result.result.download.filename_safe_package_name.Replace(".", string.Empty);
                result.result.download.filename_safe_publisher_name = result.result.download.filename_safe_publisher_name.Replace(".", string.Empty);
            }

            return result?.result?.download;
        }

        public static void GatherProjectMetadata()
        {
            _listRequest = Client.List();
            EditorApplication.update += ListProgress;
        }

        private static void ListProgress()
        {
            if (!_listRequest.IsCompleted) return;
            EditorApplication.update -= ListProgress;

            if (_listRequest.Status == StatusCode.Success)
            {
                _projectPackages = _listRequest.Result;
            }
        }

        public static void GatherAllMetadata()
        {
            _searchRequest = Client.SearchAll(_offlineMode);
            EditorApplication.update += SearchProgress;
        }

        private static void SearchProgress()
        {
            if (!_searchRequest.IsCompleted) return;
            EditorApplication.update -= SearchProgress;

            if (_searchRequest.Status == StatusCode.Success)
            {
                _allPackages = _searchRequest.Result?.ToList();
            }
            else if (_searchRequest.Status == StatusCode.Failure)
            {
                // fallback to offline mode
                if (!_offlineMode)
                {
                    _offlineMode = true;
                    GatherAllMetadata();
                }
            }
        }

        private static void IncrementalSearchProgress()
        {
            if (!_searchRequest.IsCompleted) return;
            EditorApplication.update -= IncrementalSearchProgress;

            if (_searchRequest.Status == StatusCode.Success && _searchRequest.Result != null)
            {
                if (_allPackages == null) _allPackages = new List<PackageInfo>();
                _allPackages.AddRange(_searchRequest.Result);
            }
            else
            {
                if (_failedPackages == null) _failedPackages = new List<string>();
                _failedPackages.Add(_currentSearch);
            }
            _currentSearch = null;
        }

        public static bool IsMetadataAvailable(bool considerSearch = true)
        {
            FillBufferOnDemand();

            if (considerSearch && _searchRequest != null && !_searchRequest.IsCompleted) return false;
            return _projectPackages != null && (_allPackages != null || (_searchRequest != null && _searchRequest.IsCompleted));
        }

        public static PackageInfo GetPackageInfo(string name)
        {
            FillBufferOnDemand();

            PackageInfo result = _projectPackages?.FirstOrDefault(p => p.name == name);
            if (result == null) result = _allPackages?.FirstOrDefault(p => p.name == name);
            if (result == null && (_searchRequest == null || _searchRequest.IsCompleted) && (_failedPackages == null || !_failedPackages.Contains(name)))
            {
                _currentSearch = name;
                _searchRequest = Client.Search(name);
                EditorApplication.update += IncrementalSearchProgress;
            }

            return result;
        }

        public static PackageCollection GetProjectPackages()
        {
            FillBufferOnDemand();

            return _projectPackages;
        }

        public static void FillBufferOnDemand()
        {
            if (_projectPackages == null && _listRequest == null) GatherProjectMetadata();
            if (_allPackages == null && _searchRequest == null) GatherAllMetadata();
        }

        public static bool IsInstalled(AssetInfo info)
        {
            if (info.AssetSource != Asset.Source.Package) return false;

            FillBufferOnDemand();

            return IsInstalled(info.SafeName, info.GetVersionToUse());
        }

        public static bool IsInstalled(string name, string version)
        {
            PackageInfo result = _projectPackages?.FirstOrDefault(p => p.name == name);
            return result != null && result.version == version;
        }
    }
}