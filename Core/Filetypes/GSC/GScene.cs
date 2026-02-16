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
        internal RawFile file;

        internal Dictionary<int, VertexList> vertexLists = new();
        internal Dictionary<int, ushort[]> indicesLists = new();
        internal Dictionary<int, NuRenderMesh> geometryLists = new();
        internal int referenceCounter = 5;

        public NuRenderMesh[] RenderMeshes;

        public uint NU20Version;

        public NuDisplayScene DisplayScene;

        public NuMaterialData[] Materials;

        public List<NuLightmapData> Lightmaps;

        protected abstract void Parse();

        protected abstract VertexList GetVertexList();

        protected abstract ushort[] GetIndexList();

        public static GScene Parse(RawFile file)
        {
            GScene gsc;

            uint resourceHeaderSize = file.ReadUInt(true);
            file.Seek(resourceHeaderSize, SeekOrigin.Current);

            uint gscSize = file.ReadUInt(true);

            Debug.Assert(file.ReadUInt(true) == 1);

            Debug.Assert(file.ReadString(4) == "02UN");

            uint nu20Version = file.ReadUInt(true);
            switch (nu20Version)
            {
                case 0x4f:
                    gsc = new GScene_4F();
                    break;
                case 0x50:
                case 0x53:
                    gsc = new GScene_50();
                    break;
                default:
                    throw new Exception($"Unsupported NU20 version: {nu20Version}");
            }

            gsc.NU20Version = nu20Version;
            gsc.file = file;

            gsc.Parse();

            if (file.Position != resourceHeaderSize + 4 + 4 + gscSize)
            {
                throw new Exception("Did not read entire file size!");
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
