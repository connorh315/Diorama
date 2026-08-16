using Avalonia.Input;
using Diorama.Core.Filetypes.GSC.Components;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.TEXTURES
{
    public class NuTexture : ISchemaSerializable
    {
        public NuTexGenHdr Header;

        public long Offset;
        public uint Size;

        public int HeaderSize = 124;

        public int Width;
        public int Height;

        public int MipCount;

        public uint FourCC;

        public uint Discriminator1;
        public uint Discriminator2;

        public bool IsCubemap;
        public bool HasVolume;

        public byte[] ImageHeader;
        public byte[] Data;
        public byte[] AdditionalData;

        public bool IsCompressed = true;

        public uint Dx10Format;

        internal int Calculate(RawFile file)
        {
            long startPos = file.Position;
            Debug.Assert(file.ReadString(4) == "DDS ");

            file.Seek(8, SeekOrigin.Current);

            Height = file.ReadInt();
            Width = file.ReadInt();

            if (Width == 0)
            {
                Console.WriteLine();
            }

            file.Seek(8, SeekOrigin.Current);

            MipCount = Math.Max(file.ReadInt(), 1);

            file.Seek(52, SeekOrigin.Current);

            FourCC = file.ReadUInt();

            file.Seek(20, SeekOrigin.Current);

            uint caps = file.ReadUInt();
            uint caps2 = file.ReadUInt();

            Discriminator1 = file.ReadUInt();
            Discriminator2 = file.ReadUInt();

            file.Seek(4, SeekOrigin.Current);

            IsCubemap = (caps2 & 0x00000200) != 0;
            HasVolume = (caps2 & 0x00400000) != 0;

            int blockSize;

            switch (FourCC)
            {
                case 0x31545844: // DXT1
                    blockSize = 8;
                    break;
                case 0x33545844: // DXT3
                case 0x35545844: // DXT5
                    blockSize = 16;
                    break;
                case 0x30315844: // DX10
                    HeaderSize += 20;
                    Dx10Format = file.ReadUInt();
                    file.Seek(16, SeekOrigin.Current);
                    IsCompressed = false;
                    blockSize = 4;
                    //blockSize = 16; // safe default for BC formats
                    break;
                case 0x74:
                    IsCompressed = false;
                    blockSize = 16;
                    break;
                default:
                    IsCompressed = false;
                    blockSize = 4; // assume RGBA8
                    break;
            }

            int totalDataSize = 0;

            if (Width > 0 && Height > 0)
            {
                for (int mip = 0; mip < MipCount; mip++)
                {
                    int w = Math.Max(1, Width >> mip);
                    int h = Math.Max(1, Height >> mip);

                    if (IsCompressed)
                    {
                        int bw = (w + 3) / 4;
                        int bh = (h + 3) / 4;
                        totalDataSize += bw * bh * blockSize;
                    }
                    else
                    {
                        totalDataSize += w * h * blockSize;
                    }
                }
            }

            if (IsCubemap)
                totalDataSize *= 6;

            file.Seek(startPos, SeekOrigin.Begin);
            ImageHeader = file.ReadArray(4 + HeaderSize);
            Data = file.ReadArray(totalDataSize);

            int additionalDataSize = 0;
            if (HasVolume)
            {
                additionalDataSize = file.ReadInt(true); 
                if (additionalDataSize % 4 != 0)
                {
                    Console.WriteLine("Unexpected cubemap size!!!");
                }

                AdditionalData = file.ReadArray(additionalDataSize);
            }

            return 4 + HeaderSize + totalDataSize + additionalDataSize;
        }

        public static NuTexture Load(RawFile file, NuTexGenHdr header)
        {
            SchemaSerializer schema = new SchemaSerializer(file, false);

            NuTexture texture = new NuTexture();
            texture.Header = header;
            texture.Handle(schema, 0);
            header.Level = (uint)texture.MipCount;
            return texture;
        }

        public static NuTexture Load(string filePath, NuTexGenHdr header)
        {
            using (RawFile file = new RawFile(filePath))
            {
                return Load(file, header);
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (schema.Writing)
            {
                if (!string.IsNullOrEmpty(Header.Name))
                {
                    schema.HandleArray(ref ImageHeader, ImageHeader.Length);
                    schema.HandleArray(ref Data, Data.Length);
                }
            }
            else
            {
                if (Header.Name != string.Empty)
                {
                    Size = (uint)Calculate(schema.File);
                }
            }
        }
    }
}
