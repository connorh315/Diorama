using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC
{
    public abstract class GScene
    {
        public string Path;

        internal RawFile file;

        public byte[] ResourceHeaderBlock;

        public uint NU20Version;

        public NuDisplayScene DisplayScene;

        public NuMaterialData[] Materials;

        //public List<NuLightmapData> Lightmaps;
        public NuLightmapDataBlock LightmapDataBlock;

        public NuMeshSceneBlock MeshSceneBlock { get; set; }

        public byte[] Trailer;

        protected abstract void Parse(GSerializationContext ctx);

        public void Write(RawFile file, GSerializationContext ctx)
        {
            file.WriteInt(ResourceHeaderBlock.Length, true);
            file.WriteArray(ResourceHeaderBlock);

            using (RawFileSection nu20Section = new RawFileSection(file, false, true))
            {
                file.WriteInt(1, true);

                file.WriteString("02UN");
                file.WriteUInt(NU20Version, true);

                WriteNu20(file, ctx);
            }


            file.WriteArray(Trailer);
        }

        internal abstract void WriteNu20(RawFile file, GSerializationContext ctx);

        public static GScene Parse(RawFile file)
        {
            GScene gsc;

            int resourceHeaderSize = file.ReadInt(true);
            byte[] resourceHeaderBlock = file.ReadArray(resourceHeaderSize);
            //file.Seek(resourceHeaderSize, SeekOrigin.Current);

            uint gscSize = file.ReadUInt(true);

            Debug.Assert(file.ReadUInt(true) == 1);

            Debug.Assert(file.ReadString(4) == "02UN");

            uint nu20Version = file.ReadUInt(true);
            switch (nu20Version)
            {
                case 0x4f:
                case 0x50:
                case 0x52:
                case 0x53:
                case 0x57:
                    gsc = new GScene_4F();
                    break;
                default:
                    throw new Exception($"Unsupported NU20 version: {nu20Version}");
            }

            gsc.NU20Version = nu20Version;
            gsc.file = file;
            gsc.ResourceHeaderBlock = resourceHeaderBlock;

            GSerializationContext context = new GSerializationContext();
            gsc.Parse(context);
            gsc.Path = gsc.file.FileLocation;

            if (file.Position != resourceHeaderSize + 4 + 4 + gscSize)
            {
                throw new Exception("Did not read entire file size!");
            }

            int length = (int)(file.fileStream.Length - file.Position);
            if (length < 0x100)
            {
                gsc.Trailer = file.ReadArray(length);
            }

            return gsc;
        }

        public static GScene Parse(string filePath)
        {
            using (RawFile file = new RawFile(filePath))
            {
                return Parse(file);
            }
        }
    }
}
