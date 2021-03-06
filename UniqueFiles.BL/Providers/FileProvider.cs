﻿using System.Collections.Generic;
using System.IO;

namespace UniqueFiles.BL.Providers
{
    public class FileProvider : IFileSystemEntityProvider
    {
        public IEnumerable<string> GetDescendantPaths(string folder)
        {
            return Directory.GetFiles(folder);
        }
    }
}