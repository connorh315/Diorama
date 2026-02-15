using Diorama.Core.Filetypes.GSC.Components;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.TEXTURES
{
    public class NuTexture
    {
        public NuTextureHeader Header;

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

        public byte[] Data;

        public bool IsCompressed = true;

        private int Calculate(RawFile file)
        {
            long startPos = file.Position;
            Debug.Assert(file.ReadString(4) == "DDS ");

            file.Seek(8, SeekOrigin.Current);

            Height = file.ReadInt();
            Width = file.ReadInt();

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
                    file.Seek(20, SeekOrigin.Current);
                    blockSize = 16; // safe default for BC formats
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

            if (IsCubemap)
                totalDataSize *= 6;

            file.Seek(startPos + 4 + HeaderSize, SeekOrigin.Begin);

            Data = file.ReadArray(totalDataSize);

            return 4 + HeaderSize + totalDataSize;
        }

        public static NuTexture Load(RawFile file)
        {
            NuTexture texture = new NuTexture();

            texture.Offset = file.Position;
            texture.Size = (uint)texture.Calculate(file);

            return texture;
        }
    }
}
