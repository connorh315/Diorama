using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuSceneInfo : ISchemaSerializable
    {
        public string[] Strings;

        public static NuSceneInfo Read(RawFile file, uint gscVersion)
        {
            Debug.Assert(file.ReadString(4) == "OFNI");

            var info = new NuSceneInfo(); 
            info.Strings = new string[file.ReadInt(true)];
            for (int i = 0; i < info.Strings.Length; i++)
            {
                info.Strings[i] = file.ReadPascalString(true);
            }

            return info;
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.Expect("OFNI");

            schema.HandleArray(ref Strings);
        }

        public void Write(RawFile file, uint gscVersion)
        {
            file.WriteString("OFNI");

            file.WriteInt(Strings.Length, true);
            foreach (var str in Strings)
            {
                file.WritePascalString(str, 1);
            }
        }
    }
}
