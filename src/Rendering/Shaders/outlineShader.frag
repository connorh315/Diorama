#version 420 core

in vec2 uv;

uniform sampler2D maskTexture;
uniform vec2 texelSize;
uniform float outlineWidth;

out vec4 FragColor;

void main()
{
    float center = texture(maskTexture, uv).r;

    float expanded = 0.0;

    for (int x = -4; x <= 4; ++x)
    {
        for (int y = -4; y <= 4; ++y)
        {
            vec2 offset =
                vec2(x, y) *
                texelSize *
                outlineWidth;

            expanded = max(
                expanded,
                texture(maskTexture, uv + offset).r);
        }
    }

    // Don't draw over the object itself.
    float outline = expanded * (1.0 - center);

    if (outline <= 0.0)
        discard;

    FragColor = vec4(
        1.0,
        0.35,
        0.0,
        outline);
}