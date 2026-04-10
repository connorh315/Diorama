using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2(this Vector4 v)
            => new Vector2(v.X, v.Y);

        public static Vector3 ToVector3(this Vector4 v)
            => new Vector3(v.X, v.Y, v.Z);
    }

    public static class NuExtensions
    {
        internal const uint CRC_FNV_OFFSET_32 = 2166136261;
        internal const uint CRC_FNV_PRIME_32 = 0x199933;

        internal const long CRC_FNV_OFFSET_64 = -3750763034362895579;
        internal const long CRC_FNV_PRIME_64 = 1099511628211;

        public static string Standardise(string s)
        {
            if (s[0] == '\\' || s[0] == '/')
            {
                s = s.Substring(1);
            }

            return s.Replace('/', '\\');
        }

        public static string StandardiseUpper(string s) => Standardise(s).ToUpper();

        public static string StandardiseLower(string s) => Standardise(s).ToLower();

        public static uint CalculateCRC32(string path)
        {
            uint crc = CRC_FNV_OFFSET_32;
            foreach (char character in StandardiseUpper(path))
            {
                crc ^= character;
                crc *= CRC_FNV_PRIME_32;
            }

            return crc;
        }

        public static long CalculateCRC64(string path)
        {
            long crc = CRC_FNV_OFFSET_64;
            foreach (char character in path)
            {
                crc ^= character;
                crc *= CRC_FNV_PRIME_64;
            }

            return crc;
        }
    }
}
