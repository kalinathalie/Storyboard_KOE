using OpenTK;
using OpenTK.Graphics;
using Project.Util;
using Project.Resources;
using StorybrewCommon.Animations;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;
using System.Linq;
using System.Collections.Generic;

namespace StorybrewScripts
{
    public class Kiai : StoryboardObjectGenerator
    {
        [Configurable]
        public float FOV = 60;

        [Configurable]
        public float fps = 60;

        [Configurable]
        public Color4 BGColor = new Color4(20, 20, 20, 255);

        private Timing Timing;

        private SpriteResource SpriteResource;

        private StoryboardLayer layer;

        private float delta;

        private String BlurredSquareOutline;

        private String BlurredSquareFilled;

        private String BlurredBackground;

        public override void Generate()
        {
            Timing = new Timing(Beatmap);
            SpriteResource = new SpriteResource(this);
            delta = 1000.0f / fps;
            layer = GetLayer("");
            BlurredBackground = SpriteResource.blur(Beatmap.BackgroundPath, 30, 15);
            BlurredSquareFilled = SpriteResource.blur(Sprites.SquareFilled, 8, 5);
            BlurredSquareOutline = SpriteResource.blur(Sprites.SquareOutline, 8, 5);
            BG(72892, 92997);
            Grid(72892, 92997);
            //Particles(72892, 92997);
            Overlay(72892, 92997);
            //Ring3D(72892, 92997);
        }

        private void BG(int s, int e)
        {
            var sprite = layer.CreateSprite(BlurredBackground);
            sprite.Fade(s, e, 1, 1);
            sprite.ScaleToFill();
            sprite.Color(Color4.White.Multiply(0.5f));
        }

        private void Grid(int s, int e)
        {
            int count = 16 * 14;
            float scale = 1.3f;
            float rotation = (float) -Math.PI / 6;
            Func<int, Vector2> pattern = (int i) => {
                return Patterns.Grid2D(
                    Vectors.ScreenTopLeft.scaled(scale),
                    Vectors.ScreenBottomRight.scaled(scale),
                    16, 9)(i)
                    .up(100).rotated(rotation).scaled(1.2);
            };
            var sprites = new SpriteSet(
                layer,
                i => BlurredSquareOutline,
                i => OsbOrigin.Centre,
                i => pattern(i)
                , count
            );
            float fade = 0.4f;
            //sprites.Fade(s, s + 200, 0.0f, fade);
            double wavelength = 0.4f;
            double duration = 5000;
            double[] shifts = new double[count];
            for (int i = 0; i < count; i++)
                shifts[i] = 1000 * (sprites.Get(i).InitialPosition - Vectors.Centre.left(Vectors.ScreenSize.X / 2)).Length
                                / Vectors.ScreenSize.X / wavelength;
            for (double t = s + 200; t < e - 200; t += duration)
            {
                int i = 0;
                foreach (var sprite in sprites.Sprites)
                {
                    sprite.Fade(OsbEasing.InOutSine, t + shifts[i], t + duration / 2 + shifts[i], 0.02f, fade);
                    sprite.Fade(OsbEasing.InOutSine, t + duration / 2 + shifts[i], t + duration + shifts[i], fade, 0.02f);
                    i++;
                }
            }
            //sprites.Fade(e - 200, e, fade, 0.0);
            sprites.Scale(.7 * 1 / .7);
            sprites.Rotate(rotation);
            Vector2 d = Vectors.polar(rotation, 50);
            sprites.MoveRelative(OsbEasing.None, s, e, d, -d);

            var flashes = new SpriteSet(
                layer,
                i => BlurredSquareFilled,
                i => OsbOrigin.Centre,
                i => pattern(i),
                count
            );
            float flashFade = 0.7f;
            flashes.MoveRelative(OsbEasing.None, s, e, d, -d);
            flashes.UseScalesOf(sprites);
            flashes.UseRotationsOf(sprites);
            flashes.Fade(s, 0);
            double[] prevTimes = new double[count];
            for (int i = 0; i < count; i++) prevTimes[i] = s;
            for (double t = s; t < e; t += 50)
            {
                while(true)
                {
                    int index = Random(0, count);
                    float flashFade2 = flashFade * sprites.Get(index).OpacityAt(t) / fade;
                    if (prevTimes[index] <= t)
                    {
                        flashes.Fade(OsbEasing.OutExpo, t, t + 100, 0, flashFade2, index, 1);
                        flashes.Fade(OsbEasing.Out, t + 100, t + 2000, flashFade2, 0, index, 1);
                        prevTimes[index] = t + 2000;
                        break;
                    }
                }
            }
            flashes.Additive();
        }

        private void Particles(int s, int e)
        {
            int count = 50;
            float pointScale = 0.2f;
            var pattern = Patterns.PointCloudGlobe(Vectors.Centre.withZ(0), 500, new Random(RandomSeed));
            var points = new List<Vector3>();
            for (int i = 0; i < count; i++) points.Add(pattern(i));
            SpriteSet pointCloud = layer.CreateSpriteSet(
                Sprites.Particle,
                _ => OsbOrigin.Centre,
                i => points[i].project(Vectors.ScreenSize, Vectors.Centre, FOV).screenSpaceVector,
                count
            );
            pointCloud.Fade(s, s + Timing.beat(s) / 2, 0, 0.6);
            pointCloud.Fade(e, 0);
            pointCloud.Scale(s, 0.5);
            var inOutExpo = OsbEasing.InOutExpo.ToEasingFunction();
            for (float t = s; t < e; t += delta)
            {
                float t2 = t + delta < e ? t + delta : e;
                float angle = (float) (-Math.PI / 4 * inOutExpo((t - s) / (e - s)));
                float angle2 = (float) (-Math.PI / 4 * inOutExpo((t2 - s) / (e - s)));
                pointCloud.Move3D(
                    OsbEasing.None,
                    t, t2,
                    i => points[i].rotateY(angle, Vectors.Centre3D),
                    i => points[i].rotateY(angle2, Vectors.Centre3D),
                    FOV,
                    (_sprite, _p1, _p2) =>
                    {
                        float scale1 = pointScale / _p1.normalisedVector.Length;
                        float scale2 = pointScale / _p2.normalisedVector.Length;
                        if (scale1 != scale2) _sprite.Scale(t, t2, scale1, scale2);
                    }
                );
            }
            foreach (var sprite in pointCloud.Sprites)
            {
                SpriteSet trail = Effects.Trail(layer, sprite, s, e, 1000.0 / 60, 0, 0, 150, true);
                trail.UseScaleOf(sprite);
            }
        }

        private void Overlay(int s, int e)
        {
            
        }
            
        private void Ring3D(int s, int e)
        {
            int count = 50;
            float pointScale = 0.2f;
            Vector3 initialRingRotation = new Vector3(
                (float) Math.PI / 4,
                (float) Math.PI / 3,
                0
            );
            Vector3 ringRotationSpeed = new Vector3(
                0.01f,
                0.02f,
                0.03f
            );
            Vector3 ringRotation = Vector3.Zero;
            Func<float, int, Vector3> ringPattern = (float t, int i)
                => Patterns.Circle3D(Vectors.Centre3D, 200, initialRingRotation, 2 * (float) Math.PI / count, t)(i);
            SpriteSet ring = layer.CreateSpriteSet(
                Sprites.Particle,
                _ => OsbOrigin.Centre,
                i => ringPattern(0, i).project(FOV).screenSpaceVector,
                count
            );
            ring.Fade(s, s + Timing.beat(s) / 2, 0, 1.0f);
            ring.Fade(e, 0);
            ring.Scale(s, 0.5);
            float spinSpeed = 2;
            for (float t = s; t < e; t += delta)
            {
                float t2 = t + delta < e ? t + delta : e;
                float spin = spinSpeed * (t - s) / 1000.0f;
                float spin2 = spinSpeed * (t2 - s) / 1000.0f;
                ring.Move3D(
                    OsbEasing.None,
                    t, t2,
                    i => ringPattern(spin, i).rotate3D(ringRotation, Vectors.Centre3D),
                    i => ringPattern(spin2, i).rotate3D(ringRotation + ringRotationSpeed, Vectors.Centre3D),
                    FOV,
                    (_sprite, _p1, _p2) =>
                    {
                        float scale1 = pointScale / _p1.normalisedVector.Length;
                        float scale2 = pointScale / _p2.normalisedVector.Length;
                        if (scale1 != scale2) _sprite.Scale(t, t2, scale1, scale2);
                    }
                );
                ringRotation += ringRotationSpeed;
            }
        }
    }
}
