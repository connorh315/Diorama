using BrickVault;
using BrickVault.Types;
using RawFile = Diorama.Core.RawFile;

namespace Diorama.Tests
{
    public class MassThreadExtractionContext
    {
        public byte[] CompressionArray = new byte[32000000];
        public byte[] DecompressionArray = new byte[96000000];
        public RawFile OutputFile = new RawFile(new MemoryStream());

        public int Count;

        public static void ForEachGscFile<TContext>(
            string gamePath,
            int maxDegree,
            Func<TContext> initialize,
            Action<TContext, ArchiveFile, RawFile> process,
            Action<TContext>? finalize = null)
        {
            Parallel.ForEach(
                Directory.GetFiles(gamePath, "*.DAT", SearchOption.AllDirectories),
                new ParallelOptions { MaxDegreeOfParallelism = maxDegree },
                () => (Context: initialize(), Extraction: new MassThreadExtractionContext()),
                (archive, _, state) =>
                {
                    var (ctx, extraction) = state;

                    var dat = DATFile.Open(archive);
                    if (dat == null) return state;

                    using var datFile = new BrickVault.RawFile(archive);

                    foreach (var entry in dat.Files)
                    {
                        if (!entry.Path.EndsWith(".gsc", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (extraction.CompressionArray.Length < entry.CompressedSize)
                            extraction.CompressionArray = new byte[entry.CompressedSize];

                        if (extraction.DecompressionArray.Length < entry.DecompressedSize)
                            extraction.DecompressionArray = new byte[entry.DecompressedSize];

                        var file = extraction.OutputFile;
                        file.fileStream.Position = 0;

                        dat.Extract(entry, file.fileStream, datFile,
                                    extraction.CompressionArray,
                                    extraction.DecompressionArray);

                        file.fileStream.Position = 0;
                        file.Opaque = entry.Path;

                        process(ctx, entry, file);
                    }

                    return state;
                },
                state => finalize?.Invoke(state.Context)
            );
        }
    }

    public class LocalContext
    {
        public int Succeeded;
        public int Count;
        public Dictionary<string, GscFileFailureInfo> Failed = new();
    }
}
