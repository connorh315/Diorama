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
    public class NuSpline : IVectorSerializable
    {
        public string Title;
        public List<Vector3> Path;
        public byte flag_not_sure;

        public virtual void Deserialize(RawFile file, uint parentVersion)
        {
            Title = file.ReadPascalString(true);
            Path = NuSerializer.ReadVectorArray<Vector3>(file);
            if (parentVersion > 0x4f)
            {
                flag_not_sure = file.ReadByte();
            }
        }

        public void Serialize(RawFile file, uint parentVersion)
        {
            throw new NotImplementedException();
        }
    }
}
