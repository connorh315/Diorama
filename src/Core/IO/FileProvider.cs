using BrickVault.Types;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.IO
{
    public static class FileProvider
    {
        private static IFileProvider provider;
        public static FileProviderState State { get; private set; }

        public static void InitializeArchives(string location)
        {
            provider = new ArchivesFileProvider(location);
            State = FileProviderState.Archives;
        }

        public static void InitializeExtracted(string location)
        {
            provider = new ExtractedFileProvider(location);
            State = FileProviderState.Extracted;
        }

        private static string NormalisePath(string location) => location.ToLower().Replace('/', '\\').TrimStart('\\');

        public static RawFile GetFile(string location)
        {
            location = NormalisePath(location);

            return provider.GetFile(location);
        }

        public static RawFile GetFileFromArchive(string archiveName, string location)
        {
            return ((ArchivesFileProvider)provider).GetFileFromArchive(archiveName, location);
        }

        public static RawFile GetFile(FileLocation location)
        {
            if (location is ArchiveFileLocation archiveFileLocation)
            {
                return ((ArchivesFileProvider)provider).GetFileFromArchive(archiveFileLocation.ArchiveName, archiveFileLocation.ArchiveFilePath);
            }
            else
            {
                return GetFile(location.FullPath);
            }
        }

        public static IEnumerable<RawFile> EnumerateFiles(params string[] extensions)
        {
            return provider.EnumerateFiles(extensions);
        }

        public static IEnumerable<FileLocation> EnumerateLocations(params string[] extensions)
        {
            return provider.EnumerateLocations(extensions);
        }

        public static FileLocation ReplaceInLocation(FileLocation location, string toReplace, string replacement)
        {
            if (location is ArchiveFileLocation archiveFileLocation)
            {
                return new ArchiveFileLocation(archiveFileLocation.ArchivePath, archiveFileLocation.ArchiveFilePath.Replace(toReplace, replacement));
            }
            else
            {
                return new FilesystemFileLocation(location.FullPath.Replace(toReplace, replacement));
            }
        }

        public static FileLocation ReplaceLocationExtension(FileLocation location, string replacement)
        {
            if (location is ArchiveFileLocation archiveFileLocation)
            {
                return new ArchiveFileLocation(archiveFileLocation.ArchivePath, Path.ChangeExtension(archiveFileLocation.ArchiveFilePath, replacement));
            }
            else
            {
                return new FilesystemFileLocation(Path.ChangeExtension(location.FullPath, replacement));
            }
        }

        public enum FileProviderState
        {
            Extracted,
            Archives
        }
    }
}
