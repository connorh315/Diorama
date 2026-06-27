#version 460 core

in vec2 TexCoord;

uniform sampler2D texture0;

out vec4 FragColor;

void main() 
{
	FragColor = texture(texture0, TexCoord);
}