#version 300 es
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec4 aUv;
layout (location = 3) in vec4 aColor;

out vec3 FragPos;
out vec3 Normal;
out vec2 UV;
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
    UV = aUv.xy;
    outColor = aColor;
}