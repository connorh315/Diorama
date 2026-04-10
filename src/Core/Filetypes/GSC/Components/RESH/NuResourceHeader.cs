using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components.RESH
{
    public class NuResourceHeader : ISchemaSerializable
    {
        private int ResourceHeaderSize;

        private uint Version;

        public NuFileTree FileTree;

        public List<NuResourceReference> References;

        private uint ResourceType;
        private string Stream;
        private long Transaction;
        private string Username;
        private string ProjectName;
        private byte Project;
        private string FileName;
        private byte Discipline;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleInt(ref ResourceHeaderSize);
            schema.Expect(".CC4HSERHSER");
            schema.HandleUInt(ref Version);
            schema.HandleOptional(ref FileTree);

            schema.HandleSchemaVector(ref References, Version);
            if (Version > 1)
            {
                schema.HandleUInt(ref ResourceType);
                schema.HandlePascalString(ref Stream);
                if (Version < 4)
                {
                    Debug.Assert(1 == 0, "unimplemented resource header region");
                }
                else
                {
                    schema.HandleLong(ref Transaction);
                }
                schema.HandlePascalString(ref Username);
                if (Version < 4)
                {
                    schema.HandlePascalString(ref ProjectName);
                }
                else
                {
                    schema.HandleByte(ref Project);
                }
                schema.HandlePascalString(ref FileName);
                if (Version < 4)
                {

                }
                else
                {
                    schema.HandleByte(ref Discipline);
                }
            }
        }
    }
}
