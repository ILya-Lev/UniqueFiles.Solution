using UniqueFiles.BL.Cleaners;
using UniqueFiles.BL.Providers;
using UniqueFiles.BL.Registries;

namespace UniqueFiles.BL.Transactions
{
    public class DuplicateCleaningTransaction : ITransaction
    {
        private readonly string _rootFolder;
        private readonly string _backupFolder;

        public DuplicateCleaningTransaction(string rootFolder, string backupFolder)
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

                var cleaner = new DuplicateCleanerRemoveEmptyFolders(
                    new UniqueFileRegistry(),
                    backedUpFileRegistry,
                    new FileProvider(),
                    folderNamesProvider,
                    backupManager);

                cleaner.Clean(_rootFolder);
            }
        }
    }
}
