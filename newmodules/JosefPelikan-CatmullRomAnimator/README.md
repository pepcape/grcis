# Extension: CatmullRomAnimator

### Author: Josef Pelikan

### Category: Animator

### Namespace: JosefPelikan

### Class name: CatmullRomAnimator : PropertyAnimator

### ITimeDependent: Yes

### Source file: CatmullRomAnimator.cs

This extension implements an "animator" using the famous **Catmull-Rom interpolation
spline**. Interpolation quantity (quantities) go exactly through all key-values.
The set of key-values ("knots") are distributed **evenly** on the time domain.

User is able to define arbitrary set of "properties", each property can have individual
**domain interval**, **dimensionality** (only scalar ``double`` and ``Vector3d`` are implemented)
and **key-sequence** length and logic. Each property has following attributes:

1. **name** - string for property identification. Names must be unique, any object (client) in the
   rest of the ray-tracing scene is accessing properties via their names

2. **domain interval** in seconds - similar to ``Start`` and ``End`` (see ``ITimeDependent``)

3. **period** - you need to define "time period" for some interpolation styles. Applicable
   to ``PropertyAnimator.InterpolationStyle.Cyclic`` and
   ``PropertyAnimator.InterpolationStyle.Pendulum``

4. **interpolation style** - one of the choices from enum type
   ``InterpolationStyle { Once, Cyclic, Pendulum }``

5. **key value vector** ("knot-vector") - the animation data itself. Vector of key-values
   which will be used for interpolation of specific quantity. Cubic spline named "Catmull-Rom"
   is used here (see [Wikipedia](https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline)
   and please ignore stuff like this:
   ``float a = Mathf.Pow((p1.x-p0.x), 2.0f) + Mathf.Pow((p1.y-p0.y), 2.0f);``)

**Key value vector** can have various types. The internal ``Property`` class can use any
object type, API functions are declared for these specific types and arrays:

a. ``float`` and ``double``

b. ``Vector3`` and ``Vector3d``

c. ``Vector4`` and ``Vector4d``

d. ``Quaterniond``

e. ``Matrix4d``

In ``CatmullRomAnimator`` only a. to d. types and arrays are implemented, so in the constructor
you can use knot types:
``List<float>``, ``List<double>``, ``List<Vector3>``, ``List<Vector3d>``, ``List<Vector4>``, ``List<Vector4d>``, ``List<Quaterniond>``,
``List<float[]>``, ``List<double[]>``, ``List<Vector3[]>``, ``List<Vector3d[]>``, ``List<Vector4[]>``, ``List<Vector4d[]>``, ``List<Quaterniond[]>``.

See a textbook (or Wikipedia) to learn about Catmull-Rom interpolation.
Knot vectors (and the curves) can be **open** or **closed**. **Open curve** is defined
by at least four knots (our implementation is robust and can handle even degenerated curves),
**closed curve** can be as short as two knots (but it would have little sense = two identical line
segments). A closed curve is meaningful with three or more knots.

Note that our implementation uses **uniform knot vector** - key times are distributed
over domain interval **uniformly** (and you cannot do anything about it!).

Example of a simple property definition:
```
CatmullRomAnimator cra = new CatmullRomAnimator()
...
cra.newProperty(name, start, end, period,
                PropertyAnimator.InterpolationStyle.Cyclic,
                new List<Vector3d>() {
                  new Vector3d(0.0, 0.2,-0.5),
                  new Vector3d(1.0, 0.2,-0.5),
                  new Vector3d(1.0, 1.6, 0.0),
                  new Vector3d(0.0, 1.2, 0.0)},
                true);

```

## Example

The whole example from the ``AnimatedSceneTranslate.cs`` script:
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
Note that the string ``name`` is used to connect the "client" (animated
scene node, type = ``AnimatedNodeTranslate``) with the "animator".
Later the animator will do the interpolation job and the node will ask
for results at each specific time.

### Sample animation script: AnimatedSceneTranslate.cs (together with ``AnimatedNodeTranslate``)

## Images, videos

Videosequence:

[YouTube video 640x360](https://youtu.be/VaVWIyBfSjM)
