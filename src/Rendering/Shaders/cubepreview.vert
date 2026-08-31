#version 420 core

layout (location = 0) in vec3 aPosition;

uniform mat4 rotation;
uniform mat4 projection;

out vec3 TexDirection;

void main()
{
    TexDirection = aPosition * mat3(rotation);

    vec4 clipPos =
        vec4(aPosition, 1.0) *
        projection;

    gl_Position = clipPos.xyww;
}