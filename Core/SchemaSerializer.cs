using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    public class SchemaSerializer
    {
        public RawFile File { get; private set; }
        public bool Writing { get; private set; }
        public SchemaSerializer(RawFile file, bool writing)
        {
            File = file;
            Writing = writing;
        }

        public void HandleFloat(ref float v)
        {
            if (Writing)
            {
                File.WriteFloat(v, true);
            }
            else
            {
                v = File.ReadFloat(true);
            }
        }

        public void HandleUInt(ref uint v)
        {
            if (Writing)
            {
                File.WriteUInt(v, true);
            }
            else
            {
                v = File.ReadUInt(true);
            }
        }

        public void HandleInt(ref int v)
        {
            if (Writing)
            {
                File.WriteInt(v, true);
            }
            else
            {
                v = File.ReadInt(true);
            }
        }

        public void HandleUShort(ref ushort v)
        {
            if (Writing)
            {
                File.WriteUShort(v, true);
            }
            else
            {
                v = File.ReadUShort(true);
            }
        }

        public void HandleShort(ref short v)
        {
            if (Writing)
            {
                File.WriteShort(v, true);
            }
            else
            {
                v = File.ReadShort(true);
            }
        }

        public void HandleByte(ref byte v)
        {
            if (Writing)
            {
                File.WriteByte(v);
            }
            else
            {
                v = File.ReadByte();
            }
        }

        public void HandlePascalString(ref string v, int padding = 0)
        {
            if (Writing)
            {
                File.WritePascalString(v, padding);
            }
            else
            {
                v = File.ReadPascalString();
            }
        }

        public void HandleVector3(ref Vector3 v)
        {
            if (Writing)
            {
                File.WriteVector3(v, true);
            }
            else
            {
                v = File.ReadVector3(true);
            }
        }

        public void HandleArray(ref byte[] arr, int size)
        {
            if (Writing)
            {
                File.WriteArray(arr);
            }
            else
            {
                arr = File.ReadArray(size);
            }
        }

        public void HandleSchemaVector<T>(ref List<T> arr, uint parentVersion = 0) where T : ISchemaSerializable, new()
        {
            if (Writing)
            {
                File.WriteString("ROTV");
                File.WriteInt(arr.Count, true);

                foreach (var item in arr)
                {
                    item.Handle(this, parentVersion);
                }
            }
            else
            {
                Debug.Assert(File.ReadString(4) == "ROTV");
                int count = File.ReadInt(true);
                arr = new List<T>();
                for (int i = 0; i < count; i++)
                {
                    var item = new T();
                    item.Handle(this, parentVersion);
                    arr.Add(item);
                }
            }
        }

        public void HandleSchemaVarArray<T>(ref List<T> arr, uint parentVersion = 0) where T : ISchemaSerializable, new()
        {
            if (Writing)
            {
                File.WriteInt(arr.Count, true);
                foreach (var item in arr)
                {
                    item.Handle(this, parentVersion);
                }
            }
            else
            {
                int count = File.ReadInt(true);
                arr = new List<T>();
                for (int i = 0; i < count; i++)
                {
                    var item = new T();
                    item.Handle(this, parentVersion);
                    arr.Add(item);
                }
            }
        }


        // TODO: Remove permanently once all implementations are gone
        public void HandleSerializableVector<T>(ref List<T> arr, uint parentVersion = 0) where T : IVectorSerializable, new()
        {
            if (Writing)
            {
                NuSerializer.WriteVectorArray(File, arr);
            }
            else
            {
                arr = NuSerializer.ReadVectorArray<T>(File);
            }
        }

        // TODO: Remove permanently once all implementations are gone
        public void HandleSerializableLegacyVarArray<T>(ref List<T> arr, uint parentVersion = 0) where T : IVectorSerializable, new()
        {
            if (Writing)
            {
                NuSerializer.WriteLegacyVarArray(File, arr);
            }
            else
            {
                arr = NuSerializer.ReadLegacyVarArray<T>(File);
            }
        }

        // TODO: Remove permanently once all implementations are gone
        public void HandleLegacyVarArray(ref List<byte> arr)
        {
            if (Writing)
            {
                NuSerializer.WriteLegacyVarArray(File, arr);
            }
            else
            {
                arr = NuSerializer.ReadLegacyVarArray<byte>(File);
            }
        }

        // TODO: Remove permanently once all implementations are gone
        public void HandleLegacyVarArray(ref List<ushort> arr)
        {
            if (Writing)
            {
                NuSerializer.WriteLegacyVarArray(File, arr);
            }
            else
            {
                arr = NuSerializer.ReadLegacyVarArray<ushort>(File);
            }
        }

        public void HandleLegacyVarArray(ref List<float> arr)
        {
            if (Writing)
            {
                NuSerializer.WriteLegacyVarArray(File, arr);
            }
            else
            {
                arr = NuSerializer.ReadLegacyVarArray<float>(File);
            }
        }

        public void Expect(string expected)
        {
            if (Writing)
            {
                File.WriteString(expected);
            }
            else
            {
                var actual = File.ReadString(expected.Length);
                Debug.Assert(actual == expected, $"Expected {expected} but got {actual}");
            }
        }
    }
}
