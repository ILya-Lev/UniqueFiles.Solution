namespace UniqueFiles.BL
{
    public class DuplicateCleaningTransaction
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
            var backupManager = new BackupDirectoryManager(_rootFolder, _backupFolder);
            backupManager.CreateBackupDirectory();

            var backedUpFileRegistry = new BackedUpFileRegistry(backupManager);
            var folderNamesProvider = new DirectoryProvider(backupManager.BackupRoot);

            var cleaner = new DuplicateCleaner(_rootFolder, new UniqueFileRegistry(), backedUpFileRegistry, new FileProvider(), folderNamesProvider);

            cleaner.Clean();
        }
    }
}
