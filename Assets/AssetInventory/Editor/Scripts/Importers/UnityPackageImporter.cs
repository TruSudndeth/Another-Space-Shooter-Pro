using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AssetInventory
{
    public sealed class UnityPackageImporter : AssertImporter
    {
        private const int BreakInterval = 10;

        public async Task IndexRough(string path, bool fromAssetStore, bool force = false)
        {
            ResetState();

            int progressId = MetaProgress.Start("Updating asset inventory index");
            string[] packages = Directory.GetFiles(path, "*.unitypackage", SearchOption.AllDirectories);
            MainCount = packages.Length;
            for (int i = 0; i < packages.Length; i++)
            {
                if (CancellationRequested) break;

                string package = packages[i];
                MetaProgress.Report(progressId, i + 1, packages.Length, package);
                if (i % 50 == 0) await Task.Yield(); // let editor breath

                // create asset and add additional information from file system
                Asset asset = new Asset();
                asset.SafeName = Path.GetFileNameWithoutExtension(package);
                if (fromAssetStore)
                {
                    asset.AssetSource = Asset.Source.AssetStorePackage;
                    DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(package));
                    asset.SafeCategory = dirInfo.Name;
                    asset.SafePublisher = dirInfo.Parent.Name;
                    if (string.IsNullOrEmpty(asset.DisplayCategory))
                    {
                        asset.DisplayCategory = System.Text.RegularExpressions.Regex.Replace(asset.SafeCategory, "([a-z])([A-Z])", "$1/$2", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
                    }
                }
                else
                {
                    asset.AssetSource = Asset.Source.CustomPackage;
                }

                // skip unchanged 
                Asset existing = Fetch(asset);
                long size; // determine late for performance, especially with many exclusions
                if (existing != null)
                {
                    if (existing.Exclude) continue;

                    size = new FileInfo(package).Length;
                    if (!force && existing.CurrentState == Asset.State.Done && existing.PackageSize == size && existing.Location == package) continue;

                    if (string.IsNullOrEmpty(existing.SafeCategory)) existing.SafeCategory = asset.SafeCategory;
                    if (string.IsNullOrEmpty(existing.DisplayCategory)) existing.DisplayCategory = asset.DisplayCategory;
                    if (string.IsNullOrEmpty(existing.SafePublisher)) existing.SafePublisher = asset.SafePublisher;
                    if (string.IsNullOrEmpty(existing.SafeName)) existing.SafeName = asset.SafeName;

                    asset = existing;
                }
                else
                {
                    size = new FileInfo(package).Length;
                }

                // update progress only if really doing work to save refresh time in UI
                CurrentMain = package;
                MainProgress = i + 1;

                // try to read contained upload details
                AssetHeader header = ReadHeader(package);
                if (header != null)
                {
                    asset.Version = header.version;
                    if (!string.IsNullOrWhiteSpace(header.description)) asset.Description = header.description;
                    if (!string.IsNullOrWhiteSpace(header.title)) asset.DisplayName = header.title;
                    if (header.publisher != null) asset.DisplayPublisher = header.publisher.label;
                    if (header.category != null) asset.DisplayCategory = header.category.label;

                    if (int.TryParse(header.id, out int id))
                    {
                        asset.ForeignId = id;
                    }
                }

                Asset.State previousState = asset.CurrentState;
                if (!force) asset.CurrentState = Asset.State.InProcess;
                asset.Location = package;
                asset.PackageSize = size;
                if (previousState != asset.CurrentState) asset.ETag = null; // force rechecking of download metadata
                Persist(asset);
            }
            MetaProgress.Remove(progressId);
            ResetState();
        }

        public async Task IndexDetails(int assetId = 0)
        {
            ResetState();
            int progressId = MetaProgress.Start("Indexing package contents");
            List<Asset> assets;
            if (assetId == 0)
            {
                assets = DBAdapter.DB.Table<Asset>().Where(asset => !asset.Exclude && asset.CurrentState == Asset.State.InProcess && asset.AssetSource != Asset.Source.Package).ToList();
            }
            else
            {
                assets = DBAdapter.DB.Table<Asset>().Where(asset => asset.Id == assetId && asset.AssetSource != Asset.Source.Package).ToList();
            }
            MainCount = assets.Count;
            for (int i = 0; i < assets.Count; i++)
            {
                if (CancellationRequested) break;

                MetaProgress.Report(progressId, i + 1, assets.Count, assets[i].Location);
                CurrentMain = assets[i].Location + " (" + EditorUtility.FormatBytes(assets[i].PackageSize) + ")";
                MainProgress = i + 1;

                await IndexPackage(assets[i], progressId);
                await Task.Yield(); // let editor breath

                assets[i].CurrentState = Asset.State.Done;
                Persist(assets[i]);
            }
            MetaProgress.Remove(progressId);
            ResetState();
        }

        private async Task IndexPackage(Asset asset, int progressId)
        {
            int subProgressId = MetaProgress.Start("Indexing package", null, progressId);
            string previewPath = AssetInventory.GetPreviewFolder();
            string tempPath = await AssetInventory.ExtractAsset(asset);
            if (string.IsNullOrEmpty(tempPath))
            {
                Debug.LogError($"{asset} could not be indexed due to issues extracting it: {asset.Location}");
                return;
            }
            string assetPreviewFile = Path.Combine(tempPath, ".icon.png");
            if (File.Exists(assetPreviewFile))
            {
                string targetFile = Path.Combine(previewPath, "a-" + asset.Id + Path.GetExtension(assetPreviewFile));
                File.Copy(assetPreviewFile, targetFile, true);
                asset.PreviewImage = Path.GetFileName(targetFile);
            }

            string[] assets = Directory.GetFiles(tempPath, "pathname", SearchOption.AllDirectories);
            SubCount = assets.Length;
            for (int i = 0; i < assets.Length; i++)
            {
                string packagePath = assets[i];
                string dir = Path.GetDirectoryName(packagePath);
                string fileName = File.ReadLines(packagePath).FirstOrDefault();
                string metaFile = Path.Combine(dir, "asset.meta");
                string assetFile = Path.Combine(dir, "asset");
                string previewFile = Path.Combine(dir, "preview.png");

                CurrentSub = fileName;
                SubProgress = i + 1;
                MetaProgress.Report(subProgressId, i + 1, assets.Length, fileName);

                // skip folders
                if (!File.Exists(assetFile)) continue;

                if (i % BreakInterval == 0) await Task.Yield(); // let editor breath

                string guid = null;
                if (File.Exists(metaFile))
                {
                    guid = AssetUtils.ExtractGuidFromFile(metaFile);
                    if (guid == null) continue;
                }

                // remaining info from file data (creation date is not original date anymore, ignore)
                FileInfo assetInfo = new FileInfo(assetFile);
                long size = assetInfo.Length;

                AssetFile af = new AssetFile();
                af.Guid = guid;
                af.AssetId = asset.Id;
                af.Path = fileName;
                af.SourcePath = assetFile.Substring(tempPath.Length + 1);
                af.FileName = Path.GetFileName(af.Path);
                af.Size = size;
                af.Type = Path.GetExtension(fileName).Replace(".", string.Empty).ToLowerInvariant();
                if (AssetInventory.Config.gatherExtendedMetadata)
                {
                    await ProcessMediaAttributes(assetFile, af, asset); // must be run on main thread
                }
                Persist(af);

                // update preview 
                if (AssetInventory.Config.extractPreviews && File.Exists(previewFile))
                {
                    string targetFile = Path.Combine(previewPath, "af-" + af.Id + Path.GetExtension(previewFile));
                    File.Copy(previewFile, targetFile, true);
                    af.PreviewFile = Path.GetFileName(targetFile);
                    Persist(af);
                }
                if (CancellationRequested) break;
                await Cooldown.Do();
            }

            // remove files again, no need to wait
            Task _ = Task.Run(() => Directory.Delete(tempPath, true));

            CurrentSub = null;
            SubCount = 0;
            SubProgress = 0;
            MetaProgress.Remove(subProgressId);
        }

        private AssetHeader ReadHeader(string path)
        {
            AssetHeader result = null;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    byte[] header = new byte[17];
                    stream.Read(header, 0, 17);

                    // check if really a JSON object
                    if (header[16] == '{')
                    {
                        stream.Seek(14, SeekOrigin.Begin);
                        byte[] lengthData = new byte[2];
                        stream.Read(lengthData, 0, 2);
                        int length = BitConverter.ToInt16(lengthData, 0);

                        stream.Seek(16, SeekOrigin.Begin);
                        byte[] data = new byte[length];
                        stream.Read(data, 0, length);
                        string headerData = Encoding.ASCII.GetString(data);
                        result = JsonConvert.DeserializeObject<AssetHeader>(headerData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not parse package {path}: {e.Message}");
                }
            }

            return result;
        }
    }
}