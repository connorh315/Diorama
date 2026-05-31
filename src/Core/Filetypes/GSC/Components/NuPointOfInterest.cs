using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuPointOfInterest : IVectorSerializable, ISchemaSerializable
    {
        public string Name;

        public NuMtx Offset;

        public byte ParentJointIdx;

        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0xb)
            {

            }
            else
            {
                string name = file.ReadPascalString();
            }

            NuMtx offset = new NuMtx();
            offset.Deserialize(file, parentVersion);

            byte parentJointIdx = file.ReadByte();
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0xb)
            {
                Debug.Assert(1 == 0, "unnsupported poi version");
            }
            else
            {
                schema.HandlePascalString(ref Name);
                schema.Handle(ref Offset);
                schema.HandleByte(ref ParentJointIdx);
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
