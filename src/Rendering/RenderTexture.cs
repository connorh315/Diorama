using Diorama.Core.Filetypes.TEXTURES;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Rendering
{
    public class RenderTexture
    {
        public int Handle;

        public string Name;

        private static RenderTexture whiteTexture;
        public static RenderTexture GetWhiteTexture()
        {
            if (whiteTexture == null)
            {
                whiteTexture = new RenderTexture();
                whiteTexture.CreateWhiteTexture();
            }

            return whiteTexture;
        }

        public RenderTexture()
        {
            Handle = GL.GenTexture();

            Use();
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        private void CreateWhiteTexture()
        {
            byte[] whitePixel = { 255, 255, 255, 255 }; // RGBA

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba8,
                1,
                1,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                whitePixel);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        public static RenderTexture FromNuTexture(NuTexture texture)
        {
            RenderTexture renderTexture = new RenderTexture();

            renderTexture.Name = texture.Header.Name;

            if (texture.Data == null)
            {
                renderTexture.CreateWhiteTexture();
                return renderTexture;
            }

            int blockSize = 0;
            int uncompressedPixelSize = 0;
            InternalFormat compressionFormat;
            switch (texture.FourCC)
            {
                case 0x31545844: // DXT1
                    //if (texture.Discriminator1 == 0x48504c41) // ALPH
                    //    compressionFormat = InternalFormat.CompressedRgbaS3tcDxt1Ext;
                    //else //(texture.Discriminator1 == 0x5141504f) // OPAQ
                        compressionFormat = InternalFormat.CompressedRgbS3tcDxt1Ext;
                    blockSize = 8;
                    break;
                case 0x33545844: // DXT3
                    compressionFormat = InternalFormat.CompressedRgbaS3tcDxt3Ext;
                    blockSize = 16;
                    break;
                case 0x35545844: // DXT5
                    compressionFormat = InternalFormat.CompressedRgbaS3tcDxt5Ext;
                    blockSize = 16;
                    break;
                case 0x74:
                    compressionFormat = InternalFormat.Rgba32f;
                    blockSize = 0;
                    uncompressedPixelSize = 16;
                    break;
                case 0x30315844: // DX10
                    compressionFormat = InternalFormat.Rgba8Snorm;
                    blockSize = 16; // safe default for BC formats
                    break;
                default:
                    throw new Exception("Unsupported version!");
                    blockSize = 4; // assume RGBA8
                    break;
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, texture.MipCount - 1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            int width = texture.Width;
            int height = texture.Height;

            int offset = 0; // where mip data starts

            for (int i = 0; i < texture.MipCount; i++)
            {
                int w = Math.Max(1, width);
                int h = Math.Max(1, height);

                int mipSize;

                if (width == 0 || height == 0)
                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, i - 1);
                    break;
                }

                if (texture.IsCompressed)
                {
                    int bw = (w + 3) / 4;
                    int bh = (h + 3) / 4;
                    mipSize = bw * bh * blockSize;

                    GL.CompressedTexImage2D(
                        TextureTarget.Texture2D,
                        i,
                        compressionFormat,
                        w,
                        h,
                        0,
                        mipSize,
                        texture.Data.AsSpan(offset, mipSize).ToArray()
                    );
                }
                else
                {
                    mipSize = w * h * uncompressedPixelSize;

                    GL.TexImage2D(
                        TextureTarget.Texture2D,
                        i,
                        (PixelInternalFormat)compressionFormat,
                        w,
                        h,
                        0,
                        PixelFormat.Rgba,
                        PixelType.Byte,
                        texture.Data.AsSpan(offset, mipSize).ToArray()
                    );

                    //GL.TexImage2D(
                    //    TextureTarget.Texture2D,
                    //    i,
                    //    (PixelInternalFormat)compressionFormat,
                    //    w,
                    //    h,
                    //    0,
                    //    PixelFormat.Rgba,
                    //    PixelType.Float,
                    //    texture.Data.AsSpan(offset, mipSize).ToArray()
                    //);
                }

                offset += mipSize;

                width /= 2;
                height /= 2;
            }

            return renderTexture;
        }
    }
}
