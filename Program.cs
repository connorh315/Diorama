using BrickVault.Types;
using Diorama.Filetypes.GSC;
using Diorama.Filetypes.GSC.Components;
using System.Diagnostics.Metrics;

namespace Diorama
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ParseFile(@"A:\ADDITIONALCONTENT\RELEASE1\LEVELS\LEVELPACK\LP2_THESIMPSONS\LP2_THESIMPSONSE\LP2_THESIMPSONSE2_DX11.GSC");
            //return;

            int counter = 0;
            int total = 0;
            foreach (var datPath in Directory.GetFiles("F:\\PS4Games\\CUSA01176\\data\\", "*.DAT", SearchOption.AllDirectories))
            {
                if (datPath.Contains("GAME")) continue;

                var dat = DATFile.Open(datPath);
                foreach (var entry in dat.Files)
                {
                    if (entry.Path.EndsWith("gsc"))
                    {
                        total++;
                        using (BrickVault.RawFile file = BrickVault.RawFile.Create(@"A:\testfile.gsc"), datFile = new BrickVault.RawFile(dat.FileLocation))
                        {
                            dat.ExtractFile(entry, datFile, file.fileStream);
                        }

                        bool success = TryParseFile(@"A:\testfile.gsc");
                        if (success)
                        {
                            counter++;
                        }
                    }
                }
            }
            Console.WriteLine($"Successfully processed {counter} meshes out of {total} files.");
            foreach (var pair in countFails)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }

        static Dictionary<string, int> countFails = new();

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
                    Console.WriteLine($"Failed to parse {path}: {ex}");

                if (!countFails.ContainsKey(ex.Message))
                    countFails[ex.Message] = 0;

                countFails[ex.Message]++;
            }
            return false;
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
                ushort i0 = (ushort)(indices[i ] + 1);
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
