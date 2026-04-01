using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuStateAnim : ISchemaSerializable
    {
        public ushort EndFrame;
        public List<float> Frames;
        public List<byte> States;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUShort(ref EndFrame);
            schema.HandleLegacyVarArray(ref Frames);
            schema.HandleLegacyVarArray(ref States);
        }
    }
}
