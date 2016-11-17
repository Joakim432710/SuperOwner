#version 400
layout (location = 0) in vec3 vertex_position;
layout (location = 1) in vec4 vertex_color;
layout (location = 2) in vec2 vertex_texcoord;

uniform mat4 mvp_matrix;

out vec4 color;
out vec2 texCoord;

void main(void)
{
	color = vertex_color;
	texCoord = vertex_texcoord;
	gl_Position = mvp_matrix * vec4(vertex_position, 1.0f);
}