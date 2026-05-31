using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuJointData : IVectorSerializable, ISchemaSerializable
    {
        public void Deserialize(RawFile file, uint parentVersion)
        {
            if (parentVersion < 0xd)
            {
                uint nameIndex = file.ReadUInt(true);
            }
            else
            {
                string name = file.ReadPascalString();
            }

            NuMtx orient = new NuMtx();
            orient.Deserialize(file, parentVersion);
            Vector3 locatorOffset = new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));

            byte parentIndex = file.ReadByte();
            byte flags = file.ReadByte();
        }

        public uint NameIndex;
        public string Name;

        public NuMtx Orient;

        public Vector3 LocatorOffset;

        public byte ParentIndex;
        public byte Flags;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            if (parentVersion < 0xd)
            {
                schema.HandleUInt(ref NameIndex);
            }
            else
            {
                schema.HandlePascalString(ref Name);
            }

            schema.Handle(ref Orient);

            schema.HandleVector3(ref LocatorOffset);

            schema.HandleByte(ref ParentIndex);
            schema.HandleByte(ref Flags);
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
