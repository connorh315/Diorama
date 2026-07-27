using Diorama.Core;
using Diorama.Editor.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public class ShaderFingerprint : ISchemaSerializable
    {
        public object[] Properties;

        public short FileIndex;

        public string MaterialName;

        public ShaderFingerprint()
        {
        }

        public ShaderFingerprint(EditorMaterial material, PropertyInfo[] props)
        {
            MaterialName = material.Name;

            Properties = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                Properties[i] = props[i].GetValue(material);
            }
        }

        public void Handle(SchemaSerializer schema, uint parentVersion = 0)
        {
            schema.HandleShort(ref FileIndex);

            schema.HandlePascalString(ref MaterialName);

            List<ShaderFingerprintType> types = (List<ShaderFingerprintType>)schema.Context;

            if (!schema.Writing)
            {
                Properties = new object[types.Count];
            }

            for (int i = 0; i < types.Count; i++)
            {
                HandleProperty(schema, types[i], ref Properties[i]);
            }
        }

        private void HandleProperty(SchemaSerializer schema, ShaderFingerprintType type, ref object prop)
        {
            switch (type.Type)
            {
                case FingerprintType.Bool:
                    {
                        bool val = ReadWrite<bool>(schema, prop);
                        schema.HandleBool(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.Byte:
                    {
                        byte val = ReadWrite<byte>(schema, prop);
                        schema.HandleByte(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.Short:
                    {
                        short val = ReadWrite<short>(schema, prop);
                        schema.HandleShort(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.UShort:
                    {
                        ushort val = ReadWrite<ushort>(schema, prop);
                        schema.HandleUShort(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.Int:
                    {
                        int val = ReadWrite<int>(schema, prop);
                        schema.HandleInt(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.UInt:
                    {
                        uint val = ReadWrite<uint>(schema, prop);
                        schema.HandleUInt(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.Long:
                    {
                        long val = ReadWrite<long>(schema, prop);
                        schema.HandleLong(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.Float:
                    {
                        float val = ReadWrite<float>(schema, prop);
                        schema.HandleFloat(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.String:
                    {
                        string val = (string)prop;
                        schema.HandlePascalString(ref val);
                        prop = val;
                        break;
                    }
                case FingerprintType.Enum:
                    {
                        int val = Convert.ToInt32(prop);
                        schema.HandleInt(ref val);
                        if (schema.Writing)
                        {
                            prop = Enum.ToObject(prop.GetType(), val);
                        }
                        else
                        {
                            prop = Enum.ToObject(type.DefaultEnumValue.GetType(), val);
                        }
                        break;
                    }
                default:
                    throw new NotSupportedException($"Unsupported fingerprint type: {type}");
            }
        }

        private static T ReadWrite<T>(SchemaSerializer schema, object prop)
        {
            return schema.Writing ? (T)prop : default!;
        }
    }
}
