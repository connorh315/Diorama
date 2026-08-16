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

        public TextureTarget Target { get; private set; } = TextureTarget.Texture2D;

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
                GL.BindTexture(Target, GetInvalidTexture().Handle);
                return;
            }
            GL.BindTexture(Target, Handle);
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

            GL.TexParameter(Target, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(Target, TextureParameterName.TextureMaxLevel, texture.MipCount - 1);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            if (Target == TextureTarget.TextureCubeMap)
            {
                GL.TexParameter(
                    Target,
                    TextureParameterName.TextureWrapR,
                    (int)TextureWrapMode.ClampToEdge);
            }

            if (Target == TextureTarget.Texture2D)
            {
                UploadFace(texture, Target, 0, compressionFormat, blockSize, uncompressedPixelSize);
            }
            else if (Target == TextureTarget.TextureCubeMap)
            {
                TextureTarget[] faces =
                [
                    TextureTarget.TextureCubeMapPositiveX,
                    TextureTarget.TextureCubeMapNegativeX,
                    TextureTarget.TextureCubeMapPositiveY,
                    TextureTarget.TextureCubeMapNegativeY,
                    TextureTarget.TextureCubeMapPositiveZ,
                    TextureTarget.TextureCubeMapNegativeZ
                ];

                int offset = 0;

                foreach (TextureTarget face in faces)
                {
                    offset = UploadFace(
                        texture,
                        face,
                        offset,
                        compressionFormat,
                        blockSize,
                        uncompressedPixelSize);
                }
            }
        }

        private int UploadFace(
            NuTexture texture,
            TextureTarget faceTarget,
            int offset,
            InternalFormat compressionFormat,
            int blockSize,
            int uncompressedPixelSize)
        {
            int width = texture.Width;
            int height = texture.Height;

            if (width == 0 && height == 0) return offset;

            for (int mip = 0; mip < texture.MipCount; mip++)
            {
                int w = Math.Max(1, width);
                int h = Math.Max(1, height);

                int mipSize;

                if (texture.IsCompressed)
                {
                    int bw = (w + 3) / 4;
                    int bh = (h + 3) / 4;

                    mipSize = bw * bh * blockSize;

                    GL.CompressedTexImage2D(
                        faceTarget,
                        mip,
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
                        faceTarget,
                        mip,
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

            return offset;
        }

        public static RenderTexture FromNuTexture(NuTexture texture)
        {
            RenderTexture renderTexture = new RenderTexture();

            renderTexture.Original = texture;

            renderTexture.GscName = texture.Header.Name;

            renderTexture.Target = texture.IsCubemap ? TextureTarget.TextureCubeMap : TextureTarget.Texture2D;

            if (texture.Data == null)
            {
                renderTexture.CreateWhiteTexture();
                return renderTexture;
            }

            renderTexture.Reload(texture);

            

            return renderTexture;
        }
    }
}
