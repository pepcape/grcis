# Extension: StarBackground

![Example](up640.jpg)

### Author: Josef Pelikan

### Category: Background

### Namespace: JosefPelikan

### Class name: StarBackground : DefaultBackground

### ITimeDependent: No

### Source file: StarBackground.cs

This extensions implements an alternative scene background simulation a skydome full of stars.
Deterministic random algorithm places "stars" randomly, star properties which can
vary:

1. brightness (star "size")

2. Color

User is able to define a couple of parameters:

a. **resolution** - controls grid size = number of positions wwhich are considered for stars
   Reasonable values: between 100 and 10000

b. star-field **"density"** in form of probability (greater number means denser field)
   Reasonable values: 0.0001 to 0.5 (for very wierd appearance not similar to night sky)

b. **color range** is number from `0.0` (monochromatic) to `1.0` (full-color)
   Reasonable values: 0.1 to 0.6 (for realistic star colors)

## Example

From a scene/animation definition script
```
using JosefPelikan;

...

scene.BackgroundColor = new double[] {0.0, 0.01, 0.03};
scene.Background = new StarBackground(scene.BackgroundColor, 600, 0.006, 0.5);
```

Here `600` is resolution, `0.006` probability (density) and `0.5` color coefficient.

### Sample scene script: TwoSpheresStars.cs

### Sample animation script: AnimatedSceneStars.cs

## Images, videos

Static image (computed from some variation of `TwoSpheresStars.cs`):

[1800x1200](https://drive.google.com/file/d/1YvHi4glIjDmnjNi6VQaeVubB4hr39SiF/view?usp=sharing)

Videosequence:

[YouTube video 800x600](https://youtu.be/Ekl_EDmuwrY)
