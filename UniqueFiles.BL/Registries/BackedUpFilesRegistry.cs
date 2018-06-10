using System.Collections.Generic;
using System.IO;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL.Registries
{
    public class BackedUpFilesRegistry : IBackedUpFilesRegistry
    {
        private readonly IBackupDirectoryManager _backupDirectoryManager;

        private readonly Dictionary<string, int> _backupFileCounters = new Dictionary<string, int>();

        public BackedUpFilesRegistry(IBackupDirectoryManager backupDirectoryManager)
        {
            _backupDirectoryManager = backupDirectoryManager;
        }

        public void Add(FileInfo fileInfo)
        {
            if (_backupFileCounters.ContainsKey(fileInfo.Name))
            {
                var fileSeenTimes = _backupFileCounters[fileInfo.Name] + 1;
                fileSeenTimes = MoveToSubFolder(fileInfo, fileSeenTimes);
                _backupFileCounters[fileInfo.Name] = fileSeenTimes;
            }
            else
            {
                var fileSeenTimes = MoveToRoot(fileInfo);
                _backupFileCounters.Add(fileInfo.Name, fileSeenTimes);
            }
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