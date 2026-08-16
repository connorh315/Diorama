#version 420 core

uniform vec4 Color;

in vec3 FragPos;
in vec3 normal;

out vec4 FragColor;

uniform vec3 cameraPos;

void main()
{
    vec3 cameraDir = normalize(cameraPos - FragPos);

    vec3 dx = dFdx(FragPos);
    vec3 dy = dFdy(FragPos);
    vec3 faceNormal = normalize(cross(dy, dx));

    float facing = dot(cameraDir, faceNormal);

    FragColor = vec4(Color.rgb * max(facing, 0.2), 1);
}