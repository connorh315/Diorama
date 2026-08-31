using Avalonia.Controls.Shapes;
using BrickVault;
using Diorama.Core.Filetypes.GSC;
using Diorama.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Core.Filetypes.GSC.Components
{
    public class NuBlendShape : ISchemaSerializable
    {
        public NuBlendShape Next;

        public uint Id;

        public List<NuVec> Offsets;

        public byte HasAlphas;
        public List<NuBlendShapeAlpha> Alphas;

        public uint CompressionFormat;

        public List<NuBlendRunV1> RunV1;

        public byte[] Buffer;

        public List<uint> RunBatchTableV2;

        public void Handle(SchemaSerializer schema, uint parentVersion)
        {
            GSerializationContext? ctx = schema.Context as GSerializationContext;
            
            ctx?.AddReference(this);

            schema.HandleUInt(ref Id);

            schema.HandleOptional(ref Next, parentVersion);

            if (parentVersion < 0xae)
            {
                schema.HandleSchemaVarArray(ref Offsets);
            }
            else
            {
                schema.HandleSchemaVector(ref Offsets);
            }

            if (parentVersion < 0xae)
            {
                schema.HandleByte(ref HasAlphas);
                schema.HandleSchemaVarArray(ref Alphas);
            }

            schema.HandleUInt(ref CompressionFormat);
            if (parentVersion < 0xae)
            {
                schema.HandleSchemaVarArray(ref RunV1);
            }
            schema.HandleBuffer(ref Buffer);
            if (Buffer.Length != 0)
                ctx?.AddReference(Buffer);

            if (parentVersion < 0xae)
            {
                schema.HandleSchemaVarArray(ref RunBatchTableV2);
            }
            else
            {
                schema.HandleSerializableVector(ref RunBatchTableV2);
            }
        }

        public static NuBlendShape Parse(RawFile file, GSerializationContext ctx, uint parentVersion)
        {
            var shape = new NuBlendShape();

            shape.Id = file.ReadUInt(true);

            ctx.AddReference(shape);

            uint nextShapeExists = file.ReadUInt(true);
            if (nextShapeExists != 0)
            {
                shape.Next = Parse(file, ctx, parentVersion);
            }

            if (parentVersion < 0xae)
            {
                shape.Offsets = NuSerializer.ReadLegacyVarArray<NuVec>(file);
            }
            else
            {
                shape.Offsets = NuSerializer.ReadVectorArray<NuVec>(file);
            }

            if (parentVersion < 0xae)
            {
                Debug.Assert(1 == 0, "NuBlendShape section not implemented");
            }

            shape.CompressionFormat = file.ReadUInt(true);
            int bufferSize = file.ReadInt(true);
            shape.Buffer = file.ReadArray(bufferSize);
            if (bufferSize != 0)
            {
                ctx.AddReference(shape.Buffer);
            }

            shape.RunBatchTableV2 = NuSerializer.ReadVectorArray<uint>(file);

            return shape;
        }

        public void Write(RawFile file, GSerializationContext ctx, uint parentVersion)
        {
            file.WriteUInt(Id, true);

            ctx.AddReference(this);

            if (Next != null)
            {
                file.WriteUInt(1, true);
                Next.Write(file, ctx, parentVersion);
            }
            else
            {
                file.WriteUInt(0, true);
            }

            if (parentVersion < 0xae)
            {
                NuSerializer.WriteLegacyVarArray<NuVec>(file, Offsets);
            }
            else
            {
                NuSerializer.WriteVectorArray<NuVec>(file, Offsets);
            }

            if (parentVersion < 0xae)
            {

            }

            file.WriteUInt(CompressionFormat, true);

            if (Buffer != null)
            {
                file.WriteInt(Buffer.Length, true);
                file.WriteArray(Buffer);
                ctx.AddReference(Buffer);
            }
            else
            {
                file.WriteInt(0);
            }

            NuSerializer.WriteVectorArray<uint>(file, RunBatchTableV2);
        }
    }
}
