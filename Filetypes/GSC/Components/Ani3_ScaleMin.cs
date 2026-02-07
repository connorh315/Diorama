using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public class Ani3_ScaleMin : IVectorSerializable
    {
        public float Scale { get; set; }
        public float Min { get; set; }

        public void Deserialize(RawFile file)
        {
            Scale = file.ReadFloat(true);
            Min = file.ReadFloat(true);
        }
    }
}
