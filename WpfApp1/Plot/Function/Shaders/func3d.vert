#version 450 core

layout (location = 0) in vec2 aPosition;
layout (location = 1) in float value;
layout (location = 2) in mat4 position;

out float val;
uniform mat4 projection;
uniform mat4 model;

void main()
{
   val = value;
   mat4 a = projection * model * position;
   gl_Position =  a * vec4(aPosition, 0, 1.0);
   //gl_Position =  vec4(aPosition, 0, 1.0);
}
