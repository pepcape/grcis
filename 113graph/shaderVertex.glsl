//---------------------------------------------------------------
// Simple vertex shader
//   propagates 2D texture coords, normal vector, world coords, color
//   can compute vertex-based shading with configurable components
//   @version $Rev$
//---------------------------------------------------------------
#version 130

in vec4 position;
in vec3 normal;
in vec2 texCoords;
in vec3 color;

out vec2 varTexCoords;
out vec3 varNormal;
out vec3 varWorld;
out vec3 varColor;
flat out vec3 flatColor;

uniform mat4 matrixModelView;
uniform mat4 matrixProjection;

uniform vec3 globalAmbient;
uniform vec3 lightColor;
uniform vec3 lightPosition;
uniform vec3 eyePosition;
uniform vec3 Ka;
uniform vec3 Kd;
uniform vec3 Ks;
uniform float shininess;
uniform bool globalColor;
uniform bool useAmbient;
uniform bool useDiffuse;
uniform bool useSpecular;
uniform bool shadingGouraud;

void main ()
{
  gl_Position = matrixProjection * matrixModelView * position;
  varTexCoords = texCoords;
  varNormal = normal;
  varWorld  = position.xyz;

  // Vertex-based shading.
  vec3 diffuseColor, ambientColor;
  if (globalColor)
  {
    diffuseColor = Kd;
    ambientColor = Ka;
  }
  else
    diffuseColor =
    ambientColor = color;

  if (shadingGouraud)
  {
    // Phong model at the vertex.
    vec3 P = position.xyz;
    vec3 N = normalize(normal);
    vec3 L = normalize(lightPosition - P);
    vec3 V = normalize(eyePosition - P);
    vec3 H = normalize(L + V);

    float cosb = 0.0;
    float cosa = dot(N, L);
    if (cosa > 0.0)
      cosb = pow(max(dot(N, H), 0.0), shininess);
    else
      cosa = 0.0;

    vec3 col = vec3(0.0);
    if (useAmbient)
      col += ambientColor * globalAmbient;
    if (useDiffuse)
      col += diffuseColor * lightColor * cosa;
    if (useSpecular)
      col += Ks * lightColor * cosb;

    varColor = flatColor = col;
  }
  else
    varColor = flatColor = diffuseColor;
}
