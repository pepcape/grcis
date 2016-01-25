//---------------------------------------------------------------
// Vertex shader for texturing and smooth shading
//   propagates 2D texture coords, normal vector, world coords, color
//   @version $Rev$
//---------------------------------------------------------------
#version 130

in vec3 position;
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
  gl_Position = matrixProjection * matrixModelView * vec4( position, 1.0 );
  varTexCoords = texCoords;
  varNormal = normal;
  varColor = color;
  varWorld  = position.xyz;
}
