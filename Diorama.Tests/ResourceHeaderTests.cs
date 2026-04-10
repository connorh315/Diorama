using Avalonia.Remote.Protocol;
using BrickVault;
using BrickVault.Types;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components.RESH;

using RawFile = Diorama.Core.RawFile;

namespace Diorama.Tests;

[TestClass]
public class ResourceHeaderTests
{
    [TestMethod]
    public void GetResourceHeaderTypes()
    {
        Dictionary<uint, List<string>> lookup = new();

        string gameLocation = @"G:\SteamLibrary\steamapps\common\LEGO Batman 3 Beyond Gotham";
        foreach (string datPath in Directory.EnumerateFiles(gameLocation, "*.DAT", SearchOption.AllDirectories))
        {
            DATFile dat = DATFile.Open(datPath);
            if (dat == null) continue;

            var extractionCtx = dat.GetExtractionContext();

            using (RawFile memoryFile = new RawFile(new MemoryStream()))
            {
                foreach (var gscFile in dat.GetFilesWithExtension(".GSC"))
                {
                    memoryFile.Seek(0, SeekOrigin.Begin);

                    dat.ExtractFile(gscFile, extractionCtx, memoryFile.fileStream);

                    NuResourceHeader header = GComponentFactory.Parse<NuResourceHeader>(memoryFile);

                    Dictionary<int, string> filetreeLookup = header.FileTree.GetIndexedFiles();
                    foreach (var reference in header.References)
                    {
                        string extension = Path.GetExtension(filetreeLookup[(int)reference.Hash]);
                        if ((reference.PlatformsAndClasses & 0xbd) == 0) continue;
                        if (!lookup.TryAdd(reference.PlatformsAndClasses, new List<string> { filetreeLookup[(int)reference.Hash] }))
                        {
                            if (lookup[reference.PlatformsAndClasses].Count < 20)
                                lookup[reference.PlatformsAndClasses].Add(filetreeLookup[(int)reference.Hash]);
                        }
                    }
                }
            }
        }

        foreach (var entry in lookup)
        {
            Console.WriteLine($"ID {entry.Key:X}:");
            foreach (var item in entry.Value)
            {
                Console.WriteLine($"\t{item}");
            }
        }
    }
}
