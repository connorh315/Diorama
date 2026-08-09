#version 420 core


in vec3 FragPos;
in vec3 Normal;
in vec4 UV1;
in vec3 outTangent;
in vec3 outBitangent;
in vec4 UV2;
in vec4 outColor;
in vec4 outColor2;
in vec4 outDiffuse;
in vec3 outLightDir;

out vec4 FragColor;

uniform vec3 camera;
uniform vec4 mesh_color;

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform sampler2D texture2;
uniform sampler2D texture3;

uniform sampler2D normal0;
uniform bool hasNormalMap;
uniform int normal0_uvset;
uniform float normalStrength;

uniform sampler2D specular0;
uniform bool hasSpecularMap;
uniform int specular0_uvset;

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

uniform int numAlphaLayers;

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

uniform vec3 BoundsCenter;
uniform float BoundsRadius;
uniform bool RenderSpheres;

uniform bool debug_color0;
uniform bool debug_color1;

uniform bool debug_color0r;
uniform bool debug_color0g;
uniform bool debug_color0b;
uniform bool debug_color0a;

uniform bool debug_color1r;
uniform bool debug_color1g;
uniform bool debug_color1b;
uniform bool debug_color1a;

uniform bool debug_showSpecular;

void main()
{
    vec4 surfaceSample = vec4(0.5, 0.5, 0.0, 1.0);
    vec4 specularSample = vec4(1.0);
    
    vec3 normal = normalize(Normal);
    vec3 lightDir = normalize(camera - FragPos);

    if (hasNormalMap)
    {
        surfaceSample = texture(normal0, GetUVSet(normal0_uvset) * PerLayerUVScale1);

        vec3 tangentNormal = surfaceSample.agb;

        // Decode from [0,1] -> [-1,1]
        //tangentNormal = tangentNormal * 2.0 - 1.0;

        tangentNormal = tangentNormal * 2.0 - vec3(1.0, 1.0, 0.0);
        tangentNormal.xy *= normalStrength;
        tangentNormal = normalize(tangentNormal);

        vec3 T = normalize(outTangent);
        vec3 B = normalize(outBitangent);
        vec3 N = normalize(Normal);

        //mat3 TBN = mat3(
        //    normalize(outTangent),
        //    normalize(outBitangent),
        //    normalize(Normal));

        //normal = normalize(TBN * tangentNormal);

        normal = normalize(
            T * tangentNormal.x +
            B * tangentNormal.y +
            N * tangentNormal.z);
    }

    if (hasSpecularMap)
    {
        specularSample = texture(
            specular0,
            GetUVSet(specular0_uvset) * PerLayerUVScale1
        );
    }

    float NdotL = max(dot(normal, lightDir), 0.0);

    //const float ambientStrength = 0.35;
    const float directStrength = 0.65;

    float hemi = normal.y * 0.5 + 0.5;

    float ambientStrength = mix(0.20, 0.40, hemi);

    float lit = ambientStrength + directStrength * NdotL;

    //float diff = max(dot(normal, lightDir), 0.0);

    // Your lighting model
    //float lit = 0.2 + diff;

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

    float directionalFactor = clamp(
        dot(normalize(outLightDir), normal),
        0.0,
        1.0
    );

    vec2 lmUv = GetUVSet(lightmap_uvset) * lm_scale + lm_offset;

    vec3 lm0 = texture(texture2, lmUv).rgb;
    vec3 lm1 = texture(texture3, lmUv).rgb;

    lm0 *= lm0;
    lm1 *= lm1;

    vec3 bakedLighting = mix(lm1, lm0, directionalFactor);

    vec3 albedo = base.rgb;
    vec4 layerWeights = outColor2;
    if (numAlphaLayers == 0)
        layerWeights = vec4(1.0, detail.a, base.a, 1.0);
    switch (layer2blendmode)
    {
        case 1:
            if (numAlphaLayers > 0)
            {
                float outAlpha = layerWeights.b + layerWeights.g * (1.0 - layerWeights.b);
                albedo = (detail.rgb * layerWeights.b + base.rgb * layerWeights.g * (1.0 - layerWeights.b)) / outAlpha;
            }
            else
            {
                albedo = mix(base.rgb, detail.rgb, detail.a);
            }
            break;
        case 2: // ADD
            albedo = base.rgb + detail.rgb;
            break;
        case 3: // SUBTRACT
            albedo = albedo - detail.rgb;
            break;
        case 4: // MULTIPLY
            albedo = albedo * detail.rgb;
            break;
        case 5: // MAXALPHA
            if ((detail.a * layerWeights.b) > (base.a * layerWeights.g))
                albedo = detail.rgb;
            break;
        case 10: // MAXALPHABLEND (Not correct)
            albedo = mix(base.rgb, detail.rgb, layerWeights.b);
            break;
        case 7: // SCALE
            albedo = albedo * detail.rgb;
            break;
    }

    //vec3 albedo = mix(detail.rgb, base.rgb, 1 - outColor2.b);
    
    vec3 F0 = mix(
        albedo * albedo,
        vec3(0.04),
        specularSample.g
    );

    vec3 materialDiffuseSquared = albedo * albedo;
    vec3 bakedMaterialResponse;

    float specularStrength = 1.0;

    if (hasSpecularMap)
    {
        bakedMaterialResponse = specularSample.g * materialDiffuseSquared;

        if (specularSample.g < 0.2)
        {
            bakedMaterialResponse += F0 * 0.2;
        }

        bakedMaterialResponse *= specularSample.a;
    }
    else
    {
        bakedMaterialResponse = materialDiffuseSquared * specularStrength;
    }

    

    vec3 vertexColor = has_vertex_colors ? outColor.bgr : vec3(1.0);

    bakedMaterialResponse *= vertexColor;
    bakedMaterialResponse *= mesh_color.rgb;

    //vec4 color = vec4(albedo, 1) * (has_vertex_colors ? vec4(outColor.b, outColor.g, outColor.r, 1) : vec4(1)) * mesh_color * lighting * vec4(bakedLighting, 1);

    vec3 finalDiffuse = bakedMaterialResponse * bakedLighting;

    vec4 color = vec4(finalDiffuse, base.a * mesh_color.a);

    vec3 viewDir = normalize(camera - FragPos);

    vec3 L = lightDir;
    vec3 V = viewDir;

    vec3 H = normalize(L + V);

    float NdotV = max(dot(normal, V), 0.0);
    float NdotH = max(dot(normal, H), 0.0);
    float VdotH = max(dot(V, H), 0.0);

    float baseRoughness = 0.5; // possibly pulled from the ColourX
    float surfaceStrength = 1.0;
    float roughnessBias = -0.00392;
    float roughness;
    if (hasSpecularMap)
    {
        roughness = clamp(specularSample.r + roughnessBias, 0.0, 1.0);
    }
    else
    {
        roughness = clamp(surfaceSample.r * surfaceStrength + roughnessBias, 0.0, 1.0);
    }

    roughness = max(roughness, 0.01);

    float roughness2 = roughness * roughness;
    float roughness4 = roughness2 * roughness2;

    float distributionDenom =
        NdotH * NdotH * (roughness4 - 1.0) + 1.0;

    distributionDenom *= distributionDenom;

    float rPlusOne = roughness + 1.0;
    float k = (rPlusOne * rPlusOne) * 0.125;

    float geometryV =
        NdotV * (1.0 - k) + k;

    float geometryL =
        NdotL * (1.0 - k) + k;

    float geometryDenom =
        geometryV * geometryL;

    float brdfDenom =
        distributionDenom *
        geometryDenom *
        12.566371;

    float Fc = pow(1.0 - VdotH, 5.0);

    vec3 F;

    if (hasSpecularMap)
    {
        float fresnelFactor = pow(1.0 - VdotH, 5.0);

        F = F0 + (vec3(1.0) - F0) * fresnelFactor;
    }
    else
    {
        float fresnelFactor = pow(1.0 - VdotH, 5.0);

        F = vec3(0.05 + 0.95 * fresnelFactor);
    }

    vec3 specularBRDF =
        (roughness4 * F) /
        max(brdfDenom, 0.0001);

    specularBRDF *= specularSample.a;

    vec3 specular = specularBRDF * NdotL;

    specular = clamp(
        specular,
        vec3(0.0),
        vec3(1.0)
    );

    if (debug_showSpecular)
        color.rgb += specular;

    float glowAmount = glow ? glowIntensity : 0.0;

    float rim = pow(1.0 - max(dot(normal, viewDir), 0.0), 3.0);

    color.rgb += mesh_color.rgb * rim * 1.5 * glowAmount;

    color.a = base.a * mesh_color.a;

    FragColor = color;

    //FragColor = vec4(vec3(bakedLighting), 1);

    //FragColor = vec4(texture(texture3, lmUv).rgb, 1.0);

    //FragColor = vec4(vec3(roughness), 1);

    //FragColor = vec4(vec3(specularSample.b), 1.0);

    //FragColor = vec4(textureLod(normal0, (GetUVSet(normal0_uvset) * PerLayerUVScale1), 0).agb, 1);

    //FragColor = vec4(texture(texture2, lmUv));

    if (debug_color0)
        FragColor = outColor.bgra;

    if (debug_color0r)
        FragColor = vec4(outColor.r);

    if (debug_color0g)
        FragColor = vec4(outColor.g);

    if (debug_color0b)
        FragColor = vec4(outColor.b);

    if (debug_color0a)
        FragColor = vec4(outColor.a);

    if (debug_color1)
        FragColor = outColor2;

    if (debug_color1r)
        FragColor = vec4(outColor2.r);

    if (debug_color1g)
        FragColor = vec4(outColor2.g);

    if (debug_color1b)
        FragColor = vec4(outColor2.b);

    if (debug_color1a)
        FragColor = vec4(outColor2.a);
}