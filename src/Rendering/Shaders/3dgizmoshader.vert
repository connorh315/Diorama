#version 420 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;

out vec3 FragPos;
out vec3 normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    FragPos = (vec4(aPosition, 1.0) * model).xyz;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    normal = (aNormal * mat3(model)).xyz;
}