using Diorama.Core;
using Diorama.Core.Filetypes.GSC.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public class ShaderSetArray : ISchemaSerializable
    {
        public short FileIndex = -1;
        public string MaterialName;

        public uint BitArray;
        public List<uint> Hashes;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleShort(ref FileIndex);
            schema.HandlePascalString(ref MaterialName);

            schema.HandleUInt(ref BitArray);
            schema.HandleSchemaVarArray(ref Hashes);
        }

        public uint[] GetSet(int set)
        {
            int populatedSlots = BitOperations.PopCount(BitArray);

            if (populatedSlots == 0)
                return new uint[32];

            int setCount = Hashes.Count / populatedSlots;

            if (set < 0 || set >= setCount)
                throw new ArgumentOutOfRangeException(nameof(set));

            uint[] result = new uint[32];

            int hashIndex = 0;
            for (int slot = 0; slot < 32; slot++)
            {
                if ((BitArray & (1u << slot)) == 0)
                    continue;

                result[slot] = Hashes[hashIndex + set];
                hashIndex += setCount;
            }

            return result;
        }

        public static ShaderSetArray FromMaterial(EditorMaterial material)
        {
            List<uint[]> shaderSets = new();

            for (int i = 0; i < EditorMaterial.MaxShaderSet; i++)
            {
                shaderSets.Add(material.GetShaderSet(i));
            }

            return FromHashArrays(shaderSets, material.Name);
        }

        public static ShaderSetArray FromHashArrays(List<uint[]> hashArrays, string materialName)
        {
            ShaderSetArray shaderSet = new ShaderSetArray();
            shaderSet.MaterialName = materialName;

            int hashCount = hashArrays[0].Length;

            uint bitArray = 0;
            List<uint> hashes = new List<uint>();

            for (int slot = 0; slot < hashCount; slot++)
            {
                bool required = false;

                foreach (var set in hashArrays)
                {
                    if (set != null && set[slot] != 0)
                    {
                        required = true;
                        break;
                    }
                }

                if (!required)
                    continue;

                bitArray |= 1u << slot;

                // Store one uint for EVERY set
                foreach (var set in hashArrays)
                {
                    if (set != null)
                    {
                        hashes.Add(set[slot]); // May be zero
                    }
                }
            }

            shaderSet.BitArray = bitArray;
            shaderSet.Hashes = hashes;

            return shaderSet;
        }
    }
}
