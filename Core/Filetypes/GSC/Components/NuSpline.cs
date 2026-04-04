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
        public List<Vector3> Path;
        public byte isPeriodic;
        public byte isBezier;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref Title, 1);
            schema.HandleVector3Vector(ref Path);
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
