using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuClipItem
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
        public short UnusedLightmapIndex;

        public NuClipItem[] Elements;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0x18)
            {
                schema.HandleShort(ref UnusedLightmapIndex);
            }

            short elementCount = (short)((schema.Writing) ? Elements.Length : 0);
            if (parentVersion < 0x18)
            {
                int extendedElementCount = elementCount;
                schema.HandleInt(ref extendedElementCount);
                elementCount = (short)extendedElementCount;
            }
            else
            {
                schema.HandleShort(ref elementCount);
            }

            if (!schema.Writing)
            {
                Elements = new NuClipItem[elementCount];
            }

            if (parentVersion < 0x18)
            {

                for (int i = 0; i < elementCount; i++)
                {
                    if (!schema.Writing)
                        Elements[i] = new NuClipItem();

                    schema.HandleInt(ref Elements[i].OldMaterialIndex);
                }

                int elementCount2 = schema.Writing ? Elements.Length : elementCount;
                schema.HandleInt(ref elementCount2);
                Debug.Assert(elementCount == elementCount2);

                for (int i = 0; i < elementCount; i++)
                {
                    schema.HandleInt(ref Elements[i].OldGeometryIndex);
                }
            }
            else
            {
                for (int i = 0; i < elementCount; i++)
                {
                    if (!schema.Writing)
                        Elements[i] = new NuClipItem();

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
}
