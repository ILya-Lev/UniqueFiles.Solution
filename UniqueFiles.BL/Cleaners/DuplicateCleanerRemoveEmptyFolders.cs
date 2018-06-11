using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UniqueFiles.BL.Interfaces;
using UniqueFiles.BL.Providers;
using UniqueFiles.BL.Registries;

namespace UniqueFiles.BL.Cleaners
{
    public class DuplicateCleanerRemoveEmptyFolders : DuplicateCleaner
    {
        private readonly IBackupDirectoryManager _backupDirectoryManager;
        private readonly Stack<string> _processedFolders = new Stack<string>();

        public DuplicateCleanerRemoveEmptyFolders(
            IUniqueFilesRegistry uniqueFilesRegistry,
            IBackedUpFilesRegistry backedUpFilesRegistry,
            IFileSystemEntityProvider fileNamesProvider,
            IFileSystemEntityProvider folderNamesProvider,
            IBackupDirectoryManager backupDirectoryManager)
            : base(uniqueFilesRegistry, backedUpFilesRegistry, fileNamesProvider, folderNamesProvider)
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
            WaitAllFilesAreCopied();
            while (_processedFolders.Count != 0)
            {
                _backupDirectoryManager?.DeleteBackupDirectory(_processedFolders.Pop());
            }
        }

        private void WaitAllFilesAreCopied()
        {
            while (BackedUpFilesRegistry.NumberOfFilesInCopying > 0)
            {
                Debug.WriteLine($"waiting for {BackedUpFilesRegistry.NumberOfFilesInCopying} files");
                Thread.Sleep(10);
            }
        }
    }
}