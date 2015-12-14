//---------------------------------------------------------------
// Simple vertex shader
//   propagates 2D texture coords, normal vector, world coords
//   @version $Rev$
//---------------------------------------------------------------
#version 330

in vec4 position;
in vec3 normal;
in vec2 texCoords;
in vec3 color;

out vec2 varTexCoords;
out vec3 varNormal;
out vec3 varWorld;
out vec3 varColor;

uniform mat4 matrixModelView;
uniform mat4 matrixProjection;

void main ()
{
  gl_Position = matrixProjection * matrixModelView * position;
  varTexCoords = texCoords;
  varNormal = normal;
  varColor = color;
  varWorld  = position.xyz;
}
