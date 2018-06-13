using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL.Registries
{
    public class BackedUpFilesRegistry : IBackedUpFilesRegistry
    {
        private readonly IBackupDirectoryManager _backupDirectoryManager;

        private readonly Dictionary<string, int> _backupFileCounters = new Dictionary<string, int>();

        public static volatile int NumberOfFilesInCopying = 0;

        public BackedUpFilesRegistry(IBackupDirectoryManager backupDirectoryManager)
        {
            _backupDirectoryManager = backupDirectoryManager;
        }

        public async Task AddAsync(FileInfo fileInfo)
        {
            if (_backupFileCounters.ContainsKey(fileInfo.Name))
            {
                await AddFileSeenPreviously(fileInfo);
            }
            else
            {
                await AddFileFirstTimeSeen(fileInfo);
            }
        }

        private async Task AddFileSeenPreviously(FileInfo fileInfo)
        {
            var fileSeenTimes = _backupFileCounters[fileInfo.Name] + 1;
            if (IsBigFile(fileInfo))
                fileSeenTimes = await MoveToSubfolderTask(fileInfo, fileSeenTimes);
            else
                fileSeenTimes = MoveToSubFolder(fileInfo, fileSeenTimes);

            _backupFileCounters[fileInfo.Name] = fileSeenTimes;
        }

        private Task<int> MoveToSubfolderTask(FileInfo fileInfo, int fileSeenTimes)
        {
            return Task.Run(() =>
            {
                try
                {
                    NumberOfFilesInCopying++;
                    var seenTimes = MoveToSubFolder(fileInfo, fileSeenTimes);
                    return seenTimes;
                }
                finally
                {
                    NumberOfFilesInCopying--;
                }
            });
        }

        private async Task AddFileFirstTimeSeen(FileInfo fileInfo)
        {
            var fileSeenTimes = 0;
            if (IsBigFile(fileInfo))
                fileSeenTimes = await MoveToRootTask(fileInfo);
            else
                fileSeenTimes = MoveToRoot(fileInfo);
            _backupFileCounters.Add(fileInfo.Name, fileSeenTimes);
        }

        private Task<int> MoveToRootTask(FileInfo fileInfo)
        {
            return Task.Run(() =>
            {
                try
                {
                    NumberOfFilesInCopying++;
                    var seenTimes = MoveToRoot(fileInfo);
                    return seenTimes;
                }
                finally
                {
                    NumberOfFilesInCopying--;
                }
            });
        }

        private static bool IsBigFile(FileInfo fileInfo)
        {
            return fileInfo.Length > 1_000_000;
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