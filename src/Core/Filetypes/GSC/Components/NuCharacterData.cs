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

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("LOGH");
            schema.HandleUInt(ref Version);

            if (Version < 0xc)
            {
                schema.HandleSchemaVarArray(ref JointData, Version);
                schema.HandleSchemaVarArray(ref T);
                schema.HandleSchemaVarArray(ref Inv_Wt);
                schema.HandleLegacyVarArray(ref JointIxs);
                schema.HandleSchemaVarArray(ref PointsOfInterest, Version);
                schema.HandleLegacyVarArray(ref PoiIxs);

                if (Version > 9)
                {
                    schema.HandleBuffer(ref Buffer);
                }

                if (Version > 5)
                {
                    schema.HandleSchemaVarArray(ref LayerMetadata, Version);
                }

                schema.HandleSchemaVarArray(ref Layers, Version);

                if (Version > 2)
                {
                    schema.HandleSchemaVarArray(ref ShadowData, Version);
                }

                schema.HandleFloat(ref SphereRadius);
                schema.HandleFloat(ref SphereYOff);
                schema.HandleVector3(ref Min);
                schema.HandleVector3(ref Max);
                schema.HandleFloat(ref CylinderYOff);
                schema.HandleFloat(ref CylinderHeight);
                schema.HandleFloat(ref CylinderRadius);
                schema.HandleFloat(ref LodBoundary);

                if (Version > 3)
                {
                    schema.HandleLegacyVarArray(ref DefunctTopLodRemapTable);

                    schema.HandleLegacyVarArray(ref LodRemapTable);

                    if (Version > 4)
                    {
                        schema.HandleFloat(ref DeprecatedModelRenderScale);

                        if (Version > 5)
                        {
                            schema.HandleLegacyVarArray(ref LodSpecialRemapTable);

                            if (Version > 8)
                            {
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
