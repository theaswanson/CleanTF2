﻿using System.Text.RegularExpressions;

namespace CleanTF2.Core.Utilities
{
    public class FileWrapper : IFile
    {
        public void Delete(string path) => File.Delete(path);
        public async Task<string[]> ReadAllLinesAsync(string path) => await File.ReadAllLinesAsync(path);
        public bool Exists(string path) => File.Exists(path);
    }
}
