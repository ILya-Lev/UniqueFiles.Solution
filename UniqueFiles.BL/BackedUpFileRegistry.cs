using System;
using System.Collections.Generic;
using System.IO;

namespace UniqueFiles.BL
{
    public class BackedUpFileRegistry : IBackedUpFileRegistry
    {
        public static string DefaultBackUpSubFolder { get; } = "bkp";

        private readonly Dictionary<string, int> _backedUpFiles = new Dictionary<string, int>();
        private readonly string _backUpRoot;

        public BackedUpFileRegistry(string rootFolder, string backUpFolder)
        {
            _backUpRoot = GenerateBackUpPath(rootFolder, backUpFolder);
            CreateBackupDirectory();
        }

        ~BackedUpFileRegistry()
        {
            if (Directory.GetFiles(_backUpRoot).Length == 0)
                Directory.Delete(_backUpRoot);
        }

        public void Add(FileInfo fileInfo)
        {
            if (_backedUpFiles.ContainsKey(fileInfo.Name))
            {
                var numberTimesFileSeen = _backedUpFiles[fileInfo.Name] + 1;
                var subFolder = $"{numberTimesFileSeen:D5}";
                CreateBackupDirectory(subFolder);
                fileInfo.MoveTo(Path.Combine(_backUpRoot, subFolder));
                _backedUpFiles.Add(fileInfo.Name, numberTimesFileSeen);
            }
            else
            {
                fileInfo.MoveTo(_backUpRoot);
                _backedUpFiles.Add(fileInfo.Name, 0);
            }
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

        private void CreateBackupDirectory(string subFolderName = null)
        {
            if (string.IsNullOrWhiteSpace(subFolderName))
                Directory.CreateDirectory(_backUpRoot);
            else
                Directory.CreateDirectory(Path.Combine(_backUpRoot, subFolderName));
        }
    }
}