using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama
{
    interface IVectorSerializable
    {
        void Deserialize(RawFile file);
    }
}
