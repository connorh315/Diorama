using BrickVault.Types;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.TEXTURES
{
    public class NxgTextures
    {
        public string Path;
        
        public NuTexture[] Textures;

        private static RawFile GetFromArchive(string path)
        {
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
            NxgTextures textures = new NxgTextures();

            textures.Path = filePath;

            using (RawFile file = new RawFile(filePath))
            {
                uint header = file.ReadUInt(true);
                file.Seek(header, SeekOrigin.Current);

                uint fileSize = file.ReadUInt(true);
                Debug.Assert(file.ReadString(8) == ".CC4TSXT");

                Debug.Assert(file.ReadUInt(true) == 1);
                Debug.Assert(file.ReadString(4) == "TSXT");
                uint textureSheetVersion = file.ReadUInt(true);
                Debug.Assert(textureSheetVersion > 0xb && textureSheetVersion < 0xf);

                string convInfo = file.ReadIntPascalString();
                List<NuTextureHeader> headers = NuSerializer.ReadVectorArray<NuTextureHeader>(file, textureSheetVersion);

                textures.Textures = new NuTexture[headers.Count];

                for (int i = 0; i < textures.Textures.Length; i++)
                {
                    if (headers[i].Name == string.Empty)
                    {
                        Console.WriteLine($"Pulling {headers[i].Path} from game archives!");
                        RawFile loaded = GetFromArchive(headers[i].Path);
                        if (loaded != null)
                        {
                            textures.Textures[i] = NuTexture.Load(loaded);
                        }
                        else
                        {
                            textures.Textures[i] = new NuTexture(); // will load white texture instead
                        }
                    }
                    else
                    {
                        textures.Textures[i] = NuTexture.Load(file);
                    }
                    textures.Textures[i].Header = headers[i];
                }
            }

            return textures;
        }
    }
}
