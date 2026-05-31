using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuCharacterData : ISchemaSerializable
    {
        public uint Version;

        public List<NuJointData> JointData;
        public List<NuMtx> T;
        public List<NuMtx> Inv_Wt;
        public List<byte> JointIxs;

        public List<NuPointOfInterest> PointsOfInterest;
        public List<byte> PoiIxs;

        public byte[] Buffer;

        public List<NuLayer_SpecialFlags> LayerMetadata;
        public List<NuLayerData> Layers;

        public List<NuShadowData> ShadowData;

        public float SphereRadius;
        public float SphereYOff;

        public Vector3 Min;
        public Vector3 Max;

        public float CylinderYOff;
        public float CylinderHeight;
        public float CylinderRadius;
        public float LodBoundary;

        public List<byte> DefunctTopLodRemapTable;
        public List<byte> LodRemapTable;

        public float DeprecatedModelRenderScale;

        public List<short> LodSpecialRemapTable;

        public byte KrawlyLod;

        public uint JointNameHash;
        public byte IsReplacementMesh;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            SchemaSerializer temp = new SchemaSerializer(file, false);
            
            Debug.Assert(file.ReadString(4) == "LOGH");
            uint version = file.ReadUInt(true);
            Debug.Assert(version == 0x10 || version == 0x11, $"hgol version: {version:X2}");
            if (version < 0xc)
            {
                Debug.Assert(1 == 0, "unsupported HGOL version!");
            }
            else
            {
                List<NuJointData> jointData = NuSerializer.ReadVectorArray<NuJointData>(file, version);
                List<NuMtx> T = NuSerializer.ReadVectorArray<NuMtx>(file);
                List<NuMtx> inv_wt = NuSerializer.ReadVectorArray<NuMtx>(file);
                List<byte> jointIxs = NuSerializer.ReadVectorArray<byte>(file);
                List<NuPointOfInterest> pointsOfInterest = NuSerializer.ReadVectorArray<NuPointOfInterest>(file, version);
                List<byte> poiIxs = NuSerializer.ReadVectorArray<byte>(file);

                int buffer_size = file.ReadInt(true); // I think
                if (buffer_size != 0)
                {
                    byte[] buffer = file.ReadArray(buffer_size);
                }

                List<NuLayer_SpecialFlags> layerMetaData = NuSerializer.ReadVectorArray<NuLayer_SpecialFlags>(file, version);
                List<NuLayerData> layers = NuSerializer.ReadVectorArray<NuLayerData>(file, version);

                List<NuShadowData> shadowData = NuSerializer.ReadVectorArray<NuShadowData>(file, version);

                float sphereRadius = file.ReadFloat(true);
                float sphereYOff = file.ReadFloat(true);
                Vector3 min = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                Vector3 max = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
                float cylinderYOff = file.ReadFloat(true);
                float cylinderHeight = file.ReadFloat(true);
                float cylinderRadius = file.ReadFloat(true);
                float lodBoundary = file.ReadFloat(true);

                if (version < 0x10)
                {
                    List<byte> defunctTopLodRemapTable = NuSerializer.ReadVectorArray<byte>(file);
                }

                List<byte> lodRemapTable = NuSerializer.ReadVectorArray<byte>(file);

                if (version < 0x10)
                {
                    float deprecatedModelRenderScale = file.ReadFloat(true);
                }

                List<short> lodSpecialRemapTable = NuSerializer.ReadVectorArray<short>(file);

                byte krawlyLod = file.ReadByte();

                if (version > 0x10)
                {
                    uint jointNameHash = file.ReadUInt(true);
                    byte isReplacementMesh = file.ReadByte();
                }

            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("LOGH");
            schema.HandleUInt(ref Version);

            if (Version < 0xc)
            {
                Debug.Assert(1 == 0, "unsupported HGOL version!");
            }
            else
            {
                schema.HandleSchemaVector(ref JointData, Version);
                schema.HandleSchemaVector(ref T);
                schema.HandleSchemaVector(ref Inv_Wt);
                schema.HandleSerializableVector(ref JointIxs);
                schema.HandleSchemaVector(ref PointsOfInterest, Version);
                schema.HandleSerializableVector(ref PoiIxs);

                schema.HandleBuffer(ref Buffer);

                schema.HandleSchemaVector(ref LayerMetadata, Version);
                schema.HandleSchemaVector(ref Layers, Version);

                schema.HandleSchemaVector(ref ShadowData, Version);

                schema.HandleFloat(ref SphereRadius);
                schema.HandleFloat(ref SphereYOff);
                schema.HandleVector3(ref Min);
                schema.HandleVector3(ref Max);
                schema.HandleFloat(ref CylinderYOff);
                schema.HandleFloat(ref CylinderHeight);
                schema.HandleFloat(ref CylinderRadius);
                schema.HandleFloat(ref LodBoundary);

                if (Version < 0x10)
                {
                    schema.HandleSerializableVector(ref DefunctTopLodRemapTable);
                }

                schema.HandleSerializableVector(ref LodRemapTable);

                if (Version < 0x10)
                {
                    schema.HandleFloat(ref DeprecatedModelRenderScale);
                }

                schema.HandleSerializableVector(ref LodSpecialRemapTable);

                schema.HandleByte(ref KrawlyLod);

                if (Version > 0x10)
                {
                    schema.HandleUInt(ref JointNameHash);
                    schema.HandleByte(ref IsReplacementMesh);
                }
            }
        }
    }
}
