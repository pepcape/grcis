//---------------------------------------------------------------
// Simple vertex shader
//   propagates 2D texture coords, normal vector, world coords
//   @version $Rev$
//---------------------------------------------------------------

attribute vec4 position;
attribute vec3 normal;
attribute vec2 texCoords;

varying vec2 varTexCoords;
varying vec3 varNormal;
varying vec3 varWorld;

uniform mat4 matrixModelView;
uniform mat4 matrixProjection;

void main ()
{
  gl_Position = matrixProjection * matrixModelView * position;
  varTexCoords = texCoords;
  varNormal = normal;
  varWorld  = position.xyz;
}
