using UniqueFiles.BL.Cleaners;
using UniqueFiles.BL.Providers;
using UniqueFiles.BL.Registries;

namespace UniqueFiles.BL.Transactions
{
    public class DuplicateCleaningPreserveFoldersTransaction : ITransaction
    {
        private readonly string _rootFolder;
        private readonly string _backupFolder;

        public DuplicateCleaningPreserveFoldersTransaction(string rootFolder, string backupFolder)
        {
            _rootFolder = rootFolder;
            _backupFolder = backupFolder;
        }

        public void Execute()
        {
            using (var backupManager = new BackupDirectoryManager(_rootFolder, _backupFolder))
            {
                backupManager.CreateBackupDirectory();
                var backedUpFileRegistry = new BackedUpFilesRegistry(backupManager);
                var folderNamesProvider = new DirectoryProvider(backupManager.BackupRoot);

                var cleaner = new DuplicateCleaner(
                    new UniqueFilesRegistry(),
                    backedUpFileRegistry,
                    new FileProvider(),
                    folderNamesProvider);

                cleaner.Clean(_rootFolder);
            }
        }
    }
}