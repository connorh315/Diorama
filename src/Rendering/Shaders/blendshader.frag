#version 420 core


in vec3 FragPos;
in vec3 Normal;
in vec3 outTangent;
in vec3 outBitangent;
in vec4 outColor;
in vec4 outColor2;
in vec4 outDiffuse;
in vec3 outLightDir;
in vec2 uvSets[4];

out vec4 FragColor;

uniform float time;

uniform vec3 camera;

uniform vec4 diffuse0_color;
uniform vec4 diffuse1_color;
uniform vec4 diffuse2_color;

uniform sampler2D diffuse0tex;
uniform sampler2D diffuse1tex;
uniform sampler2D diffuse2tex;
uniform sampler2D diffuse3tex;

uniform sampler2D texture2;
uniform sampler2D texture3;

uniform sampler2D normal0;
uniform bool hasNormal0Map;
uniform int normal0_uvset;
uniform float normalStrength;

uniform sampler2D specular0;
uniform bool hasSpecularMap;
uniform int specular0_uvset;
uniform vec4 specular0_specular;

uniform bool use_scene_envmap;
uniform samplerCube scene_envmap_tex;

uniform bool use_custom_envmap;
uniform samplerCube custom_envmap_tex;

uniform int diffuse0_uvset;
uniform int diffuse1_uvset;
uniform int diffuse2_uvset;

uniform vec2 lm_offset;
uniform vec2 lm_scale;
uniform int lightmap_uvset;

uniform bool lightingEnabled;

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
uniform int layer3blendmode;

uniform vec2 layer0texanim;
uniform vec2 layer1texanim;
uniform vec2 layer2texanim;
uniform vec2 layer3texanim;

uniform int lightingmodel;

vec2 GetUVSet(int uvset)
{
    if (uvset == -1)
        return vec2(0, 0);

    return uvSets[uvset];
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
uniform bool debug_showEnvMap;

vec4 CompositeOver(vec4 bottom, vec4 top)
{
    float outAlpha =
        top.a + bottom.a * (1.0 - top.a);

    if (outAlpha <= 0.00001)
        return vec4(0.0);

    vec3 outColor =
        (top.rgb * top.a +
         bottom.rgb * bottom.a * (1.0 - top.a))
        / outAlpha;

    return vec4(outColor, outAlpha);
}

vec4 CompositeLayer(
    vec4 accumulated,
    vec4 layer,
    float layerWeight,
    int blendMode)
{
    switch (blendMode)
    {
        case 1: // OVER
        {
            vec4 source = vec4(layer.rgb, layer.a * layerWeight);
            return CompositeOver(accumulated, source);
        }

        case 2: // ADD
        {
            float alpha = layer.a * layerWeight;

            accumulated.rgb += layer.rgb * alpha;
            accumulated.a = max(accumulated.a, alpha);

            return accumulated;
        }

        case 3: // SUBTRACT
        {
            accumulated.rgb -= layer.rgb * layerWeight;
            accumulated.a = layer.a;
            return accumulated;
        }

        case 4: // MULTIPLY
        {
            // Weight determines how strongly the multiply is applied.
            accumulated.rgb *= mix(
                vec3(1.0),
                layer.rgb,
                layerWeight
            );

            return accumulated;
        }

        case 5: // MAXALPHA
        {
            if (layerWeight > accumulated.a)
                return vec4(layer.rgb, layerWeight);

            return accumulated;
        }

        case 7: // SCALE
        {
            accumulated.rgb *= layer.rgb;
            return accumulated;
        }

        case 10: // MAXALPHABLEND - TODO: verify
        {
            accumulated.rgb = mix(
                accumulated.rgb,
                layer.rgb,
                layerWeight
            );

            accumulated.a = max(
                accumulated.a,
                layerWeight
            );

            return accumulated;
        }

        case 11:
        {
            accumulated.rgb *= vec3(layer.r, 1, 1);
            return accumulated;
        }

        case 13:
        {
            accumulated.rgb *= vec3(1, 1, layer.b);
            return accumulated;
        }

        case 25:
        {
            accumulated = vec4(layer.g, layer.g, layer.g, layer.a);
            return accumulated;
        }

        default:
            return accumulated;
    }
}

void main()
{
    vec4 surfaceSample = vec4(0.5, 0.5, 0.0, 1.0);
    vec4 specularSample = vec4(1.0);
    
    vec3 normal = normalize(Normal);
    vec3 lightDir = normalize(camera - FragPos);

    if (hasNormal0Map)
    {
        vec2 normal0uv = GetUVSet(normal0_uvset) * PerLayerUVScale1;
        normal0uv += layer0texanim;
        surfaceSample = texture(normal0, normal0uv);

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

    vec2 diffuse0uv = GetUVSet(diffuse0_uvset) * PerLayerUVScale1;
    vec2 diffuse1uv = GetUVSet(diffuse1_uvset) * PerLayerUVScale2;
    vec2 diffuse2uv = GetUVSet(diffuse2_uvset) * PerLayerUVScale3;

    diffuse0uv += layer0texanim;
    diffuse1uv += layer1texanim;
    diffuse2uv += layer2texanim;

    vec4 diffuse0 = texture(diffuse0tex, diffuse0uv) * diffuse0_color;
    vec4 diffuse1 = texture(diffuse1tex, diffuse1uv) * diffuse1_color;
    vec4 diffuse2 = texture(diffuse2tex, diffuse2uv) * diffuse2_color;

    switch (alphaTestMode)
    {
        case 2: // LESS
            if (diffuse0.a < alphaRef)
                discard;
            break;

        case 5: // GREATEREQUAL
            if (diffuse0.a <= alphaRef)
                discard;
            break;

        case 1: // NEVER
            break;

        default:
            break;
    }

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

    vec3 layerWeights = numAlphaLayers > 0
        ? outColor2.bgr
        : vec3(1.0);
    
    vec4 composite = vec4(0.0);

    composite = CompositeLayer(
        composite,
        diffuse0,
        1.0,
        layer1blendmode
    );

    // Layer 0 starts the accumulator.
    //vec4 composite = vec4(
    //    diffuse0.rgb,
    //    diffuse0.a
    //);

    // Layer 1 over layer 0.
    composite = CompositeLayer(
        composite,
        diffuse1,
        layerWeights.x,
        layer2blendmode
    );

    // Layer 2 over the result of 0 + 1.
    composite = CompositeLayer(
        composite,
        diffuse2,
        layerWeights.y,
        layer3blendmode
    );

    vec3 albedo = composite.rgb;
    float albedoAlpha = composite.a;

    vec3 vertexColor = has_vertex_colors ? outColor.bgr : vec3(1.0);

    if (!lightingEnabled || lightingmodel == 0)
    {
        vec3 unlitColor = albedo * vertexColor;

        FragColor = vec4(unlitColor, albedoAlpha);

        return;
    }
    
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

    bakedMaterialResponse *= vertexColor;

    //vec4 color = vec4(albedo, 1) * (has_vertex_colors ? vec4(outColor.b, outColor.g, outColor.r, 1) : vec4(1)) * lighting * vec4(bakedLighting, 1);

    vec3 finalDiffuse = bakedMaterialResponse * bakedLighting * lit;

    vec4 color = vec4(finalDiffuse, albedoAlpha);

    vec3 viewDir = normalize(camera - FragPos);

    vec3 L = lightDir;
    vec3 V = viewDir;

    vec3 H = normalize(L + V);

    float NdotV = max(dot(normal, V), 0.0);
    float NdotH = max(dot(normal, H), 0.0);
    float VdotH = max(dot(V, H), 0.0);

    float baseRoughness = 0.5; // possibly pulled from the ColourX
    float surfaceStrength = 1.0;
    float roughnessBias = specular0_specular.y;
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

    vec3 environment = vec3(1.0);
    if ((use_scene_envmap || use_custom_envmap) && debug_showEnvMap)
    {
        vec3 reflectionDir = reflect(-viewDir, normal);
        if (use_scene_envmap) 
        {
            environment = texture(scene_envmap_tex, reflectionDir).rgb;
        }
        else
        {
            environment = texture(custom_envmap_tex, reflectionDir).rgb;
        }

        float envFresnel =
            pow(1.0 - NdotV, 5.0);

        vec3 envF =
            F0 + (vec3(1.0) - F0) * envFresnel;

        vec3 environmentSpecular =
            environment * envF;


        color.rgb += environmentSpecular;
    }

    float glowAmount = glow ? glowIntensity : 0.0;

    float rim = pow(
        1.0 - max(dot(normal, viewDir), 0.0),
        3.0
    );

    color.rgb += rim * 1.5 * glowAmount;

    color.a = albedoAlpha;

    FragColor = color;

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