#version 420 core


in vec3 FragPos;
in vec3 Normal;
in vec4 UV1;
in vec4 outColor;

out vec4 FragColor;

uniform vec3 camera;
uniform vec3 color;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
    vec3 lightDir = normalize(camera - FragPos);

    float diff = max(dot(normalize(Normal), lightDir), 0.0);

    vec3 baseColor = vec3(outColor); // neutral grey

    vec3 color = baseColor * (0.2 + diff); // ambient + diffuse

    vec4 base = texture(texture0, UV1.xy);
    vec4 detail = texture(texture1, UV1.zw);
    // FragColor = base;
    FragColor = base * detail * outColor;
    // FragColor = vec4(1.0);
}