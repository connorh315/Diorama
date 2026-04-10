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
        public object Context { get; private set; }
        public void SetContext(object context)
        {
            Context = context;
        }
        public SchemaSerializer(RawFile file, bool writing)
        {
            File = file;
            Writing = writing;
        }

        public void HandleLong(ref long v)
        {
            if (Writing)
            {
                File.WriteLong(v, true);
            }
            else
            {
                v = File.ReadLong(true);
            }
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

        public void HandlePascalString(ref string v, int padding = 1)
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

        public void HandleIntPascalString(ref string v, int padding = 0, uint security = 256)
        {
            if (Writing)
            {
                File.WriteIntPascalString(v, padding: padding);
            }
            else
            {
                v = File.ReadIntPascalString(security: security);
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

        public void HandleBuffer(ref byte[] arr)
        {
            if (Writing)
            {
                File.WriteInt(arr.Length, true);
                File.WriteArray(arr);
            }
            else
            {
                int size = File.ReadInt(true);
                arr = File.ReadArray(size);
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

        public void HandleArray(ref string[] strings)
        {
            if (Writing)
            {
                File.WriteInt(strings.Length, true);
                for (int i = 0; i < strings.Length; i++)
                {
                    File.WritePascalString(strings[i], 1);
                }
            }
            else
            {
                int count = File.ReadInt(true);
                strings = new string[count];
                for (int i = 0; i < count; i++)
                {
                    strings[i] = File.ReadPascalString();
                }
            }
        }

        public void HandleSchemaVector<T>(ref List<T> arr, uint parentVersion = 0) where T : ISchemaSerializable, new()
        {
            if (Writing)
            {
                if (Settings.ShouldWriteROTV)
                {
                    File.WriteString("ROTV");
                }
                else
                {
                    File.WriteInt(0);
                }

                File.WriteInt(arr.Count, true);

                foreach (var item in arr)
                {
                    item.Handle(this, parentVersion);
                }
            }
            else
            {
                string magic = File.ReadString(4);
                Debug.Assert(magic == "ROTV" || magic == "\0\0\0");
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

        public void HandleSchemaVarArray(ref List<uint> arr, uint parentVersion = 0)
        {
            if (Writing)
            {
                File.WriteInt(arr.Count, true);
                foreach (var item in arr)
                {
                    File.WriteUInt(item, true);
                }
            }
            else
            {
                int count = File.ReadInt(true);
                arr = new List<uint>();
                for (int i = 0; i < count; i++)
                {
                    var item = File.ReadUInt(true);
                    arr.Add(item);
                }
            }
        }

        public void HandleVector3Vector(ref List<Vector3> arr, uint parentVersion = 0)
        {
            if (Writing)
            {
                if (Settings.ShouldWriteROTV)
                {
                    File.WriteString("ROTV");
                }
                else
                {
                    File.WriteInt(0);
                }
                File.WriteInt(arr.Count, true);

                foreach (var item in arr)
                {
                    File.WriteVector3(item, true);
                }
            }
            else
            {
                string magic = File.ReadString(4);
                Debug.Assert(magic == "ROTV" || magic == "\0\0\0");
                int count = File.ReadInt(true);
                arr = new List<Vector3>();
                for (int i = 0; i < count; i++)
                {
                    arr.Add(File.ReadVector3(true));
                }
            }
        }

        // TODO: Remove permanently once all implementations are gone
        public void HandleSerializableVector<T>(ref List<T> arr, uint parentVersion = 0) where T : IVectorSerializable, new()
        {
            if (Writing)
            {
                NuSerializer.WriteVectorArray(File, arr, parentVersion);
            }
            else
            {
                arr = NuSerializer.ReadVectorArray<T>(File, parentVersion);
            }
        }

        public void HandleSerializableVector(ref List<ushort> arr, uint parentVersion = 0)
        {
            if (Writing)
            {
                NuSerializer.WriteVectorArray(File, arr, parentVersion);
            }
            else
            {
                arr = NuSerializer.ReadVectorArray<ushort>(File, parentVersion);
            }
        }

        public void HandleSerializableVector(ref List<uint> arr, uint parentVersion = 0)
        {
            if (Writing)
            {
                NuSerializer.WriteVectorArray(File, arr, parentVersion);
            }
            else
            {
                arr = NuSerializer.ReadVectorArray<uint>(File, parentVersion);
            }
        }

        // TODO: Remove permanently once all implementations are gone
        public void HandleSerializableLegacyVarArray<T>(ref List<T> arr, uint parentVersion = 0) where T : IVectorSerializable, new()
        {
            if (Writing)
            {
                NuSerializer.WriteLegacyVarArray(File, arr, parentVersion);
            }
            else
            {
                arr = NuSerializer.ReadLegacyVarArray<T>(File, parentVersion);
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

        public void HandleOptional<T>(ref T optional, uint parentVersion = 0) where T : ISchemaSerializable, new()
        {
            if (Writing)
            {
                if (optional == null)
                {
                    File.WriteInt(0);
                    return;
                }

                File.WriteInt(1, true);
                optional.Handle(this, parentVersion);
            }
            else
            {
                int exists = 0;
                HandleInt(ref exists);
                if (exists == 1)
                {
                    optional = new T();
                    optional.Handle(this, parentVersion);
                }
            }
        }

        public void Handle<T>(ref T value, uint parentVersion = 0) where T : ISchemaSerializable, new()
        {
            if (Writing)
            {
                value.Handle(this, parentVersion);
            }
            else
            {
                value = new T();
                value.Handle(this, parentVersion);
            }
        }
    }
}
