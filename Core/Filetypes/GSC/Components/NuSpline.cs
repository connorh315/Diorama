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

        public virtual void Deserialize(RawFile file, uint parentVersion)
        {
            Title = file.ReadPascalString(true);
            Path = NuSerializer.ReadVectorArray<Vector3>(file);
        }
    }

    public class NuSpline_50 : NuSpline
    {
        public override void Deserialize(RawFile file, uint parentVersion)
        {
            base.Deserialize(file, parentVersion);
            byte flag_not_sure = file.ReadByte();
        }
    }
}
