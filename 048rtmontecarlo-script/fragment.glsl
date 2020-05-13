//---------------------------------------------------------------
// Simple fragment shader utilizing single texture
//   normal vector, single point light-source, switchable shading components,
//   flat/smooth shading (Gouraud/Phong)
//   @version $Rev: 384 $
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
uniform bool shadingGouraud;

out vec4 fragColor;

void main ()
{
  if (!shadingPhong && !shadingGouraud)
  {
    if (useTexture)
    {
      fragColor = vec4(texture2D(texSurface, varTexCoords));

      if (fragColor.w < 0.01)
        discard;
    }
    else
      fragColor = vec4(varColor, 1.0);
  }
  else if (shadingPhong)
  {
    // Phong shading (interpolation of normals):
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

    vec4 ka, kd;

    if (useTexture)
      ka = kd = vec4(texture2D(texSurface, varTexCoords));
    else
      if (globalColor)
      {
        ka = vec4(Ka, 1.0);
        kd = vec4(Kd, 1.0);
      }
      else
        ka = kd = vec4(varColor, 1.0);

    fragColor = vec4(0.0, 0.0, 0.0, 1.0);

    if (useAmbient)
      fragColor += ka * vec4(globalAmbient, 1.0);

    if (useDiffuse)
      fragColor += kd * vec4(lightColor, 1.0) * cosa;

    if (useSpecular)
      fragColor += vec4(Ks, 1.0) * vec4(lightColor, 1.0) * cosb;
  }
  else
    if (shadingGouraud)     // Gouraud shading (interpolation of colors):
      fragColor = vec4(varColor, 1.0);
    else                      // flat shading (constant color):
      fragColor = vec4(flatColor, 1.0);
}
