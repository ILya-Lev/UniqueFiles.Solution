using System;
using System.IO;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL
{
    public class BackupDirectoryManager : IBackupDirectoryManager, IDisposable
    {
        public static string DefaultBackUpSubFolder { get; } = "bkp";
        public string BackupRoot { get; private set; }

        public BackupDirectoryManager(string rootFolder, string backupFolder)
        {
            BackupRoot = GenerateBackUpPath(rootFolder, backupFolder);
        }

        public void Dispose()
        {
            DeleteBackupDirectory();
        }

        public void CreateBackupDirectory(string subFolder = null)
        {
            var actualPath = GetActualPath(subFolder);
            Directory.CreateDirectory(actualPath);
        }

        public void DeleteBackupDirectory(string subFolder = null)
        {
            var actualPath = GetActualPath(subFolder);

            if (Directory.GetFileSystemEntries(actualPath).Length == 0)
                Directory.Delete(actualPath);
        }

        private string GenerateBackUpPath(string rootFolder, string backUpFolder)
        {
            if (string.IsNullOrWhiteSpace(backUpFolder)
                || string.Equals(rootFolder, backUpFolder, StringComparison.OrdinalIgnoreCase))
            {
                return Path.Combine(rootFolder, DefaultBackUpSubFolder);
            }

            return backUpFolder;
        }

        private string GetActualPath(string subFolder)
        {
            return string.IsNullOrWhiteSpace(subFolder)
                ? BackupRoot
                : Path.Combine(BackupRoot, subFolder);
        }
    }
}
