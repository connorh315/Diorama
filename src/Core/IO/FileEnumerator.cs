using BrickVault.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.IO
{
    public class FileEnumerator
    {
        /// <summary>
        /// Iterates through all files in the specified directory and subdirectories, matching specified extensions.
        /// The RawFile is automatically disposed of after use, do not store beyond the enumeration.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<RawFile> EnumerateDirectory(string directoryPath, params string[] extensions)
        {
            foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
            {
                if (extensions.Length == 0 || extensions.Any(ext => filePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    using var file = new RawFile(filePath);
                    yield return file;
                }
            }
        }

        /// <summary>
        /// Iterates through all .DAT archive files in the specified directory and subdirectories, extracting files that match specified extensions.
        /// The RawFile is automatically disposed of after use, do not store beyond the enumeration.
        /// </summary>
        /// <param name="archivesPath"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        //public static IEnumerable<RawFile> EnumerateArchives(string archivesPath, params string[] extensions)
        //{
        //    foreach (var archivePath in Directory.EnumerateFiles(archivesPath, "*.DAT", SearchOption.AllDirectories))
        //    {
        //        DATFile datFile = DATFile.Open(archivePath);

        //        var ctx = datFile.GetExtractionContext();

        //        foreach (var archiveFile in datFile.Files)
        //        {
        //            if (extensions.Length == 0 || extensions.Any(ext => archiveFile.Path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
        //            {
        //                using var file = new RawFile(new MemoryStream());
        //                file.SetFileLocation($"dat://{archiveFile.Path}");

        //                datFile.ExtractFile(archiveFile, ctx, file.fileStream);

        //                yield return file;
        //            }
        //        }
        //    }
        //}
    }
}
