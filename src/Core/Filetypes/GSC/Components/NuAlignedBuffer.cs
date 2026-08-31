using Avalonia.Remote.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuAlignedBuffer : ISchemaSerializable
    {
        public byte[] Buffer;

        public static void HandleSingle(SchemaSerializer schema, ref string output, byte padding = 1)
        {
            if (schema.Writing)
            {
                if (output == null)
                {
                    schema.File.WriteInt(0);
                    return;
                }
                byte[] bytes = new byte[Encoding.UTF8.GetByteCount(output) + padding];
                Encoding.UTF8.GetBytes(output, 0, output.Length, bytes, 0);
                schema.HandleBuffer(ref bytes);
            }
            else
            {
                int length = schema.File.ReadInt(true);
                if (length != 0)
                {
                    output = schema.File.ReadNullString();
                }
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleBuffer(ref Buffer);
        }

        public string GetString(int index)
        {
            int end = index;

            while (end < Buffer.Length && Buffer[end] != 0)
                end++;

            return Encoding.UTF8.GetString(Buffer, index, end - index);
        }

        private int size = 0;
        private List<string> Entries;
        private int Padding = 2;

        public void SetPadding(int padding) => this.Padding = padding;

        public int AddString(string val)
        {
            if (Buffer != null)
                throw new Exception("Not how this function should be used!");

            if (Entries == null)
                Entries = new();

            Entries.Add(val);
            int offset = size;
            size += val.Length + Padding;
            return offset;
        }

        public void Finalise()
        {
            Buffer = new byte[size];
            int offset = 0;
            
            foreach (var entry in Entries)
            {
                FillBuffer(Buffer, offset, entry);
                offset += entry.Length + Padding;
            }

            Entries.Clear();
            Entries = null;
        }

        private void FillBuffer(byte[] buffer, int offset, string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
        }
    }
}
