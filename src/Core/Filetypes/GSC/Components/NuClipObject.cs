using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public struct NuClipItem
    {
        public int OldGeometryIndex;
        public int OldMaterialIndex;

        public short MaterialIndex;
        public short TransformIndex;
        public short LightmapIndex;
        public short TransformIndex2;
        public short MeshIndex;

        public byte LightmapType;
        public byte TransformType;
        public byte GeomType;
        public byte Unused;

        public byte RequiresLightState;

        public byte IsFaceOn;
    }

    public class NuClipObject : ISchemaSerializable
    {
        public NuClipItem[] Elements;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            short elementCount = (short)((schema.Writing) ? Elements.Length : 0);
            schema.HandleShort(ref elementCount);

            if (!schema.Writing)
            {
                Elements = new NuClipItem[elementCount];
            }

            for (int i = 0; i < elementCount; i++)
            {
                if (parentVersion < 0x22)
                {
                    schema.HandleInt(ref Elements[i].OldGeometryIndex);
                    schema.HandleInt(ref Elements[i].OldMaterialIndex);
                }
                else
                {
                    schema.HandleShort(ref Elements[i].MaterialIndex);
                    schema.HandleShort(ref Elements[i].TransformIndex);
                    schema.HandleShort(ref Elements[i].LightmapIndex);
                    schema.HandleShort(ref Elements[i].TransformIndex2);
                    schema.HandleShort(ref Elements[i].MeshIndex);

                    schema.HandleByte(ref Elements[i].LightmapType);
                    schema.HandleByte(ref Elements[i].TransformType);
                    schema.HandleByte(ref Elements[i].GeomType);
                    if (parentVersion > 0x22)
                    {
                        schema.HandleByte(ref Elements[i].Unused);
                    }

                    schema.HandleByte(ref Elements[i].RequiresLightState);

                    schema.HandleByte(ref Elements[i].IsFaceOn);
                }
            }
        }
    }
}
