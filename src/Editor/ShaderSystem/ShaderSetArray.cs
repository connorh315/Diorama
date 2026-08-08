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

        public EditorShaderSetBitmask BitArray;
        public List<uint> Hashes;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleShort(ref FileIndex);
            schema.HandlePascalString(ref MaterialName);

            //schema.Handle(ref BitArray, parentVersion);
            schema.Handle(ref BitArray);

            schema.HandleSchemaVarArray(ref Hashes);
        }

        public uint[] GetSet(int set)
        {
            int populatedSlots = BitArray.PopCount();

            if (populatedSlots == 0)
                return new uint[32];

            int setCount = Hashes.Count / populatedSlots;

            int totalSlots = 32;
            if (BitArray.IsExtended)
                totalSlots = 96;

            uint[] result = new uint[totalSlots];

            if (set < 0 || set >= setCount)
            {
                Console.WriteLine($"Requested non-existent shader set from {MaterialName} - Substituting in 0s");
                return result;
            }

            int hashIndex = 0;
            for (int slot = 0; slot < totalSlots; slot++)
            {
                if (BitArray.GetSlot(slot) == 0)
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
            EditorShaderSetBitmask bitmask = new EditorShaderSetBitmask(hashCount > 32);
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

                bitmask.SetSlotActive(slot);

                // Store one uint for EVERY set
                foreach (var set in hashArrays)
                {
                    if (set != null)
                    {
                        hashes.Add(set[slot]); // May be zero
                    }
                }
            }

            shaderSet.BitArray = bitmask;
            shaderSet.Hashes = hashes;

            return shaderSet;
        }
    }
}
