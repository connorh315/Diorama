using Diorama.Core.Filetypes.TEXTURES;
using Diorama.Editor;
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

        public string Name { get => Original?.Header?.Name ?? ""; }

        public string GscName;

        public bool Deleted { get; private set; } = false;

        public void Delete()
        {
            Original?.Header?.Name = "DELETED TEXTURE";
            Deleted = true;
        }

        public NuTexture Original;

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

        private static RenderTexture invalidTexture;
        public static RenderTexture GetInvalidTexture()
        {
            if (invalidTexture == null)
            {
                invalidTexture = new RenderTexture();
                invalidTexture.CreateTextureFromData(new byte[] { 255, 0, 255, 255, 0, 0, 0, 255, 0, 0, 0, 255, 255, 0, 255, 255}, 2, 2); // pink/black checkerboard pattern
            }
            return invalidTexture;
        }

        public RenderTexture()
        {
            Handle = GL.GenTexture();

            Use();
        }

        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            if (Deleted)
            {
                GL.BindTexture(TextureTarget.Texture2D, GetInvalidTexture().Handle);
                return;
            }
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        private void CreateTextureFromData(byte[] data, int width, int height)
        {
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba8,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        private void CreateWhiteTexture() => CreateTextureFromData(new byte[] { 255, 255, 255, 255 }, 1, 1);

        public void Reload(NuTexture texture)
        {
            Use();

            Original = texture;

            int blockSize = 0;
            int uncompressedPixelSize = 0;
            InternalFormat compressionFormat;
            switch (texture.FourCC)
            {
                case 0x31545844: // DXT1
                    if (texture.Discriminator1 == 0x48504c41) // ALPH
                        compressionFormat = InternalFormat.CompressedRgbaS3tcDxt1Ext;
                    else //(texture.Discriminator1 == 0x5141504f) // OPAQ
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
                }

                offset += mipSize;

                width /= 2;
                height /= 2;
            }
        }

        public static RenderTexture FromNuTexture(NuTexture texture)
        {
            RenderTexture renderTexture = new RenderTexture();

            renderTexture.Original = texture;

            if (texture.Data == null)
            {
                renderTexture.CreateWhiteTexture();
                return renderTexture;
            }

            renderTexture.Reload(texture);

            renderTexture.GscName = texture.Header.Name;

            return renderTexture;
        }
    }
}
