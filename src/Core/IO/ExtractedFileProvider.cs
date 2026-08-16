using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.IO
{
    internal class ExtractedFileProvider : IFileProvider
    {
        private string location;
        public ExtractedFileProvider(string path)
        {
            location = path;
        }

        public RawFile GetFile(string path)
        {
            string systemLocation = Path.Combine(location, path);

            if (!Path.Exists(systemLocation))
                return null;

            return new RawFile(systemLocation);
        }

        public IEnumerable<RawFile> EnumerateFiles(params string[] extensions)
        {
            foreach (string extension in extensions)
            {
                string pattern = extension.StartsWith('.')
                    ? $"*{extension}"
                    : $"*.{extension}";

                foreach (string path in Directory.EnumerateFiles(
                    location,
                    pattern,
                    SearchOption.AllDirectories))
                {
                    using var file = new RawFile(path);
                    yield return file;
                }
            }
        }

        public IEnumerable<FileLocation> EnumerateLocations(params string[] extensions)
        {
            foreach (string extension in extensions)
            {
                string pattern = extension.StartsWith('.')
                    ? $"*{extension}"
                    : $"*.{extension}";

                foreach (string path in Directory.EnumerateFiles(
                    location,
                    pattern,
                    SearchOption.AllDirectories))
                {
                    yield return new FilesystemFileLocation(path.ToLower());
                }
            }
        }
    }
}
