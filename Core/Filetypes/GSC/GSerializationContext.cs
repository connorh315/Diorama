using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC
{
    public class GSerializationContext
    {
        public int ReferenceCounter = 5;
        public Dictionary<object, int> References = new();
        private Dictionary<int, object> ReferenceLookup = new();
        
        public int AddReference(object obj)
        {
            References.Add(obj, ReferenceCounter);
            ReferenceLookup.Add(ReferenceCounter, obj);

            return ReferenceCounter++;
        }

        public object GetObject(int reference)
        {
            return ReferenceLookup[reference];
        }

        public T GetObject<T>(int reference)
        {
            return (T)ReferenceLookup[reference];
        }

        public bool GetOrAddReference(object obj, out int reference)
        {
            if (obj == null)
            {
                reference = 0;
                return false;
            }

            if (References.TryGetValue(obj, out reference)) return true;

            reference = AddReference(obj);
            return false;
        }
    }
}
