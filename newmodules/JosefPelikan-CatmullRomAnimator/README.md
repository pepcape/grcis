# Extension: CatmullRomAnimator

### Author: Josef Pelikan

### Category: Animator

### Namespace: JosefPelikan

### Class name: CatmullRomAnimator : PropertyAnimator

### ITimeDependent: Yes

### Source file: CatmullRomAnimator.cs

This extensions implements an "animator" using the famous **Catmull-Rom interpolation
spline**. Interpolation quantity (quantities) go through key-values exactly.
The set of key-values ("knots") are distributed **evenly** on the domain interval.

User is able to define arbitrary set of "properties", each property can have individual
**domain interval**, **dimensionality** (only scalar ``double`` and ``Vector3d`` are implemented)
and **key-sequence** length and logic. Each property has following attributes:

1. **name** - string for property identification. Names must be unique, any object (client) in the
   rest of the ray-tracing scene is accessing properties via their names

2. **domain interval** in seconds - similar to ``Start`` and ``End`` (see ``ITimeDependent``)

3. **period** - for some interpolation styles you need to define "time period". Applicable
   to ``PropertyAnimator.InterpolationStyle.Cyclic`` and
   ``PropertyAnimator.InterpolationStyle.Pendulum``

4. **interpolation style** - one of choices from enum type
   ``InterpolationStyle { Once, Cyclic, Pendulum }``

5. **key value vector** ("knot-vector") - the animation data itself. Vector of key-values
   which will be used for interpolation of specific quantity. Cubic spline named "Catmull-Rom"
   is used here (see [Wikipedia](https://en.wikipedia.org/wiki/Centripetal_Catmull%E2%80%93Rom_spline)
   and please ignore stuff like this:
   ``float a = Mathf.Pow((p1.x-p0.x), 2.0f) + Mathf.Pow((p1.y-p0.y), 2.0f);``)

**Key value vector** can have various types. internal ``Property`` class can use any
object type, API functions are declared for three specific types:
a. ``double``
b. ``Vector3d``
c. ``Matrix4d``

In ``CatmullRomAnimator`` only the two formar types are implemented, so in the constructor
you can use ``List<double>`` or ``List<Vector3d>`` as knot vector data.

See a textbook (or Wikipedia) to learn about Catmull-Rom interpolation.
Knot vectors (and the curves) can be **open** or **closed**. Open curve is defined
by at least four knots (our implementation is robust and can handle even degenerated curves),
closed curve can be as short as two knots (but it would have little sense = two identical line
segments). Starting from three knots a closed curve is meaningful.

Note that our implementation uses **uniform knot vector** - key values are distributed
over domain interval **uniformly** (and you cannot do anything about it!).

Example of simple property definition:
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
