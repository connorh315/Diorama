using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Types
{
    public class NuVector
    {
        public static List<T> ReadVectorArray<T>(RawFile file)
        {
            Debug.Assert(file.ReadString(4) == "ROTV");
            int count = file.ReadInt(true);

            List<T> list = new List<T>();

            for (int i = 0; i < count; i++)
            {
                list.Add(ReadPrimitive<T>(file));
            }

            return list;
        }

        static T ReadPrimitive<T>(RawFile file)
        {
            if (typeof(T) == typeof(short))
                return (T)(object)file.ReadShort();

            if (typeof(T) == typeof(ushort))
                return (T)(object)file.ReadUShort();

            if (typeof(T) == typeof(int))
                return (T)(object)file.ReadInt();

            if (typeof(T) == typeof(long))
                return (T)(object)file.ReadLong();

            if (typeof(T) == typeof(float))
                return (T)(object)file.ReadFloat();

            throw new NotSupportedException($"Unsupported vector type: {typeof(T)}");
        }
    }
}
