using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuIrradiancePlaneLayers : ISchemaSerializable
    {
        public uint Version;
        public NuMtx Transform;
        public NuMtx InvTransform;
        public float DensityX;
        public float DensityZ;
        public int NumSamplesX;
        public int NumSamplesZ;
        public int NumLayers;
        public float DOffset;
        public List<float> Heights;
        public List<NuIrradiancePlaneSample> Samples;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Version);
            schema.Handle(ref Transform);
            schema.Handle(ref InvTransform);
            schema.HandleFloat(ref DensityX);
            schema.HandleFloat(ref DensityZ);
            schema.HandleInt(ref NumSamplesX);
            schema.HandleInt(ref NumSamplesZ);
            schema.HandleInt(ref NumLayers);
            schema.HandleFloat(ref DOffset);
            if (parentVersion < 7)
            {
                schema.HandleLegacyVarArray(ref Heights);
                schema.HandleSchemaVarArray(ref Samples);
            }
            else
            {
                schema.HandleSerializableVector(ref Heights);
                schema.HandleSchemaVector(ref Samples);
            }
        }
    }
}
