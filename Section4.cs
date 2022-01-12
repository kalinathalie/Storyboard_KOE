using OpenTK;
using OpenTK.Graphics; using Project.Util;
using Project.Resources;
using StorybrewCommon.Animations;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class Section4 : StoryboardObjectGenerator
    {
        StoryboardLayer layer;

        StoryboardLayer maskLayer;

        StoryboardLayer maskLayer2;

        Timing Timing;

        [Configurable]
        public float FOV = 60;

        [Configurable]
        public float fps = 60;

        private float delta;

        public override void Generate()
        {
            Timing = new Timing(Beatmap);
            delta = 1000.0f / fps;

            Section4_1();
            //Section4_2();
        }

        void Section4_1()
        {
            // circle
            int s = 62840;
            int e = 63468;
            float fade = 1;
            layer = GetLayer("");
		    maskLayer = GetLayer("masks");
		    maskLayer2 = GetLayer("masks2");
            var downwardsMovement = new Movement(OsbEasing.OutExpo, s, e, Vectors.Centre + Vectors.up(100), Vectors.Centre);

            var ring1 = maskLayer2.CreateSprite(Sprites.Circle, OsbOrigin.Centre, downwardsMovement.StartPosition);
            var ring1Mask = maskLayer2.CreateSprite(Sprites.Circle, OsbOrigin.Centre, downwardsMovement.StartPosition);

            int circleWidth = GetMapsetBitmap(Sprites.Circle).Width;
            float width = 350.0f;
            float width2 = 250.0f;
            float scale = width / circleWidth;
            float thickness = 10;
            float lineThickness = thickness * 0.71f;
            float maskScale = (width - thickness * 2) / circleWidth / scale;

            ring1.Fade(s, e, fade, fade);
            ring1.Scale(OsbEasing.OutExpo, s, e - Timing.beat(s), scale, scale / width * width2);
            ring1.Move(downwardsMovement);

            ring1Mask.Fade(s, e, 1, 1);
            ring1Mask.Color(s, Color4.Black); ring1Mask.Scale(OsbEasing.OutExpo, s, e - Timing.beat(s), scale * maskScale, scale * maskScale / width * width2);
            ring1Mask.Move(downwardsMovement);

            // 3d ring
            float startOfRotation = e - 1.25f * (float) Timing.beat(s);
            var inOutExpo = OsbEasing.InOutExpo.ToEasingFunction();
            var outExpo = OsbEasing.OutExpo.ToEasingFunction();
            Func<float, float, Vector3> moving3DCircle = (float t, float time) =>
            {
                float tilt = time < startOfRotation ? 0 : (float) inOutExpo((time - startOfRotation) / (e - startOfRotation));
                Vector3 centre = downwardsMovement.PositionAtTime(time).withZ(0);
                float radius = (MathHelper.Clamp(
                    InterpolatingFunctions.Float(width, width2, outExpo((time - s) / (e - Timing.beat(s) - s))),
                    width2, width
                ) - thickness) / 2;

                Vector3 rot1 = new Vector3(
                    5 * (float) Math.PI / 16,
                    (float) Math.PI / 2,
                    0
                );

                Vector3 rot2 = new Vector3(
                    5 * (float) Math.PI / 16,
                    (float) Math.PI / 4,
                    0
                );

                Vector3 rot = Vector3.Lerp(rot1, rot2, tilt);

                var res = radius * new Vector3(
                    (float) Math.Cos(-t),
                    (float) Math.Sin(-t),
                    0
                ).rotateX(rot.X).rotateY(rot.Y).rotateZ(rot.Z) + centre;
                return res;
            };

            int count = 200;
            float dotScale = lineThickness / circleWidth;
            for (int i = 0; i < count; i++)
            {
                var sprite = layer.CreateSprite(Sprites.Circle, OsbOrigin.Centre);
                sprite.Fade(s, e, 1, 1);
                for (float t = s; t < e; t += delta)
                {
                    float t2 = t + delta < e ? t + delta : e;

                    Vector3 p1 = moving3DCircle(
                        i * 2 * (float) Math.PI / count,
                        t
                    );

                    Vector3 p2 = moving3DCircle(
                        i * 2 * (float) Math.PI / count,
                        t2
                    );

                    sprite.Move3D(
                        OsbEasing.None,
                        t, t2,
                        p1, p2,
                        FOV,
                        (_sprite, _p1, _p2) =>
                        {
                            float scale1 = dotScale / _p1.normalisedVector.Length;
                            float scale2 = dotScale / _p2.normalisedVector.Length;
                            if (scale1 != scale2) _sprite.Scale(t, t2, scale1, scale2);
                        }
                    );
                }
            }

            // dotted line through the sphere
            count = 35;
            float opacity = 1;
            float lineAppearStart = s + (float) Timing.beat(s) / 3;
            float appearDelay = (float) Timing.beat(s) / 2 / count;
            float length = width2 + 2 * 70;
            float dottedLineThickness = 1.2f;

            SpriteSet axisDottedLine = maskLayer.CreateSpriteSet(
                Sprites.Pixel,
                _ => OsbOrigin.TopCentre,
                Patterns.Line2D(Vectors.Centre + Vectors.up(length / 2), Vectors.Centre + Vectors.down(length / 2), count),
                count
            );

            var dotMovement = new Movement(OsbEasing.OutQuint, s + Timing.beat(s) / 3, e, Vectors.Centre + Vectors.up(100), Vectors.Centre);
            axisDottedLine.MoveRelative(dotMovement - Vectors.Centre);
            axisDottedLine.ScaleVec((int) lineAppearStart, new Vector2(dottedLineThickness, length / count / 2));

            axisDottedLine.Fade(
                OsbEasing.None,
                i => lineAppearStart + i * appearDelay,
                i => lineAppearStart + i * appearDelay + 100,
                0, opacity
            );
            axisDottedLine.Fade(e, 0);

            SpriteSet gradientMask = maskLayer.CreateSpriteSet(Sprites.TransparentGradient, _ => OsbOrigin.CentreRight, 2);
            gradientMask.Rotate(lineAppearStart, i => Math.PI / 2 + i * Math.PI);
            float gradientWidth = GetMapsetBitmap(Sprites.TransparentGradient).Width;
            gradientMask.ScaleVec(lineAppearStart, new Vector2(length / 2 / gradientWidth, dottedLineThickness));
            gradientMask.AddSprites(
                Sprites.Pixel,
                i => (i % 2 == 0 ? OsbOrigin.BottomCentre : OsbOrigin.TopCentre),
                i => Vectors.Centre + (i % 2 == 0 ? 1 : -1) * Vectors.up(length / 2),
                2
            );
            float gradientExtension = Vectors.Centre.Y - length / 2;
            gradientMask.ScaleVec(lineAppearStart, new Vector2(dottedLineThickness, gradientExtension), 2);
            gradientMask.Color(lineAppearStart, Color4.Black);
            gradientMask.Fade(lineAppearStart, e, 1.0, 1.0);

            SpriteSet cover = maskLayer.CreateSpriteSet(Sprites.Pixel, 2);
            cover.Move(i => downwardsMovement.ResampledSection(lineAppearStart, e, delta)
                + Vectors.up((width2 - lineThickness) / 2) * (i % 2 == 0 ? 1 : -1));
            cover.Fade(lineAppearStart, e, 1.0, 1.0);
            cover.ScaleVec(lineAppearStart, new Vector2(dottedLineThickness, lineThickness));

            // point cloud
            count = 50;
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
            pointCloud.Fade(startOfRotation, startOfRotation + Timing.beat(s) / 2, 0, 0.6);
            pointCloud.Fade(e, 0);
            pointCloud.Scale(startOfRotation, 5);
            for (float t = startOfRotation; t < e; t += delta)
            {
                float t2 = t + delta < e ? t + delta : e;
                float angle = (float) (-Math.PI / 4 * inOutExpo((t - startOfRotation) / (e - startOfRotation)));
                float angle2 = (float) (-Math.PI / 4 * inOutExpo((t2 - startOfRotation) / (e - startOfRotation)));
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
                SpriteSet trail = Effects.Trail(layer, sprite, startOfRotation, e, 1000.0 / 60, 0, 0, 150, true);
                trail.UseScaleOf(sprite);
            }
        }

        void Section4_2()
        {
            // triangle pattern
            int s = 63468;
            int e = 63939;
            float patternRadius = 300;
            float scale = 0.6f;
            float relativeRingScale = 0.5f;
            Vector2 patternPosition = Vectors.Centre + Vectors.polar(-Math.PI / 6, patternRadius);

            SpriteSet outer = MakeOuterRing(s, e, patternPosition, patternRadius, scale);
            SpriteSet inner = MakeInnerRing(s, e, patternPosition, patternRadius * relativeRingScale, scale);
            
            SpriteSet extension = layer.CreateSpriteSet();
            for (int i = 0; i < 6; i++)
            {
                var ring = Patterns.Circle2D(
                    patternPosition,
                    patternRadius * 3,
                    (float) -Math.PI / 3,
                    5 * (float) Math.PI / 6
                );
                extension += MakeOuterRing(s, e, ring(i), patternRadius, scale);
            }
            for (int i = 0; i < 6; i++)
            {
                var ring = Patterns.Circle2D(
                    patternPosition,
                    patternRadius * (float) Math.Sqrt(3),
                    (float) -Math.PI / 3,
                    4 * (float) Math.PI / 6
                );
                extension += MakeInnerRing(s, e, ring(i), patternRadius * relativeRingScale, scale);
            }

            SpriteSet trianglePattern = inner + outer + extension;

            float zoomScale = 0.9f;
            float cutScale = 0.9f;
            //trianglePattern.MoveRelative(s, Vector2.Zero);
            trianglePattern.Scale(OsbEasing.None, s, s + 3 * Timing.beat(s) / 2, cutScale, cutScale * zoomScale, Vectors.Centre);
            //trianglePattern.MoveRelative(OsbEasing.None, s, s + 3 * Timing.beat(s) / 2, Vector2.Zero, Vectors.down(patternRadius));
        }

        SpriteSet MakeOuterRing(int s, int e, Vector2 position, float patternRadius, float scale)
        {
            var bitmap = GetMapsetBitmap(Sprites.Triangle);
            float triangleWidth = bitmap.Width;

            var hexagon = Patterns.Circle2D(position, patternRadius, (float) -Math.PI / 3, 5 * (float) Math.PI / 6);
            SpriteSet triangles = layer.CreateSpriteSet(Sprites.Triangle, _ => OsbOrigin.Centre, hexagon, 6);
            triangles.Fade(s, e, 1.0, 1.0);
            triangles.Rotate(i => -Math.PI / 3 + i * Math.PI / 3);
            triangles.Scale(patternRadius / triangleWidth * scale);
            return triangles;
        }

        SpriteSet MakeInnerRing(int s, int e, Vector2 position, float patternRadius, float scale)
        {
            var bitmap = GetMapsetBitmap(Sprites.Triangle);
            var bitmap2 = GetMapsetBitmap(Sprites.Circle);
            float triangleWidth = bitmap.Width;
            float circleWidth = bitmap2.Width;

            var hexagon2 = Patterns.Circle2D(position, patternRadius, (float) -Math.PI / 3, 5 * (float) Math.PI / 6);
            SpriteSet circles = layer.CreateSpriteSet(Sprites.Circle, _ => OsbOrigin.Centre, hexagon2, 6);
            circles.Fade(s, e, 1.0, 1.0);
            circles.Scale(patternRadius / circleWidth * scale * 0.8f);

            float dist = 0.36f;
            Func<int, Vector2> hexagon3 = (int i) => Vector2.Lerp(hexagon2(i), hexagon2(i + 1), i < 6 ? dist : 1 - dist);
            SpriteSet smallTriangles = layer.CreateSpriteSet(Sprites.Triangle, _ => OsbOrigin.Centre, hexagon3, 12);
            smallTriangles.Fade(s, e, 1.0, 1.0);
            smallTriangles.Scale(patternRadius / triangleWidth * scale * 0.3f);
            smallTriangles.Rotate(i => hexagon3(i).rotationTo(hexagon3(i + 6)), 0, 6);
            smallTriangles.Rotate(i => hexagon3(i).rotationTo(hexagon3(i - 6)), 6, 6);

            return circles + smallTriangles;
        }
    }
}
