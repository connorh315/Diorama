using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Types
{
    public class NuSerializer
    {
        public static List<T> ReadVectorArray<T>(RawFile file, uint parentVersion = 0)
            where T : new()
        {
            string marker = file.ReadString(4);
            Debug.Assert(marker == "ROTV" || marker == "\0\0\0");

            int count = file.ReadInt(true);
            var list = new List<T>();

            bool isVectorSerializable =
                typeof(IVectorSerializable).IsAssignableFrom(typeof(T));

            for (int i = 0; i < count; i++)
            {
                if (isVectorSerializable)
                {
                    var obj = new T();
                    ((IVectorSerializable)obj).Deserialize(file, parentVersion);
                    list.Add(obj);
                }
                else
                {
                    list.Add(ReadPrimitive<T>(file));
                }
            }

            return list;
        }

        public static void WriteVectorArray<T>(RawFile file, List<T> items, uint parentVersion = 0)
        {
            if (Settings.ShouldWriteROTV)
            {
                file.WriteString("ROTV");
            }
            else
            {
                file.WriteInt(0);
            }
            file.WriteInt(items.Count, true);

            bool isVectorSerializable =
                typeof(IVectorSerializable).IsAssignableFrom(typeof(T));

            for (int i = 0; i < items.Count; i++)
            {
                if (isVectorSerializable)
                {
                    ((IVectorSerializable)items[i]).Serialize(file, parentVersion);
                }
                else
                {
                    WritePrimitive(file, items[i]);
                }
            }
        }

        public static List<T> ReadLegacyVarArray<T>(RawFile file, uint parentVersion = 0)
            where T : new()
        {
            int count = file.ReadInt(true);
            var list = new List<T>();

            bool isVectorSerializable =
                            typeof(IVectorSerializable).IsAssignableFrom(typeof(T));

            for (int i = 0; i < count; i++)
            {
                if (isVectorSerializable)
                {
                    var obj = new T();
                    ((IVectorSerializable)obj).Deserialize(file, parentVersion);
                    list.Add(obj);
                }
                else
                {
                    list.Add(ReadPrimitive<T>(file));
                }
            }

            return list;
        }

        public static void WriteLegacyVarArray<T>(RawFile file, List<T> items, uint parentVersion = 0)
        {
            file.WriteInt(items.Count, true);

            bool isVectorSerializable =
                            typeof(IVectorSerializable).IsAssignableFrom(typeof(T));

            for (int i = 0; i < items.Count; i++)
            {
                if (isVectorSerializable)
                {
                    ((IVectorSerializable)items[i]).Serialize(file, parentVersion);
                }
                else
                {
                    WritePrimitive(file, items[i]);
                }
            }
        }

        // TODO: Boxing + unboxing is bad
        static T ReadPrimitive<T>(RawFile file)
        {
            if (typeof(T) == typeof(byte))
                return (T)(object)file.ReadByte();

            if (typeof(T) == typeof(short))
                return (T)(object)file.ReadShort();

            if (typeof(T) == typeof(ushort))
                return (T)(object)file.ReadUShort();

            if (typeof(T) == typeof(int))
                return (T)(object)file.ReadInt();

            if (typeof(T) == typeof(uint))
                return (T)(object)file.ReadUInt();

            if (typeof(T) == typeof(long))
                return (T)(object)file.ReadLong();

            if (typeof(T) == typeof(float))
                return (T)(object)file.ReadFloat(true);

            if (typeof(T) == typeof(string))
                return (T)(object)file.ReadPascalString();

            if (typeof(T) == typeof(Vector3))
            {
                return (T)(object)new Vector3(file.ReadFloat(true), file.ReadFloat(true), file.ReadFloat(true));
            }

            throw new NotSupportedException($"Unsupported vector type: {typeof(T)}");
        }

        static void WritePrimitive<T>(RawFile file, T value)
        {
            switch (value)
            {
                case byte v: file.WriteByte(v); break;
                case short v: file.WriteShort(v); break;
                case ushort v: file.WriteUShort(v); break;
                case int v: file.WriteInt(v); break;
                case uint v: file.WriteUInt(v); break;
                case long v: file.WriteLong(v); break;
                case float v: file.WriteFloat(v, true); break;
                case string v: file.WritePascalString(v); break;
                case Vector3 v:
                    file.WriteFloat(v.X, true);
                    file.WriteFloat(v.Y, true);
                    file.WriteFloat(v.Z, true);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported type: {typeof(T)}");
            }
        }
    }
}
