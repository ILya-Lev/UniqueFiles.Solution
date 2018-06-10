using System.Collections.Generic;
using System.IO;

namespace UniqueFiles.BL
{
    public class DuplicateCleaner
    {
        private readonly string _rootFolder;
        private readonly IUniqueFileRegistry _uniqueFileRegistry;
        private readonly IBackedUpFileRegistry _backedUpFileRegistry;
        private readonly IFileSystemEntityProvider _fileNamesProvider;
        private readonly IFileSystemEntityProvider _folderNamesProvider;

        public DuplicateCleaner(string rootFolder, IUniqueFileRegistry uniqueFileRegistry,
            IBackedUpFileRegistry backedUpFileRegistry, IFileSystemEntityProvider fileNamesProvider,
            IFileSystemEntityProvider folderNamesProvider)
        {
            _rootFolder = rootFolder;
            _uniqueFileRegistry = uniqueFileRegistry;
            _backedUpFileRegistry = backedUpFileRegistry;
            _fileNamesProvider = fileNamesProvider;
            _folderNamesProvider = folderNamesProvider;
        }

        public void Clean()
        {
            var folderQueue = new Queue<string>();
            folderQueue.Enqueue(_rootFolder);
            while (folderQueue.Count != 0)
            {
                var currentFolder = folderQueue.Dequeue();
                ProcessOneFolder(currentFolder, folderQueue);
            }
        }

        private void ProcessOneFolder(string currentFolder, Queue<string> folderQueue)
        {
            foreach (var filePath in _fileNamesProvider.GetDescendantPaths(currentFolder))
            {
                ProcessOneFile(filePath);
            }

            foreach (var directoryPath in _folderNamesProvider.GetDescendantPaths(currentFolder))
            {
                folderQueue.Enqueue(directoryPath);
            }
        }

        private void ProcessOneFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            if (_uniqueFileRegistry.Contains(fileInfo))
                _backedUpFileRegistry.Add(fileInfo);
            else
                _uniqueFileRegistry.Add(fileInfo);
        }
    }
}
