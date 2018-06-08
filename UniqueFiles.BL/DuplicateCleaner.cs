using System.Collections.Generic;

namespace UniqueFiles.BL
{
    public class DuplicateCleaner
    {
        private readonly IUniqueFileRegistry _uniqueFileRegistry;
        private readonly IBackedUpFileRegistry _backedUpFileRegistry;
        private readonly IFileSystemEntityProvider _fileNamesProvider;
        private readonly IFileSystemEntityProvider _folderNamesProvider;

        public DuplicateCleaner(IUniqueFileRegistry uniqueFileRegistry, IBackedUpFileRegistry backedUpFileRegistry, IFileSystemEntityProvider fileNamesProvider, IFileSystemEntityProvider folderNamesProvider)
        {
            _uniqueFileRegistry = uniqueFileRegistry;
            _backedUpFileRegistry = backedUpFileRegistry;
            _fileNamesProvider = fileNamesProvider;
            _folderNamesProvider = folderNamesProvider;
        }

        public void Clean(string rootFolder)
        {
            var folderQueue = new Queue<string>();
            folderQueue.Enqueue(rootFolder);
            while (folderQueue.Count != 0)
            {
                var currentFolder = folderQueue.Dequeue();
                ProcessOneFolder(currentFolder, folderQueue);
            }
        }

        private void ProcessOneFolder(string currentFolder, Queue<string> folderQueue)
        {
            foreach (var file in _fileNamesProvider.GetNames(currentFolder))
            {
                ProcessOneFile(file);
            }

            foreach (var directory in _folderNamesProvider.GetNames(currentFolder))
            {
                folderQueue.Enqueue(directory);
            }
        }

        private void ProcessOneFile(string fileName)
        {
            if (_uniqueFileRegistry.Contains(fileName))
                _backedUpFileRegistry.Add(fileName);
            else
                _uniqueFileRegistry.Add(fileName);
        }
    }
}
