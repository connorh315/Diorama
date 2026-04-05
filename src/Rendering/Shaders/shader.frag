#version 300 es
precision mediump float;

in vec3 FragPos;
in vec3 Normal;
in vec2 UV;
in vec4 outColor;

out vec4 FragColor;

uniform vec3 camera;
uniform vec3 color;

uniform sampler2D texture0;

void main()
{
    vec3 lightDir = normalize(camera - FragPos);

    float diff = max(dot(normalize(Normal), lightDir), 0.0);

    vec3 baseColor = vec3(outColor); // neutral grey

    vec3 color = baseColor * (0.2 + diff); // ambient + diffuse

    FragColor = texture(texture0, UV);
}