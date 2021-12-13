//---------------------------------------------------------------
// Simple fragment shader utilizing single texture
//   normal vector, single point light-source, configurable shading components,
//   flat/smooth shading (Gouraud/Phong)
//   @version $Rev$
//---------------------------------------------------------------
#version 130

in vec2 varTexCoords;
in vec3 varNormal;
in vec3 varWorld;
in vec3 varColor;
flat in vec3 flatColor;

uniform vec3 globalAmbient;
uniform vec3 lightColor;
uniform vec3 lightPosition;
uniform vec3 eyePosition;
uniform vec3 Ka;
uniform vec3 Kd;
uniform vec3 Ks;
uniform float shininess;
uniform bool useTexture;
uniform bool globalColor;
uniform sampler2D texSurface;
uniform bool useAmbient;
uniform bool useDiffuse;
uniform bool useSpecular;
uniform bool shadingPhong;
uniform bool shadingSmooth;

out vec3 fragColor;

void main ()
{
  vec3 diffuseColor, ambientColor;
  if (globalColor)
  {
    diffuseColor = Kd;
    ambientColor = Ka;
  }
  else
    diffuseColor =
    ambientColor =
      useTexture ? vec3(texture2D(texSurface, varTexCoords)) : varColor;

  if (shadingPhong)           // Phong shading (interpolation of normals from vertices)
  {
    // Phong shading (interpolation of normals).
    vec3 P = varWorld;
    vec3 N = normalize(varNormal);
    vec3 L = normalize(lightPosition - P);
    vec3 V = normalize(eyePosition - P);
    vec3 H = normalize(L + V);

    float cosb = 0.0;
    float cosa = dot(N, L);
    if (cosa > 0.0)
      cosb = pow(max(dot(N, H), 0.0), shininess);
    else
      cosa = 0.0;

    fragColor = vec3(0.0);
    if (useAmbient)
      fragColor += ambientColor * globalAmbient;
    if (useDiffuse)
      fragColor += diffuseColor * lightColor * cosa;
    if (useSpecular)
      fragColor += Ks * lightColor * cosb;
  }
  else
    if (shadingSmooth)        // Gouraud shading (interpolation of colors from vertices)
      fragColor = diffuseColor;
    else                      // flat shading (constant color)
      fragColor = flatColor;
}
