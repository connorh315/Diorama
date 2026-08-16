using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.IO
{
    internal interface IFileProvider
    {
        public RawFile GetFile(string path);

        public IEnumerable<RawFile> EnumerateFiles(params string[] extension);
        public IEnumerable<FileLocation> EnumerateLocations(params string[] extension);
    }
}
