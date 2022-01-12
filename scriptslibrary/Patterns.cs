using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Curves;
using StorybrewCommon.Animations;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Util
{
    // Functions that return functions, meant for population of values
    static class Patterns
    {
        public static Func<int, Vector3> PointCloudGlobe(Vector3 centre, float radius, Random rng)
        {
            return (int i) =>
            {
                double phi = 2 * Math.PI * rng.NextDouble();
                double costheta = 2 * rng.NextDouble() - 1;
                double u = rng.NextDouble();
                double theta = Math.Acos(costheta);
                float r = radius * (float) Math.Pow(u, 1.0 / 3);
                return r * new Vector3(
                    (float) (Math.Sin(theta) * Math.Cos(phi)),
                    (float) (Math.Sin(theta) * Math.Sin(phi)),
                    (float) (Math.Cos(theta))
                ) + centre;
            };
        }

        public static Func<int, Vector3> PointCloudSphere(Vector3 centre, float radius, Random rng)
        {
            return (int i) =>
            {
                // random point on a cylinder
                double z = 2 * radius * rng.NextDouble() - radius;
                double angle = 2 * Math.PI * rng.NextDouble();
                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);

                // projected onto a sphere
                double r = Math.Sqrt(radius * radius - z * z);
                x *= r;
                y *= r;

                return new Vector3((float) x, (float) y, (float) z);
            };
        }

        public static Func<int, Vector3> Circle3D(Vector3 centre, float radius, Vector3 rotation, float delta, float offset)
        {
            return (int i) =>
            {
                float t = i * delta + offset;
                return radius * new Vector3(
                    (float) Math.Cos(-t),
                    (float) Math.Sin(-t),
                    0
                ).rotateX(rotation.X).rotateY(rotation.Y).rotateZ(rotation.Z) + centre;
            };
        }

        public static Func<int, Vector2> Circle2D(Vector2 centre, float radius, float delta, float offset)
        {
            return (int i) =>
            {
                float t = i * delta + offset;
                return radius * new Vector2(
                    (float) Math.Cos(-t),
                    (float) Math.Sin(-t)
                ) + centre;
            };
        }

        public static Func<int, Vector2> Line2D(Vector2 p1, Vector2 p2, int count)
        {
            return (int i) =>
            {
                return p1 + i * (p2 - p1) / count;
            };
        }

        public static Func<int, Vector2> Grid2D(Vector2 p1, Vector2 p2, int cols, int rows)
        {
            return (int i) =>
            {
                return Vector2.Lerp(
                    Vector2.Lerp(p1, new Vector2(p1.X, p2.Y), i / cols / (float) (rows - 1)),
                    Vector2.Lerp(new Vector2(p2.X, p1.Y), p2, i / cols / (float) (rows - 1)),
                    i % cols / (float) (cols - 1)
                );
            };
        }

        public static Func<Vector2, Color4> RadialColorGradient(Func<double, double> easingFunc, Vector2 centre, float radius, Color4 centreColor, Color4 perimiterColor)
        {
            return (Vector2 v) =>
            {
                if (radius == 0) return perimiterColor;
                return centreColor.Lerp(perimiterColor, (float) easingFunc((v - centre).Length / radius));
            };
        }

        public static Func<Vector2, Color4> RadialColorGradient(OsbEasing easing, Vector2 centre, float radius, Color4 centreColor, Color4 perimiterColor)
            => RadialColorGradient(easing.ToEasingFunction(), centre, radius, centreColor, perimiterColor);

        private static double lerp(double a, double b, double t)
            => a + (a - b) * t;

        public static Func<Vector2, double> RadialGradient(Func<double, double> easingFunc, Vector2 centre, float radius, double centreValue, double perimiterValue)
        {
            return (Vector2 v) =>
            {
                if (radius == 0) return perimiterValue;
                return lerp(centreValue, perimiterValue, easingFunc((v - centre).Length / radius));
            };
        }

        public static Func<Vector2, double> RadialGradient(OsbEasing easing, Vector2 centre, float radius, double centreValue, double perimiterValue)
            => RadialGradient(easing.ToEasingFunction(), centre, radius, centreValue, perimiterValue);
    }
}