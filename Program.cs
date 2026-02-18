using Avalonia;
using BrickVault.Types;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;

namespace Diorama
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                //.With(new Win32PlatformOptions
                //{
                //    RenderingMode = new Collection<Win32RenderingMode> { Win32RenderingMode.Wgl }
                //})
                .LogToTrace();

        static void Main2(string[] args)
        {

            //ParseFile(@"A:\ADDITIONALCONTENT\OPUS_ADVENTURETIME\LEVELS\LEVELPACK\OPUS_ADVENTURETIME\OPUS_ADVENTURETIME_MIDTRO3\CUT_GIZ_ENCHIRIDION_BOOTH_DX11.GSC");
            //TryParseFile(@"A:\CHARS\CREATURE\GHOST_75827\GHOST_75827_KRAWLIE_DX11.GHG");
            //TryParseFile(@"A:\CHARS\SUPER_CHARACTER\FACE\FACE_METALBEARD_DX11.GHG");
            //TryParseFile(@"A:\LEVELS\BUILDER\BUILDERGAMEMECHANICS\BUILDERMASTERBUILD\BUILDERMASTERBUILD_DX11.GSC");
            //TryParseFile("A:\\levels\\builder\\buildergamemechanics\\builderghostreceptor\\builderghostreceptor_dx11.gsc");
            //ParseFile("A:\\levels\\vfx\\vfx_story\\vfx_1wizardofoz\\vfx_1wizardofoza\\vfx_1wizardofoza_dx11.gsc");
            //TryParseFile("A:\\levels\\vfx\\vfx_ipsharedscenes\\vfx_puncheffects\\vfx_puncheffects_dx11.gsc");
            //return;

            int counter = 0;
            int total = 0;

            var datFiles = Directory.GetFiles(
                @"F:\PS4Games\CUSA01176\data",
                "*.DAT",
                SearchOption.AllDirectories
            );
            //Parallel.ForEach(
            //    datFiles,
            //    new ParallelOptions { MaxDegreeOfParallelism = 4 },
            //datPath =>
            byte[] compressedShare = new byte[32000000];
            byte[] decompressedShare = new byte[96000000];

            using var file = new RawFile(new MemoryStream(100000000));
            foreach (string datPath in datFiles)
            {
                var dat = DATFile.Open(datPath);


                using (var datFile = new BrickVault.RawFile(dat.FileLocation))
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

                        Interlocked.Increment(ref total);

                        file.fileStream.Position = 0;

                        dat.Extract(entry, file.fileStream, datFile, compressedShare, decompressedShare);
                        file.fileStream.Position = 0;

                        file.Opaque = entry.Path;

                        if (TryParseFile(file))
                        {
                            Interlocked.Increment(ref counter);
                        }
                    }
                }
            }
            //);

            Console.WriteLine($"Successfully processed {counter} meshes out of {total} files.");
            foreach (var pair in countFails)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }

        static ConcurrentDictionary<string, int> countFails =
            new ConcurrentDictionary<string, int>();


        static bool TryParseFile(string path)
        {
            try
            {
                ParseFile(path, false);
                return true;
            }
            catch (Exception ex)
            {
                if (!ex.Message.StartsWith("Unsupported NU20 version"))
                    //Console.WriteLine($"Failed to parse {path}: {ex}");

                    if (!countFails.ContainsKey(ex.Message))
                        countFails[ex.Message] = 0;

                countFails[ex.Message]++;
            }
            return false;
        }

        static bool TryParseFile(RawFile file)
        {
            try
            {
                ParseRawFile(file);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "should be 0")
                {
                    Console.WriteLine();
                }

                //if (!ex.Message.StartsWith("Unsupported NU20 version"))
                //    Console.WriteLine($"Failed to parse {file.FileLocation}: {ex}");

                countFails.AddOrUpdate(
                    ex.Message,
                    1,                  // if key does not exist
                    (_, old) => old + 1 // if key exists
                );
            }
            return false;
        }

        static void ParseRawFile(RawFile file)
        {
            GScene gsc = GScene.Parse(file);
        }

        static void ParseFile(string path, bool shouldExport = true)
        {
            GScene file = GScene.Parse(path);

            for (int meshId = 0; meshId < file.RenderMeshes.Length; meshId++)
            {
                NuRenderMesh mesh = file.RenderMeshes[meshId];
                List<string> lines = new List<string>();
                for (int i = 0; i < mesh.VertexBuffers.Length; i++)
                {
                    VertexList vList = mesh.VertexBuffers[i];
                    for (uint j = mesh.VerticesBase; j < mesh.VerticesBase + mesh.VerticesCount; j++)
                    {
                        Vertex v = vList.Vertices[j];
                        lines.Add($"v {v.Position.X:F6} {v.Position.Y:F6} {v.Position.Z:F6}");
                    }
                    break;
                }

                WriteObjFacesFromTriangleStrip(lines, mesh.Indices, mesh.IndicesBase, mesh.IndicesCount, mesh.VerticesBase, mesh.VerticesCount);
                if (shouldExport)
                {
                    File.WriteAllLines($@"A:\output{meshId}.obj", lines);
                }
            }
        }

        static void WriteObjFacesFromTriangleStrip(
            List<string> lines,
            ushort[] indices,
            uint indicesOffset,
            uint indicesCount,
            uint verticesOffset,
            uint verticesCount
        )
        {
            bool flip = false;

            for (uint i = indicesOffset; i < indicesOffset + indicesCount; i += 3)
            {
                ushort i0 = (ushort)(indices[i] + 1);
                ushort i1 = (ushort)(indices[i + 1] + 1);
                ushort i2 = (ushort)(indices[i + 2] + 1);

                // Degenerate triangle → strip restart or stitch
                if (i0 == i1 || i1 == i2 || i0 == i2)
                {
                    flip = false; // reset winding after degenerates
                    continue;
                }

                if (!flip)
                {
                    lines.Add($"f {i0} {i1} {i2}");
                }
                else
                {
                    lines.Add($"f {i1} {i0} {i2}");
                }

                flip = !flip;
            }
        }

    }
}
