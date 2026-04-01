using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuLSVSample : ISchemaSerializable
    {
        public int LightId1;
        public float ShadowFactor1;
        
        public int LightId2;
        public float ShadowFactor2;

        public Vector3[] Vectors = new Vector3[9];

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref LightId1);
            schema.HandleFloat(ref ShadowFactor1);

            schema.HandleInt(ref LightId2);
            schema.HandleFloat(ref ShadowFactor2);

            for (int i = 0; i < 9; i++)
            {
                schema.HandleVector3(ref Vectors[i]);
            }
        }
    }
}
