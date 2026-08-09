#version 420 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec4 aColor;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec4 aColor2;
layout (location = 5) in vec4 aUv1;
layout (location = 6) in vec4 aDiffuse;
layout (location = 7) in vec4 aUv2;
layout (location = 12) in vec3 lightDirSet;

out vec3 FragPos;
out vec3 Normal;
out vec4 UV1;
out vec3 outTangent;
out vec3 outBitangent;
out vec4 UV2;
out vec4 outColor;
out vec4 outColor2;
out vec4 outDiffuse;
out vec3 outLightDir;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform bool bitangent_flip;

vec3 ResolveNormalised(vec3 normalised)
{
    vec3 resolved = normalised * 2 - 1;
    return normalize(resolved * mat3(model));
}

void main()
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;

    vec4 worldPos = vec4(aPosition, 1.0) * model;
    FragPos = worldPos.xyz;

    Normal = ResolveNormalised(aNormal);
    outTangent = ResolveNormalised(aTangent);
    outTangent = normalize(outTangent - Normal * dot(Normal, outTangent)); // correction for imperfections
    outBitangent = normalize(cross(Normal, outTangent));
    if (bitangent_flip)
        outBitangent = -outBitangent;

    UV1 = aUv1;
    UV2 = aUv2;
    outColor = aColor;
    outColor2 = aColor2;
    outDiffuse = aDiffuse;
    outLightDir = ResolveNormalised(lightDirSet);
}