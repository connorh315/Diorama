using Avalonia.Controls.Shapes;
using BrickVault;
using BrickVault.Types;

namespace Diorama.Core.IO
{
    internal class ArchivesFileProvider : IFileProvider
    {
        private string location;
        private List<DATFile> Archives = new();
        private Dictionary<string, DATFile> ArchivesByName = new();

        public ArchivesFileProvider(string path)
        {
            location = path;

            foreach (var archivePath in Directory.EnumerateFiles(location, "*.DAT*", SearchOption.AllDirectories))
            {
                DATFile datFile = DATFile.Open(archivePath);
                if (datFile != null)
                {
                    Archives.Add(datFile);
                    if (!ArchivesByName.TryAdd(datFile.FileName.ToLower(), datFile))
                    {
                        throw new Exception($"Location contains two DAT archives with the same name: {datFile.FileName}");
                    }
                }
            }
        }

        public RawFile GetFile(string path)
        {
            foreach (var archive in Archives)
            {
                var file = archive.FileTree.GetFile(path);
                if (file != null)
                {
                    RawFile extract = new RawFile(new MemoryStream());

                    using (var ctx = archive.GetExtractionContext())
                    {
                        archive.ExtractFile(file, ctx, extract.fileStream);
                    }

                    extract.SetFileLocation(new ArchiveFileLocation(archive.FileLocation, file.Path));

                    return extract;
                }
            }

            return null;
        }

        private bool HasExtension(ArchiveFile archiveFile, params string[] extensions)
        {
            return (extensions.Length == 0 || extensions.Any(ext => archiveFile.Path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));
        }

        public IEnumerable<RawFile> EnumerateFiles(params string[] extensions)
        {
            foreach (var archive in Archives)
            {
                using (var ctx = archive.GetExtractionContext())
                {
                    foreach (var archiveFile in archive.Files)
                    {
                        if (HasExtension(archiveFile, extensions))
                        {
                            using var file = new RawFile(new MemoryStream());
                            file.SetFileLocation(new ArchiveFileLocation(archive.FileLocation, archiveFile.Path));

                            archive.ExtractRapid(archiveFile, file.fileStream, ctx.Archive);
                            file.Seek(0, SeekOrigin.Begin);

                            yield return file;
                        }
                    }
                }
            }
        }

        public IEnumerable<FileLocation> EnumerateLocations(params string[] extensions)
        {
            foreach (var archive in Archives)
            {
                foreach (var archiveFile in archive.Files)
                {
                    string filePath = archiveFile.Path;
                    if (HasExtension(archiveFile, extensions))
                    {
                        yield return new ArchiveFileLocation(archive.FileLocation, filePath);
                    }
                }
            }
        }

        public RawFile GetFileFromArchive(string archiveName, string location)
        {
            archiveName = archiveName.ToLower();

            var archive = ArchivesByName[archiveName];

            var file = archive.FileTree.GetFile(location);
            if (file != null)
            {
                RawFile extract = new RawFile(new MemoryStream());

                using (var ctx = archive.GetExtractionContext())
                {
                    archive.ExtractRapid(file, extract.fileStream, ctx.Archive);
                }

                extract.Seek(0, SeekOrigin.Begin);

                extract.SetFileLocation(new ArchiveFileLocation(archive.FileLocation, file.Path));

                return extract;
            }

            return GetFile(location); // just try all other archives
        }
    }
}
