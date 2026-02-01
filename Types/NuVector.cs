using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Types
{
    public class NuVector
    {
        public static List<T> ReadVectorArray<T>(RawFile file)
            where T : new()
        {
            Debug.Assert(file.ReadString(4) == "ROTV");

            int count = file.ReadInt(true);
            var list = new List<T>();

            bool isVectorSerializable =
                typeof(IVectorSerializable).IsAssignableFrom(typeof(T));

            for (int i = 0; i < count; i++)
            {
                if (isVectorSerializable)
                {
                    var obj = new T();
                    ((IVectorSerializable)obj).Deserialize(file);
                    list.Add(obj);
                }
                else
                {
                    list.Add(ReadPrimitive<T>(file));
                }
            }

            return list;
        }

        // TODO: Boxing + unboxing is bad
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

            if (typeof(T) == typeof(Vector3))
            {
                return (T)(object)(new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true)));
            }

            throw new NotSupportedException($"Unsupported vector type: {typeof(T)}");
        }
    }
}
