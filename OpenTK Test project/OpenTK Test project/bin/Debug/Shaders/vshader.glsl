﻿#version 330
 
in vec4 vPosition;
in vec4 vColor;
uniform mat4 modelview;
uniform mat4 projection;
uniform float angle;

out vec4 color;
 
void main()
{
    gl_Position = projection * modelview * vPosition;
				
    color = vColor;
}