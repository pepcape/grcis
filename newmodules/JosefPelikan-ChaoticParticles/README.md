# Extension: ChaoticParticles

### Author: Josef Pelikan

### Category: Solid

### Namespace: JosefPelikan

### Class name: ChaoticParticles : DefaultSceneNode, ISolid, ITimeDependent

### ITimeDependent: Yes

### Source file: ChaoticParticles.cs

This extension implements a new "Solid" -- container for a **set of spherical** (possibly glowing)
**particles** which can be animated using a ``ITimeDependentProperty``.

There are **two optional properties** which can control the set of particles. Two properties
can be defined independently:

1. **posName** (``Vector4d[]`` or ``Vector4[]``) - array of **positions and radii** of individual particles ([X, Y, Z] = position, W = radius)

2. **colName** (``Vector3d[]`` or ``Vector3[]``) - array of **colors** of individual particles ([X, Y, Z] = [R, G, B])

## Examples

Sample animated scene is in the ``ChaoticParticlesScene.cs`` script. It needs two
more modules: ``StarBackground.cs`` and ``CatmullRomAnimator.cs``.

### Sample animation script: ChaoticParticlesScene.cs

## Images, videos

Videosequence:

[YouTube video 640x360](https://youtu.be/6UPf2p8KwdI)
