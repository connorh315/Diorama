using BrickVault;
using BrickVault.Types;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using RawFile = Diorama.Core.RawFile;

namespace Diorama.Tests
{
    public abstract class GscTestBase
    {
        private bool TryParseFile(RawFile file, out string? errorMessage)
        {
            try
            {
                var scene = GScene.Parse(file);
                errorMessage = null;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private void AddFailedFile(Dictionary<string, GscFileFailureInfo> failed, string path, string reason)
        {
            if (reason.StartsWith("Attempting to read string"))
            {
                reason = "Invalid string read attempt";
            }

            if (!failed.TryGetValue(reason, out var failureInfo))
            {
                failureInfo = new GscFileFailureInfo();
                failed[reason] = failureInfo;
            }

            failureInfo.AddFile(path);
        }

        protected void DeserializeAll(string gamePath)
        {
            byte[] compressedShare = new byte[32000000];
            byte[] decompressedShare = new byte[96000000];

            using var file = new RawFile(new MemoryStream(100000000));

            Dictionary<string, GscFileFailureInfo> failed = new();

            int succeeded = 0;
            int count = 0;
            foreach (var archive in Directory.GetFiles(gamePath, "*.DAT", SearchOption.AllDirectories))
            {
                var dat = DATFile.Open(archive);

                if (dat == null)
                    continue;

                using (BrickVault.RawFile datFile = new BrickVault.RawFile(archive))
                {
                    foreach (var entry in dat.Files)
                    {
                        if (!entry.Path.EndsWith("gsc"))
                            continue;

                        if (compressedShare.Length < entry.CompressedSize)
                        {
                            compressedShare = new byte[entry.CompressedSize];
                        }

                        if (decompressedShare.Length < entry.DecompressedSize)
                        {
                            decompressedShare = new byte[entry.DecompressedSize];
                        }

                        count++;

                        file.fileStream.Position = 0;

                        dat.Extract(entry, file.fileStream, datFile, compressedShare, decompressedShare);
                        file.fileStream.Position = 0;

                        file.Opaque = entry.Path;

                        if (TryParseFile(file, out var errorMessage))
                        {
                            succeeded++;
                        }
                        else
                        {
                            AddFailedFile(failed, entry.Path, errorMessage);
                        }
                    }
                }
            }

            foreach (string failedReason in failed.Keys)
            {
                var failureInfo = failed[failedReason];

                Console.WriteLine($"{failedReason} - {failureInfo.Count}");
                foreach (var path in failureInfo.Paths)
                {
                    Console.WriteLine($"\t{path}");
                }
            }
        }
    }
}
