using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC
{
    public class GScene_4F : GScene
    {
        protected override VertexList GetVertexList()
        {
            int reference = 0;
            int offset = 0;
            uint vertexReference = file.ReadUInt(true);
            if ((vertexReference & 0xc0000000) != 0)
            {
                // Reference to existing vertex list
                reference = (int)(vertexReference & 0xffff);
                uint unk = file.ReadUInt(true);
                offset = file.ReadInt(true);
            }
            else
            {
                // New vertex list
                uint unknown = file.ReadUInt(true); // 0x502??
                reference = referenceCounter++;
                VertexList list = VertexList.Parse(file);
                vertexLists.Add(reference, list);
                offset = file.ReadInt(true);
                Debug.Assert(offset == 0);
            }

            if (!vertexLists.ContainsKey(reference))
            {
                Console.WriteLine();
            }

            return vertexLists[reference];
        }

        protected override ushort[] GetIndexList()
        {
            int reference = 0;
            uint indexReference = file.ReadUInt(true);
            if ((indexReference & 0xc0000000) != 0)
            {
                reference = (int)(indexReference & 0xffff);
                int unknown = file.ReadInt(true);
                return indicesLists[reference];
            }
            else
            {
                file.ReadUInt(true); // unknown
                uint indicesCount = file.ReadUInt(true);
                uint unknown3 = file.ReadUInt(true);

                ushort[] indexBuffer = new ushort[indicesCount];
                for (int idx = 0; idx < indicesCount; idx++)
                {
                    indexBuffer[idx] = file.ReadUShort(false);
                }

                reference = referenceCounter++;
                indicesLists.Add(reference, indexBuffer);
            }

            if (!indicesLists.ContainsKey(reference))
            {
                Console.WriteLine();
            }

            return indicesLists[reference];
        }

        protected virtual void ReadInfo()
        {
            Debug.Assert(file.ReadString(4) == "OFNI");

            uint infoStringCount = file.ReadUInt(true);
            for (int i = 0; i < infoStringCount; i++)
            {
                string infoString = file.ReadPascalString(true, 256);
                //Console.WriteLine($"Info String {i}: {infoString}");
            }
        }

        public NuNameTable NameTable { get; set; }
        protected virtual void ReadNameTable()
        {
            NameTable = NuNameTable.Read(file);
        }

        protected virtual void ReadSplines()
        {
            List<NuSpline> splines = NuSerializer.ReadVectorArray<NuSpline>(file);
        }

        protected virtual void ReadLightmapData()
        {
            Debug.Assert(file.ReadString(4) == "TDML");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 3); 

            if (version >= 3)
            {
                List<NuLightmapData> lightmapData = NuSerializer.ReadVectorArray<NuLightmapData>(file);
            }
        }

        protected virtual void ReadCpuSkinned()
        {
            Debug.Assert(file.ReadString(4) == "SUPC");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 4);

            if (version < 3)
            {
                Debug.Assert(false, "CPUS section not supported");
            }
            else if (version == 3)
            {

            }
            else
            {
                List<NuCpuSkinLod> lods = NuSerializer.ReadVectorArray<NuCpuSkinLod>(file, version);
            }
        }

        protected virtual void ReadDisplayScene()
        {
            
        }

        protected virtual void ReadTextureAnim3SceneBlock()
        {
            Debug.Assert(file.ReadString(4) == "BNAT");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 5);

            List<NuTexAnim3Header> texAnim3Headers = NuSerializer.ReadLegacyVarArray<NuTexAnim3Header>(file);
        }

        protected virtual void ReadAnimSceneBlock()
        {
            Debug.Assert(file.ReadString(4) == "3ALA");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 3 || version == 4, "ala3 vesrion!");
            
            if (version == 3)
            {
                List<NuInstAnim> nuinstanim = NuSerializer.ReadLegacyVarArray<NuInstAnim>(file); // 1wizardofozc2_tech_dx11.gsc
            }
            else if (version == 4)
            {
                List<NuInstAnim_4> nuinstanim = NuSerializer.ReadLegacyVarArray<NuInstAnim_4>(file);
            }

            List<NuStateAnim> nustateanim = NuSerializer.ReadLegacyVarArray<NuStateAnim>(file);

            List<NuAnimHeader> nuanimheader = NuSerializer.ReadLegacyVarArray<NuAnimHeader>(file);
        }

        protected virtual void ReadBlendShapeCharList()
        {
            Debug.Assert(file.ReadString(4) == "BCSB");
            uint version = file.ReadUInt(true);

            List<NuBlendShapeAnimList> nublendshapeanimlist = NuSerializer.ReadLegacyVarArray<NuBlendShapeAnimList>(file, version);
        }

        protected virtual void ReadOccluderList()
        {
            Debug.Assert(file.ReadString(4) == "BCCO");
            uint version = file.ReadUInt(true);
            if (version >= 3)
            {
                List<ushort> nuoccluder = NuSerializer.ReadVectorArray<ushort>(file);
                Debug.Assert(nuoccluder.Count == 0, "occluderlist != 0");
            }
        }

        protected virtual void ReadLSVOctreeBlock()
        {
            Debug.Assert(file.ReadString(4) == "5LVI");
            uint version = file.ReadUInt(true);
            if (version >= 3)
            {
                List<NuLSVOctree> nulsvoctree = NuSerializer.ReadVectorArray<NuLSVOctree>(file);
                Debug.Assert(nulsvoctree.Count == 0);
            }
            else
            {
                List<NuLSVOctree> nulsvoctree = NuSerializer.ReadLegacyVarArray<NuLSVOctree>(file);
            }
        }

        protected virtual void ReadMetaData()
        {
            Debug.Assert(file.ReadString(4) == "ATEM");
            uint version = file.ReadUInt(true);

            if (version >= 0x46)
            {
                List<NuDynamicString> metaStrings = NuSerializer.ReadVectorArray<NuDynamicString>(file);
            }
        }

        protected override void Parse()
        {
            ReadInfo();

            ReadNameTable();

            uint defunctHasSeperateTextureFiles = file.ReadUInt(true);
            if (defunctHasSeperateTextureFiles != 0)
            {
                referenceCounter++;
                TextureHeaders headers = TextureHeaders.Read(file);
                Debug.Assert(defunctHasSeperateTextureFiles == 1);
            }

            ReadSplines();

            List<NuVfxLocator> vfxLocators = NuSerializer.ReadVectorArray<NuVfxLocator>(file);

            Debug.Assert(file.ReadUInt(true) == 1); // possibly number of MESHes?

            Debug.Assert(file.ReadString(4) == "HSEM");

            uint meshVersion = file.ReadUInt(true);
            Debug.Assert(meshVersion == 0xaf);

            uint partCount = file.ReadUInt(true);
            RenderMeshes = new NuRenderMesh[partCount];
            for (int part = 0; part < partCount; part++)
            {
                Debug.Assert(file.ReadUInt(true) == 1); // unknown

                NuRenderMesh mesh = new NuRenderMesh();

                uint vertexBufferCount = file.ReadUInt(true);
                mesh.VertexBuffers = new VertexList[vertexBufferCount];
                for (int vList = 0; vList < vertexBufferCount; vList++)
                {
                    mesh.VertexBuffers[vList] = GetVertexList();
                }

                uint fastBlendVbsCount = file.ReadUInt(true);
                for (int fastVList = 0; fastVList < fastBlendVbsCount; fastVList++)
                {
                    GetVertexList();
                }
                if (fastBlendVbsCount != 0)
                {
                    referenceCounter += 1;
                }

                mesh.Indices = GetIndexList();
                mesh.IndicesBase = file.ReadUInt(true);
                mesh.IndicesCount = file.ReadUInt(true);
                mesh.VerticesBase = file.ReadUInt(true);

                ushort primitiveType = file.ReadUShort(true);
                Debug.Assert(primitiveType == 0, "Unknown primitive type");

                mesh.VerticesCount = file.ReadUInt(true);

                int vbInstBitsAsU32 = file.ReadInt(true);

                List<byte> skinMtxMap = NuSerializer.ReadLegacyVarArray<byte>(file);
                if (skinMtxMap.Count != 0)
                {
                    referenceCounter++;
                }
                //Debug.Assert(skinMtxMap.Count == 0); // legacy array?

                int nuBlendShape = file.ReadInt(true); // i think
                if (nuBlendShape != 0)
                {
                    NuBlendShape.Parse(file, this, meshVersion);
                }
                //Debug.Assert(defunctOptFlags == 0);

                uint defunctOptFlags = file.ReadUInt(true);

                Vector4 centreExtents0 = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                Vector4 centreExtents1 = new Vector4(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

                float densityDiscDiameter = file.ReadFloat(true);

                referenceCounter++;

                geometryLists.Add(referenceCounter++, mesh);

                RenderMeshes[part] = mesh;
            }

            Debug.Assert(file.ReadUInt(true) == 0);
            Debug.Assert(file.ReadUInt(true) == 1);

            NuMaterialData[] materials = NuMaterialData.Read(file);

            List<NuMtlOldReferencedMaterial> embedded_textures = NuSerializer.ReadVectorArray<NuMtlOldReferencedMaterial>(file);

            file.Seek(0x9, SeekOrigin.Current); // TODO: What is this data?

            uint lightmapCount = file.ReadUInt(true);
            Debug.Assert(lightmapCount == 1);
            ReadLightmapData();

            uint cpusCount = file.ReadUInt(true);
            Debug.Assert(cpusCount == 1);
            ReadCpuSkinned();

            DisplayScene = NuDisplayScene.Read(file, NameTable);

            ReadTextureAnim3SceneBlock();

            float playbackFPS = file.ReadFloat(true);

            Debug.Assert(file.ReadUInt(true) == 1);
            ReadAnimSceneBlock();

            Debug.Assert(file.ReadUInt(true) == 0); // possibly portal instances? (SNIP)

            ReadBlendShapeCharList();

            ReadOccluderList();

            ReadLSVOctreeBlock();

            if (NameTable.Version < 0x14)
            {
                List<NuCharacterData> nucharacterdata = NuSerializer.ReadLegacyVarArray<NuCharacterData>(file);
            }
            else
            {
                List<NuCharacterData> nucharacterdata = NuSerializer.ReadVectorArray<NuCharacterData>(file);
            }

            uint oldWiiMeshSceneBlockLinkArrayCount = file.ReadUInt(true);

            ReadMetaData();

            if (NameTable.Version < 0x51)
            {
                uint texhdrsceneblock = file.ReadUInt(true);
                Debug.Assert(texhdrsceneblock == 0);
            }

            byte useSingleLodAnim = file.ReadByte();

            uint numblendshapes = file.ReadUInt(true);

            List<ushort> unk = NuSerializer.ReadVectorArray<ushort>(file); // pad data

            uint unk2 = file.ReadUInt(true);
            Debug.Assert(unk2 == 0);

        }
    }
}
