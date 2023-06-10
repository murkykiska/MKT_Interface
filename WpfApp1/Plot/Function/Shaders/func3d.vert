#version 450 core

layout (location = 0) in vec2 aPosition;
layout (location = 1) in mat4 position;
layout (location = 2) in float value;

out float val;
uniform mat4 projection;
uniform mat4 model;

void main()
{
   val = value;
   gl_Position = projection * model * position * vec4(aPosition, 0, 1.0);
}
