#version 300 es
precision mediump float;

in vec3 Normal;
in vec3 FragPos;

out vec4 FragColor;

uniform vec3 camera;

void main()
{
    vec3 lightDir = normalize(camera - FragPos);

    float diff = max(dot(normalize(Normal), lightDir), 0.0);

    vec3 baseColor = vec3(0.7); // neutral grey

    vec3 color = baseColor * (0.2 + diff); // ambient + diffuse

    FragColor = vec4(color, 1.0);
}