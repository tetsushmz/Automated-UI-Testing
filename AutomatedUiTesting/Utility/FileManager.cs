using System;
using System.IO;
using FlaUI.Core.Tools;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utility
{
    public class FileManager
    {
        private static ILog Logger => LogManager.GetLogger(typeof(FileManager));

        public static void RemoveFolderIfExists(string folderName, string folderPath)
        {
            var fullPath = $"{folderPath}\\{folderName}";
            if (!Directory.Exists(fullPath))
            {
                return;
            }

            var retryResult = Retry.WhileException(
                () =>
                {
                    Logger.Info($"Trying to remove '{fullPath}'.");
                    Directory.Delete(fullPath, true);
                },
                TimeSpan.FromSeconds(20));
            Assert.IsTrue(retryResult.Success, $"Failed to remove '{fullPath}'.");
            Logger.Info($"Successfully removed '{fullPath}'.");
        }
    }
}
