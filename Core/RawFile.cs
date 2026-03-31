using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Diorama.Core
{
    public class RawFile : IDisposable
    {
        public Stream fileStream;

        public object Opaque;

        private RawFile() { }

        public RawFile(Stream stream) { fileStream = stream; }

        public string FileLocation { get; private set; }

        public RawFile(string fileLocation)
        {
            fileStream = File.Open(fileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            FileLocation = fileLocation;
        }

        /// <summary>
        /// Creates a file, meaning that the length is reset to 0.
        /// Ideal when writing to a file, that may be shorter than the original.
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static RawFile Create(string fileLocation)
        {
            RawFile createOnly = new RawFile();
            createOnly.fileStream = File.Create(fileLocation);
            createOnly.FileLocation = fileLocation;
            return createOnly;
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    fileStream?.Flush();
                    fileStream?.Dispose(); // Also flushes
                }
                disposed = true;
            }
        }

        ~RawFile()
        {
            Dispose(false);
        }

        public RawFile CreateView()
        {
            return new RawFile(FileLocation);
        }

        public long Position => fileStream.Position;

        public long Seek(long offset, SeekOrigin origin) => fileStream.Seek(offset, origin);

        private byte[] ReadBlock(int size, bool bigEndian)
        {
            byte[] data = new byte[size];
            fileStream.Read(data, 0, size);
            if (bigEndian)
            {
                Array.Reverse(data);
            }
            return data;
        }

        private void WriteBlock(byte[] block, bool bigEndian)
        {
            if (bigEndian)
            {
                Array.Reverse(block);
            }
            fileStream.Write(block);
        }

        public byte ReadByte()
        {
            return (byte)fileStream.ReadByte();
        }

        public byte[] ReadArray(int size)
        {
            return ReadBlock(size, false);
        }

        public void WriteArray(byte[] toWrite)
        {
            WriteBlock(toWrite, false);
        }

        public void WriteByte(byte value) => fileStream.WriteByte(value);

        public short ReadShort(bool bigEndian = false)
        {
            return BitConverter.ToInt16(ReadBlock(2, bigEndian));
        }

        public void WriteShort(short toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public ushort ReadUShort(bool bigEndian = false)
        {
            return BitConverter.ToUInt16(ReadBlock(2, bigEndian));
        }

        public void WriteUShort(ushort toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public int ReadInt(bool bigEndian = false)
        {
            return BitConverter.ToInt32(ReadBlock(4, bigEndian));
        }

        public void WriteInt(int toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public float ReadFloat(bool bigEndian = false)
        {
            return BitConverter.ToSingle(ReadBlock(4, bigEndian));
        }

        public void WriteFloat(float toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public uint ReadUInt(bool bigEndian = false)
        {
            return BitConverter.ToUInt32(ReadBlock(4, bigEndian));
        }

        public void WriteUInt(uint toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public long ReadLong(bool bigEndian = false)
        {
            return BitConverter.ToInt64(ReadBlock(8, bigEndian));
        }

        public void WriteLong(long toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public ulong ReadULong(bool bigEndian = false)
        {
            return BitConverter.ToUInt64(ReadBlock(8, bigEndian));
        }

        public void WriteULong(ulong toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public Half ReadHalf(bool bigEndian = false)
        {
            return BitConverter.ToHalf(ReadBlock(2, bigEndian));
        }

        public void WriteHalf(Half toWrite, bool bigEndian = false)
        {
            WriteBlock(BitConverter.GetBytes(toWrite), bigEndian);
        }

        public string ReadString(int length)
        {
            if (length == 0) return string.Empty;
            byte[] array = new byte[length];
            fileStream.Read(array, 0, length);
            if (array[array.Length - 1] == 0)
            {
                return Encoding.Default.GetString(array, 0, array.Length - 1);
            }
            return Encoding.Default.GetString(array);
        }

        public void WriteString(string toWrite, int padding = 0)
        {
            byte[] buffer = Encoding.Default.GetBytes(toWrite);
            fileStream.Write(buffer, 0, buffer.Length);
            byte[] pad = new byte[padding];
            fileStream.Write(pad, 0, pad.Length);
        }

        public string ReadNullString()
        {
            string combined = "";
            while (true)
            {
                byte currByte = (byte)fileStream.ReadByte();
                if (currByte == 0) break;

                combined += (char)currByte;
            }

            return combined;
        }



        /// <summary>
        /// Reads a pascal string with a SHORT preceding it
        /// </summary>
        /// <param name="bigEndian"></param>
        /// <param name="security"></param>
        /// <returns></returns>
        /// <exception cref="DataMisalignedException"></exception>
        public string ReadPascalString(bool bigEndian = true, ushort security = 256)
        {
            ushort length = ReadUShort(true);
            if (length > security)
            {
                Console.WriteLine("Attempting to read string of length {0} at position {1}!", length, Position);
                throw new DataMisalignedException("Potential bad string quashed");
            }

            return ReadString(length);
        }

        public string ReadIntPascalString(bool bigEndian = true, uint security = 256)
        {
            uint length = ReadUInt(bigEndian);
            if (length > security)
            {
                Console.WriteLine("Attempting to read string of length {0} at position {1}!", length, Position);
                throw new DataMisalignedException("Potential bad string quashed");
            }
            return ReadString((int)length);
        }

        public void WritePascalString(string toWrite, int padding = 0)
        {
            WriteShort((short)(toWrite.Length + padding), true);
            WriteString(toWrite, padding);
        }

        public void WriteIntPascalString(string toWrite, bool bigEndian = true, int padding = 0)
        {
            WriteInt(toWrite.Length + padding, bigEndian);
            WriteString(toWrite, padding);
        }

        public void ReadInto(byte[] destination, int size)
        {
            fileStream.Read(destination, 0, size);
        }

        private static readonly byte[] ZeroBuffer = new byte[8192]; // 8 KB reusable zero buffer

        public void WritePadding(long count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (count > 0)
            {
                int toWrite = (int)Math.Min(count, ZeroBuffer.Length);
                fileStream.Write(ZeroBuffer, 0, toWrite);
                count -= toWrite;
            }
        }

        public Vector3 ReadVector3(bool bigEndian = false)
        {
            return new Vector3(ReadFloat(bigEndian), ReadFloat(bigEndian), ReadFloat(bigEndian));
        }

        public void WriteVector3(Vector3 toWrite, bool bigEndian = false)
        {
            WriteFloat(toWrite.X, bigEndian);
            WriteFloat(toWrite.Y, bigEndian);
            WriteFloat(toWrite.Z, bigEndian);
        }

        public Vector4 ReadVector4(bool bigEndian = false)
        {
            return new Vector4(ReadFloat(bigEndian), ReadFloat(bigEndian), ReadFloat(bigEndian), ReadFloat(bigEndian));
        }

        public void WriteVector4(Vector4 toWrite, bool bigEndian = false)
        {
            WriteFloat(toWrite.X, bigEndian);
            WriteFloat(toWrite.Y, bigEndian);
            WriteFloat(toWrite.Z, bigEndian);
            WriteFloat(toWrite.W, bigEndian);
        }

        /// <summary>
        /// Create a blank space that will be filled later
        /// </summary>
        /// <returns></returns>
        public long CreateIntMarker()
        {
            WritePadding(4);

            return Position - 4;
        }

        public void FillIntMarker(long markerPosition, bool bigEndian = false)
        {
            long currentPosition = Position;

            long length = currentPosition - markerPosition - 4;

            Seek(markerPosition, SeekOrigin.Begin);

            WriteInt((int)length, bigEndian);

            Seek(currentPosition, SeekOrigin.Begin);
        }
    }

    public class RawFileSection : IDisposable
    {
        public long MarkerPosition;

        public long SectionSize => File.Position - MarkerPosition - 4;

        public RawFile File;

        public bool IncludeMarkerSize = false;

        public bool Endianness;

        public RawFileSection(RawFile file, bool includeMarkerSize = true, bool endianness = false)
        {
            MarkerPosition = file.Position;
            file.WritePadding(4); // Reserve space for length
            File = file;
            IncludeMarkerSize = includeMarkerSize;
            Endianness = endianness;
        }

        public void Dispose()
        {
            long currentPosition = File.Position;
            int length = (int)SectionSize;
            File.Seek(MarkerPosition, SeekOrigin.Begin);
            File.WriteInt(IncludeMarkerSize ? length + 4 : length, Endianness);
            File.Seek(currentPosition, SeekOrigin.Begin);
        }
    }
}
