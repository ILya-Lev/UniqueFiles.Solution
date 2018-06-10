using System.Collections.Generic;
using System.IO;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL.Registries
{
    public class BackedUpFileRegistry : IBackedUpFileRegistry
    {
        private readonly IBackupDirectoryManager _backupDirectoryManager;

        private readonly Dictionary<string, int> _backedUpFiles = new Dictionary<string, int>();

        public BackedUpFileRegistry(IBackupDirectoryManager backupDirectoryManager)
        {
            _backupDirectoryManager = backupDirectoryManager;
        }

        ~BackedUpFileRegistry()
        {
            _backupDirectoryManager.DeleteBackupDirectory();
        }

        public void Add(FileInfo fileInfo)
        {
            var initialFolder = fileInfo.Directory.FullName;
            if (_backedUpFiles.ContainsKey(fileInfo.Name))
            {
                var fileSeenTimes = _backedUpFiles[fileInfo.Name] + 1;
                fileSeenTimes = MoveToSubFolder(fileInfo, fileSeenTimes);
                _backedUpFiles[fileInfo.Name] = fileSeenTimes;
            }
            else
            {
                var fileSeenTimes = MoveToRoot(fileInfo);
                _backedUpFiles.Add(fileInfo.Name, fileSeenTimes);
            }
            _backupDirectoryManager.DeleteBackupDirectory(initialFolder);
        }

        private int MoveToRoot(FileInfo fileInfo)
        {
            try
            {
                fileInfo.MoveTo(Path.Combine(_backupDirectoryManager.BackupRoot, fileInfo.Name));
                return 0;
            }
            catch (IOException exc) when (exc.Message.Contains("Cannot create a file when that file already exists"))
            {
                return MoveToSubFolder(fileInfo, 1);
            }
        }

        private int MoveToSubFolder(FileInfo fileInfo, int fileSeenTimes)
        {
            try
            {
                var subFolder = $"{fileSeenTimes:D5}";
                _backupDirectoryManager.CreateBackupDirectory(subFolder);

                fileInfo.MoveTo(Path.Combine(_backupDirectoryManager.BackupRoot, subFolder, fileInfo.Name));
                return fileSeenTimes;
            }
            catch (IOException exc) when (exc.Message.Contains("Cannot create a file when that file already exists"))
            {
                return MoveToSubFolder(fileInfo, fileSeenTimes + 1);
            }
        }
    }
}