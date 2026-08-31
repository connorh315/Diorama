#version 420 core

in vec3 TexDirection;

uniform samplerCube texture0;

out vec4 FragColor;

void main()
{
    FragColor = texture(texture0, normalize(TexDirection));
}