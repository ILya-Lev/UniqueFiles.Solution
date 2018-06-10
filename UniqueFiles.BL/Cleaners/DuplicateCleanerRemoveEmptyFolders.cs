using System.Collections.Generic;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL.Cleaners
{
    public class DuplicateCleanerRemoveEmptyFolders : DuplicateCleaner
    {
        private readonly IBackupDirectoryManager _backupDirectoryManager;
        private readonly Stack<string> _processedFolders = new Stack<string>();

        public DuplicateCleanerRemoveEmptyFolders(
            IUniqueFileRegistry uniqueFileRegistry,
            IBackedUpFilesRegistry backedUpFilesRegistry,
            IFileSystemEntityProvider fileNamesProvider,
            IFileSystemEntityProvider folderNamesProvider,
            IBackupDirectoryManager backupDirectoryManager)
            : base(uniqueFileRegistry, backedUpFilesRegistry, fileNamesProvider, folderNamesProvider)
        {
            _backupDirectoryManager = backupDirectoryManager;
        }

        public override void Clean(string rootFolder)
        {
            base.Clean(rootFolder);

            CleanupEmptyFolders();
        }

        protected override void ProcessOneFolder(string currentFolder, Queue<string> folders)
        {
            base.ProcessOneFolder(currentFolder, folders);
            _processedFolders.Push(currentFolder);
        }

        private void CleanupEmptyFolders()
        {
            while (_processedFolders.Count != 0)
            {
                _backupDirectoryManager?.DeleteBackupDirectory(_processedFolders.Pop());
            }
        }
    }
}