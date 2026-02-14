#version 300 es
layout (location = 0) in vec3 aPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}