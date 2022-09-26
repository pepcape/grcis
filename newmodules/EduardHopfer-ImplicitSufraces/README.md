# Extension: Implicit surfaces

### Author: Eduard Hopfer

### Category: Solid

### Namespace: EduardHopfer

This extension allows rendering of implicit surfaces represented as **Signed distance functions**.

## Signed distance function
Takes in a point in 3D space and returns the distance to surface it represents. Distance 0 means the point is on the surface of the object,
positive distances correspond to points outside, and negative distances to points inside the object.

Examples of SDFs can be found in `ImplicitCommon.cs`

## Rendering
The SDFs are rendered using **Sphere Tracing**. This is a variation of *ray marching* which has the benefit of never overshooting a surface.
I chose an implementation which detects intersections when the sign of the SDF changes. As I later realized, this has many downsides and
should be changed to the traditional implementation which just checks whether we are close enought to the object.

Possible improvements: [Enhanced sphere tracing](https://erleuchtet.org/~cupe/permanent/enhanced_sphere_tracing.pdf)

## CSG operations
CSG operations work differently when working with SDFs.
- Union: minimum of individual sdf values
- Intersection: maximum of individual sdf values
- Shape inversion: invert the sign of the sdf value
- Difference: intersection of sdf with inverted sdf

Additionally one can use some variation of a [smooth minimum](https://iquilezles.org/articles/smin/) instead of the classic max and min operations to get smooth blending of the shapes.

## Usage
Example scene can be found in `Scene.cs`.

To render SDF you have to use the **SphereTracing** class as the rendering algorithm. It has support for color blending
and removes aliases caused by self intersections.

Using an implicit in your scene is easy, just create a new `DistanceField` with the appropriate SDF and use it like any other solid.
For CSG operations over implicits use `ImplicitInnerNode`. Interop with standard objects is not implemented yet, so its childern must be other implicit inner nodes or distance fiels. One exception is a scene node which has a single child that is an implicit. This is useful
for things like AnimatedCSGInnerNode. 

The implicit inner node implements `ITimeDependent` but just passes the time value down to its children.