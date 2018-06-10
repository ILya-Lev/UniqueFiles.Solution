using System.Collections.Generic;
using System.IO;
using UniqueFiles.BL.Interfaces;

namespace UniqueFiles.BL.Cleaners
{
    public class DuplicateCleaner
    {
        private readonly IUniqueFileRegistry _uniqueFileRegistry;
        private readonly IBackedUpFilesRegistry _backedUpFilesRegistry;
        private readonly IFileSystemEntityProvider _fileNamesProvider;
        private readonly IFileSystemEntityProvider _folderNamesProvider;

        public DuplicateCleaner(IUniqueFileRegistry uniqueFileRegistry,
            IBackedUpFilesRegistry backedUpFilesRegistry,
            IFileSystemEntityProvider fileNamesProvider,
            IFileSystemEntityProvider folderNamesProvider)
        {
            _uniqueFileRegistry = uniqueFileRegistry;
            _backedUpFilesRegistry = backedUpFilesRegistry;
            _fileNamesProvider = fileNamesProvider;
            _folderNamesProvider = folderNamesProvider;
        }

        public virtual void Clean(string rootFolder)
        {
            var folders = new Queue<string>();
            folders.Enqueue(rootFolder);
            while (folders.Count != 0)
            {
                var currentFolder = folders.Dequeue();
                ProcessOneFolder(currentFolder, folders);
            }
        }

        protected virtual void ProcessOneFolder(string currentFolder, Queue<string> folders)
        {
            foreach (var filePath in _fileNamesProvider.GetDescendantPaths(currentFolder))
            {
                ProcessOneFile(filePath);
            }

            foreach (var directoryPath in _folderNamesProvider.GetDescendantPaths(currentFolder))
            {
                folders.Enqueue(directoryPath);
            }
        }

        private void ProcessOneFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (_uniqueFileRegistry.Contains(fileInfo))
                _backedUpFilesRegistry.Add(fileInfo);
            else
                _uniqueFileRegistry.Add(fileInfo);
        }
    }
}
