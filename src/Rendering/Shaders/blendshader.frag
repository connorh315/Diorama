#version 420 core


in vec3 FragPos;
in vec3 Normal;
in vec4 UV1;
in vec4 UV2;
in vec4 outColor;

out vec4 FragColor;

uniform vec3 camera;
uniform vec4 mesh_color;

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;

uniform vec2 lm_offset;
uniform vec2 lm_scale;
uniform int lightmap_uvset;

uniform float lightingEnabled;

vec2 GetLightmapUV()
{
    if (lightmap_uvset == 0)
        return UV1.xy;

    if (lightmap_uvset == 1)
        return UV1.zw;

    if (lightmap_uvset == 2)
        return UV2.xy;

    if (lightmap_uvset == 3)
        return UV2.zw;
}

void main()
{
    vec3 normal = normalize(Normal);
    vec3 lightDir = normalize(camera - FragPos);

    float diff = max(dot(normal, lightDir), 0.0);

    // Your lighting model
    float lit = 0.2 + diff;

    // Branchless toggle
    float lighting = mix(1.0, lit, lightingEnabled);

    vec4 base = texture(texture0, UV1.xy);
    vec4 detail = texture(texture1, UV1.zw);

    vec2 lmUv = GetLightmapUV() * lm_scale + lm_offset;
    vec4 lm = texture(texture2, lmUv);

    FragColor = base * detail * lm * mesh_color * lighting;
}