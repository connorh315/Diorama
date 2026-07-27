using Diorama.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public class ShaderFingerprintType : ISchemaSerializable
    {
        public string PropertyName;
        private int type;
        public object DefaultEnumValue;

        public FingerprintType Type
        {
            get => (FingerprintType)type;
            set
            {
                if (type == (int)value)
                    return;

                type = (int)value;
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandlePascalString(ref PropertyName);
            schema.HandleInt(ref type);
        }
    }

    public enum FingerprintType
    {
        Bool = 1,
        Byte = 2,
        Short = 3,
        UShort = 4,
        Int = 5,
        UInt = 6,
        Long = 8,
        ULong = 9,
        Float = 10,
        Double = 11,
        String = 12,
        Enum = 13
    }
}
