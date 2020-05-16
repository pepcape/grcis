# Extension: AnimatedNodeTranslate

### Author: Josef Pelikan

### Category: Animated scene component

### Namespace: JosefPelikan

### Class name: AnimatedNodeTranslate : AnimatedCSGInnerNode

### ITimeDependent: Yes

### Source file: AnimatedNodeTranslate.cs

This simple extension implements an "animated scene node" which is able to change
its translation during the time. It is designed to work together with a ``PropertyAnimator``
which helps with translation vector interpolation.

Every time a ``Time`` property of the node is updated, it requests a scene's
current ``Animator`` (``MT.scene.Animator``), looks for the associated animated property
and gets the actual time-interpolated translation vector.

Example of a scene setup (code snippet from the script ``AnimatedSceneTranslate.cs``):
```
using JosefPelikan;

...

string name = "translatePath";

CatmullRomAnimator pa = new CatmullRomAnimator()
{
  Start =   0.0,
  End   =  24.0
};
pa.newProperty(name, 0.0, 24.0, 8.0,
               PropertyAnimator.InterpolationStyle.Cyclic,
               new List<Vector3d>() {
                 new Vector3d(0.0, 0.2,-0.5),
                 new Vector3d(1.0, 0.2,-0.5),
                 new Vector3d(1.0, 1.6, 0.0),
                 new Vector3d(0.0, 1.2, 0.0)},
               true);
scene.Animator = pa;

// And later in the scene definition..

AnimatedNodeTranslate an = new AnimatedNodeTranslate(
  name,
  new Vector3d(0.0, 0.2,-0.5),
  Matrix4d.Identity,
  0.0, 20.0);

```
Note that the string ``name`` is used to connect the node to the animated property living inside
of a PropertyAnimator.

### Sample animation script: AnimatedSceneTranslate.cs (together with ``CatmullRomAnimator``)

## Images, videos

Videosequence:

[YouTube video 640x360](https://youtu.be/VaVWIyBfSjM)
