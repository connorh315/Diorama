#version 420 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec4 aColor;
layout (location = 5) in vec4 aUv1;

out vec3 FragPos;
out vec3 Normal;
out vec4 UV1;
out vec4 outColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    
    vec4 worldPos = model * vec4(aPosition, 1.0);

    FragPos = worldPos.xyz;
    Normal = mat3(transpose(inverse(model))) * aNormal;
    UV1 = aUv1;
    outColor = aColor;
}