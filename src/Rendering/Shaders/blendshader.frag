#version 420 core


in vec3 FragPos;
in vec3 Normal;
in vec4 UV1;
in vec4 UV2;
in vec4 outColor;
in vec4 outColor2;
in vec4 outDiffuse;

out vec4 FragColor;

uniform vec3 camera;
uniform vec4 mesh_color;

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform sampler2D texture3;

uniform int diffuse0_uvset;
uniform int diffuse1_uvset;

uniform vec2 lm_offset;
uniform vec2 lm_scale;
uniform int lightmap_uvset;

uniform float lightingEnabled;

uniform int alphaTestMode;
uniform float alphaRef;

uniform bool glow;
uniform float glowIntensity;

uniform bool has_vertex_colors;

uniform float PerLayerUVScale1;
uniform float PerLayerUVScale2;
uniform float PerLayerUVScale3;
uniform float PerLayerUVScale4;

uniform int layer1blendmode;
uniform int layer2blendmode;

vec2 GetUVSet(int uvset)
{
    if (uvset == 0)
        return UV1.xy;

    if (uvset == 1)
        return UV1.zw;

    if (uvset == 2)
        return UV2.xy;

    if (uvset == 3)
        return UV2.zw;

    if (uvset == -1)
        return vec2(0, 0);
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

    vec2 baseuv = GetUVSet(diffuse0_uvset) * PerLayerUVScale1;
    vec4 base = texture(texture0, baseuv);

    switch (alphaTestMode)
    {
        case 2: // LESS
            if (base.a < alphaRef)
                discard;
            break;

        case 5: // GREATEREQUAL
            if (base.a <= alphaRef)
                discard;
            break;

        case 1: // NEVER
            break;

        default:
            break;
    }

    vec2 diffuse1uv = GetUVSet(diffuse1_uvset) * PerLayerUVScale2   ;
    vec4 detail = texture(texture1, diffuse1uv);

    vec2 lmUv = GetUVSet(lightmap_uvset) * lm_scale + lm_offset;
    vec4 ao = texture(texture2, lmUv);
    vec4 smoothLm = texture(texture3, lmUv);

    vec3 albedo = base.rgb;
    switch (layer2blendmode)
    {
        case 1:
            albedo = mix(base.rgb, detail.rgb, outColor2.b);
            break;
        case 5: // MAXALPHA
            if (outColor2.b > base.a)
                albedo = detail.rgb;
            break;
        case 7: // SCALE
            albedo = albedo * detail.rgb;
            break;
    }

    //vec3 albedo = mix(detail.rgb, base.rgb, 1 - outColor2.b);
        
    vec4 color = vec4(albedo, 1) * (has_vertex_colors ? vec4(outColor.b, outColor.g, outColor.r, 1) : vec4(1)) * smoothLm * ao * mesh_color * lighting;

    float glowAmount = glow ? glowIntensity : 0.0;

    vec3 viewDir = normalize(camera - FragPos);
    float rim = pow(1.0 - max(dot(normal, viewDir), 0.0), 3.0);

    color.rgb += mesh_color.rgb * rim * 1.5 * glowAmount;

    color.a = base.a * mesh_color.a;

    //color = vec4(outColor2.b, outColor2.b, outColor2.b, 1);

    FragColor = color;
}