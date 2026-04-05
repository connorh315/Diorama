using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuAnimSceneBlock : ISchemaSerializable
    {
        public uint Version;
        public List<NuInstAnim> NuInstAnim;
        public List<NuStateAnim> NuStateAnim;
        public List<NuAnimHeader> NuAnimHeader;

        public static NuAnimSceneBlock Parse(RawFile file)
        {
            NuAnimSceneBlock block = new NuAnimSceneBlock();

            Debug.Assert(file.ReadString(4) == "3ALA");
            block.Version = file.ReadUInt(true);
            Debug.Assert(block.Version == 3 || block.Version == 4, "ala3 vesrion!");

            SchemaSerializer temp = new SchemaSerializer(file, false);
            temp.HandleSchemaVarArray(ref block.NuInstAnim, block.Version);

            temp.HandleSchemaVarArray(ref block.NuStateAnim);

            temp.HandleSchemaVarArray(ref block.NuAnimHeader);

            return block;
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("3ALA");
            schema.HandleUInt(ref Version);
            Debug.Assert(Version == 3 || Version == 4, $"Unsupported ala3 version: {Version}");

            schema.HandleSchemaVarArray(ref NuInstAnim, Version);
            schema.HandleSchemaVarArray(ref NuStateAnim);
            schema.HandleSchemaVarArray(ref NuAnimHeader);
        }

        public void Write(RawFile file)
        {
            file.WriteString("3ALA");
            file.WriteUInt(Version, true);

            SchemaSerializer temp = new SchemaSerializer(file, true);
            temp.HandleSchemaVarArray(ref NuInstAnim, Version);

            temp.HandleSchemaVarArray(ref NuStateAnim);

            temp.HandleSchemaVarArray(ref NuAnimHeader);
        }
    }
}
