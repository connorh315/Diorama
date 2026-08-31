using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSpline : ISchemaSerializable
    {
        public string Title;
        public uint NameIndex;
        public List<Vector3> Path;
        public List<NuVec> Points;
        public byte isPeriodic;
        public byte isBezier;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0x45)
            {
                schema.HandleUInt(ref NameIndex);
                if (schema.Context is GSerializationContext ctx)
                {
                    ctx.AddReference(this);
                }
            }
            else
            {
                schema.HandlePascalString(ref Title, 1);
            }
            if (parentVersion < 0x45)
            {
                schema.HandleSchemaVarArray(ref Points);
            }
            else
            {
                schema.HandleSchemaVector(ref Points);
            }
            if (parentVersion > 0x4f)
            {
                schema.HandleByte(ref isPeriodic);
                if (parentVersion > 0x57)
                {
                    schema.HandleByte(ref isBezier);
                }
            }
        }


        //public virtual void Deserialize(RawFile file, uint parentVersion)
        //{
        //    Title = file.ReadPascalString(true);
        //    Path = NuSerializer.ReadVectorArray<Vector3>(file);
        //    if (parentVersion > 0x4f)
        //    {
        //        isPeriodic = file.ReadByte();
        //        if (parentVersion > 0x57)
        //        {
        //            isBezier = file.ReadByte();
        //        }
        //    }
        //}


        //public void Serialize(RawFile file, uint parentVersion)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
