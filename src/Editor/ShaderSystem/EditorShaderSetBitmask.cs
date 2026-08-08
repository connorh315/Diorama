using Diorama.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public struct EditorShaderSetBitmask : ISchemaSerializable
    {
        public uint Low;
        public uint Mid;
        public uint High;

        private bool extended;
        public bool IsExtended => extended;

        public EditorShaderSetBitmask()
        {
            extended = false;

            Low = 0;
            Mid = 0;
            High = 0;
        }

        public EditorShaderSetBitmask(bool extended) : this()
        {
            this.extended = extended;
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Low);
            ShaderFingerprintCache cache = (ShaderFingerprintCache)schema.Context;
            if (cache.ExtendedBitMask)
            {
                extended = true;
                schema.HandleUInt(ref Mid);
                schema.HandleUInt(ref High);
            }
        }

        public int PopCount()
        {
            int count = BitOperations.PopCount(Low);
            if (extended)
            {
                count += BitOperations.PopCount(Mid) + BitOperations.PopCount(High);
            }
            return count;
        }

        public uint GetSlot(int slot)
        {
            if (slot < 32)
                return (Low & (1u << slot));
            if (slot < 64)
                return (Mid & (1u << (slot - 32)));
            if (slot < 96)
                return (High & (1u << (slot - 64)));
            throw new Exception("Invalid slot index.");
        }

        public void SetSlotActive(int slot)
        {
            if (slot < 32)
                Low |= (1u << slot);
            else if (slot < 64)
                Mid |= (1u << (slot - 32));
            else if (slot < 96)
                High |= (1u << (slot - 64));
            else
                throw new Exception("Invalid slot index.");
        }
    }
}
