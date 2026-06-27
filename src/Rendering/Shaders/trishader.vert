#version 460 core

layout(location = 0) in vec2 inPosition;
layout(location = 1) in vec2 inTexCoord;

out vec2 TexCoord;

void main()
{
	TexCoord = inTexCoord;
	gl_Position = vec4(inPosition, 0.0, 1.0);
}