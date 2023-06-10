#version 450 core
layout (location = 0) out vec4 FragColor;

in float val;
uniform sampler1D color;

void main()
{
	FragColor = texture(color, val);
}