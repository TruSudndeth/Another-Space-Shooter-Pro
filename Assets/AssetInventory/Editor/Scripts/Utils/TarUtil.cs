using System;
using System.IO;
using System.Text;
using Unity.SharpZipLib.GZip;
using Unity.SharpZipLib.Tar;
using UnityEngine;

namespace AssetInventory
{
    public static class TarUtil
    {
        public static void ExtractGz(string fileName, string destinationFolder)
        {
            Stream inStream = File.OpenRead(fileName);
            GZipInputStream gzipStream = new GZipInputStream(inStream);

            try
            {
                TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream, Encoding.Default);
                tarArchive.ExtractContents(destinationFolder);
                tarArchive.Close();
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not extract archive '{fileName}'. The process was either interrupted or the file is corrupted: {e.Message}");
            }

            gzipStream.Close();
            inStream.Close();
        }
    }
}