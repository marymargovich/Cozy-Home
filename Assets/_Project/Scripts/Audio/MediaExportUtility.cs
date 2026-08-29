using System;
using System.IO;
using UnityEngine;

namespace CozyHome.Audio
{
    public static class MediaExportUtility
    {
        public static string BuildExportPath(string mediaType, string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = string.Empty;
            }

            string normalizedType = string.IsNullOrWhiteSpace(mediaType) ? "Media" : mediaType.Trim();
            string fileName = $"CozyHome_{normalizedType}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
            string targetDirectory = GetExportDirectory();
            Directory.CreateDirectory(targetDirectory);
            return Path.Combine(targetDirectory, fileName);
        }

        public static string GetExportDirectory()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            string exportDirectory = Path.Combine(desktopPath, "CozyHome_Exports");
            return exportDirectory;
#elif UNITY_IOS || UNITY_ANDROID
            return Application.persistentDataPath;
#else
            return Application.persistentDataPath;
#endif
        }
    }
}
