using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    interface IVectorSerializable
    {
        void Deserialize(RawFile file, uint parentVersion);

        void Serialize(RawFile file, uint parentVersion);

        //void Deserialize(RawFile file, uint parentVersion) => Deserialize(file, 0);
    }
}
