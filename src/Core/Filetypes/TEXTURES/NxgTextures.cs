using Avalonia.Input;
using BrickVault.Types;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.GSC.Components.RESH;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.TEXTURES
{
    public class NxgTextures : ISchemaSerializable
    {
        public string Path;
        
        private static RawFile GetFromArchive(string path)
        {
            return null;

            if (string.IsNullOrEmpty(Settings.DatLocation)) return null;

            string normalised = path.Replace('/', '\\').TrimStart('\\').ToLower();

            foreach (var file in Directory.EnumerateFiles(Settings.DatLocation, "*.DAT", SearchOption.AllDirectories))
            {
                var dat = DATFile.Open(file);
                foreach (var archiveFile in dat.Files)
                {
                    if (archiveFile.Path == normalised)
                    {
                        return new RawFile(dat.Extract(archiveFile));
                    }
                }
            }

            return null;
        }

        public static NxgTextures Read(string filePath)
        {
            SchemaSerializer schema = new SchemaSerializer(new RawFile(filePath), false);

            NxgTextures textures = new NxgTextures();

            textures.Path = filePath;

            textures.Handle(schema, 0);

            return textures;
        }

        public static NxgTextures Read(RawFile file)
        {
            SchemaSerializer schema = new SchemaSerializer(file, false);

            NxgTextures textures = new NxgTextures();

            textures.Path = file.FileLocation;

            textures.Handle(schema, 0);

            return textures;
        }

        public NuResourceHeader ResourceHeader;

        public NuTextureSet TextureSet;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Handle(ref ResourceHeader);

            using (schema.HandleRegion())
            {
                schema.Expect(".CC4TSXT");
                schema.HandleOptional(ref TextureSet);
            }

            TextureSet?.HandleImageContent(schema, 0);

            if (!schema.Writing)
            {
                for (int i = 0; i < TextureSet.Textures.Length; i++)
                {
                    var tex = TextureSet.Textures[i];
                    if (tex.Header.Name == string.Empty)
                    {
                        Console.WriteLine($"Pulling {tex.Header.Path} from game archives!");
                        RawFile loaded = GetFromArchive(tex.Header.Path);
                        if (loaded != null)
                        {
                            TextureSet.Textures[i].Calculate(loaded);
                        }
                        // otherwise white texture will default
                    }
                }
            }
        }
    }
}
