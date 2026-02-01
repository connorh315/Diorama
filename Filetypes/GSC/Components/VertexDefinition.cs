using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diorama.Filetypes.GSC.Components
{
    public enum VertexDefinitionVariableEnum
    {
        position,
        normal,
        colorSet0,
        tangent,
        colorSet1,
        uvSet01,
        diffuse,
        uvSet2,
        albedo,
        blendIndices0,
        blendWeight0,
        tangent2,
        lightDirSet,
        lightColSet,
        blendPos2,
        random
    }

    public enum VertexDefinitionStorageEnum
    {
        vec2float = 2,
        vec3float,
        vec4float,
        vec2half,
        vec4half,
        vec4char,
        vec4mini,
        color4char
    }

    public class VertexDefinition
    {
        public VertexDefinitionVariableEnum Variable;

        public VertexDefinitionStorageEnum Type;

        public int Offset;

        public static VertexDefinition Parse(RawFile file)
        {
            VertexDefinition def = new VertexDefinition();
            def.Variable = (VertexDefinitionVariableEnum)file.ReadByte();
            def.Type = (VertexDefinitionStorageEnum)file.ReadByte();
            def.Offset = file.ReadByte();
            return def;
        }
    }
}
