using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AssetInventory
{
    public sealed class ColorImporter : AssetImporter
    {
        public IEnumerator Index()
        {
            ResetState(false);
            int progressId = MetaProgress.Start("Extracting color information");

            string previewFolder = AssetInventory.GetPreviewFolder();

            List<AssetFile> files = DBAdapter.DB.Table<AssetFile>()
                .Where(a => a.PreviewFile != null && a.Hue < 0)
                .ToList();

            SubCount = files.Count;
            for (int i = 0; i < files.Count; i++)
            {
                if (CancellationRequested) break;
                yield return Cooldown.DoCo();

                AssetFile file = files[i];
                MetaProgress.Report(progressId, i + 1, files.Count, file.FileName);

                // skip audio files per default
                if (!AssetInventory.Config.extractAudioColors && AssetInventory.IsFileType(file.Path, "Audio")) continue;

                string previewFile = Path.Combine(previewFolder, file.AssetId.ToString(), file.PreviewFile);
                if (!File.Exists(previewFile)) continue;

                CurrentSub = $"Extracting colors from {file.FileName}";
                SubProgress = i + 1;

                yield return AssetUtils.LoadTexture(previewFile, result =>
                {
                    if (result == null) return;

                    file.Hue = ColorUtils.GetHue(result);
                    Persist(file);
                });
            }
            MetaProgress.Remove(progressId);
            ResetState(true);
        }
    }
}