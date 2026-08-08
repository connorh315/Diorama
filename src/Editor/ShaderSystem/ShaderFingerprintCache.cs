using Avalonia.Controls.Shapes;
using Diorama.Core;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Filetypes.GSC.Components;
using Diorama.Core.Filetypes.GSC.Components.RESH;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Diorama.Editor.ShaderSystem
{
    public class ShaderFingerprintCache : ISchemaSerializable
    {
        public uint Version = 1;

        public string ArchivesLocation;

        public NuFileTree FileTree;

        public List<ShaderFingerprintType> fingerprintLayout = new();

        public List<ShaderFingerprint> Cache = new();

        private int shaderArrayOffset = 0;

        public bool ExtendedBitMask = false;

        public List<ShaderSetArray> SetArray = new();

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            schema.HandleUInt(ref Version);

            using (SchemaRegion region = new SchemaRegion(schema))
            {
                schema.HandlePascalString(ref ArchivesLocation);

                schema.Handle(ref FileTree);

                schema.HandleSchemaVarArray(ref fingerprintLayout);

                if (!schema.Writing)
                {
                    foreach (var type in fingerprintLayout)
                    {
                        PropertyInfo property = typeof(EditorMaterial).GetProperty(type.PropertyName);

                        if (property.PropertyType.IsEnum)
                        {
                            Type propertyType = property.PropertyType;

                            type.DefaultEnumValue = Enum.ToObject(propertyType, 0);
                        }
                    }
                }

                schema.SetContext(this);

                schema.HandleSchemaVarArray(ref Cache);
            }

            shaderArrayOffset = (int)schema.File.Position;

            if (schema.Writing)
            {
                schema.HandleBool(ref ExtendedBitMask);
                schema.HandleSchemaVarArray(ref SetArray);
            }
        }

        public void OpenShaderSet(SchemaSerializer schema)
        {
            schema.File.Seek(shaderArrayOffset, SeekOrigin.Begin);
            schema.HandleBool(ref ExtendedBitMask);
            schema.SetContext(this);
            schema.HandleSchemaVarArray(ref SetArray);
        }

        public void CloseShaderSet()
        {
            SetArray = null;
        }

        private bool initialisedLayout = false;
        private EditorMaterial viewModel;
        private PropertyInfo[] props;
        public (ShaderFingerprint fingerprint, ShaderSetArray array) AddMaterial(NuMaterialData material)
        {
            if (!initialisedLayout)
            {
                viewModel = new EditorMaterial()
                {
                    Original = material
                };

                props = EditorShaderSystem.GetProperties(viewModel);

                foreach (var prop in props)
                {
                    fingerprintLayout.Add(new ShaderFingerprintType()
                    {
                        PropertyName = prop.Name,
                        Type = GetFingerprintType(prop.PropertyType)
                    });
                }
                initialisedLayout = true;
            }

            viewModel.Original = material;

            var fingerprint = new ShaderFingerprint(viewModel, props);

            Cache.Add(fingerprint);

            var setArray = ShaderSetArray.FromMaterial(viewModel);

            SetArray.Add(setArray);

            if (setArray.BitArray.IsExtended)
            {
                ExtendedBitMask = true;
            }

            return (fingerprint, setArray);
        }

        public static FingerprintType GetFingerprintType(Type type)
        {
            if (type == typeof(bool))
                return FingerprintType.Bool;

            if (type == typeof(byte))
                return FingerprintType.Byte;

            if (type == typeof(short))
                return FingerprintType.Short;

            if (type == typeof(ushort))
                return FingerprintType.UShort;

            if (type == typeof(int))
                return FingerprintType.Int;

            if (type == typeof(uint))
                return FingerprintType.UInt;

            if (type == typeof(long))
                return FingerprintType.Long;

            if (type == typeof(ulong))
                return FingerprintType.ULong;

            if (type == typeof(float))
                return FingerprintType.Float;

            if (type == typeof(double))
                return FingerprintType.Double;

            if (type == typeof(string))
                return FingerprintType.String;

            if (type.IsEnum)
            {
                return FingerprintType.Enum;
            }

            throw new Exception("Unknown type!");
        }
    }
}
