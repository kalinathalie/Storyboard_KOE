using OpenTK;
using System;

namespace StorybrewCommon.Util
{
    public static class Bezier
    {
        private static int F(int n)
            => n <= 1 ? 1 : n * F(n - 1);

        private static double B(int n, int i, float u)
            => F(n) / (double) (F(i) * F(n - i)) * Math.Pow(u, i) * Math.Pow(1 - u, n - i);

        public static Vector2 OnBezier(ref Vector2[] p, float t)
        {
            Vector2 s = Vector2.Zero;
            for (int i = 0; i <= p.Length - 1; i++)
                s += (float) B(p.Length - 1, i, t) * p[i];
            return s;
        }

        public static Vector2 DerivativeAt(ref Vector2[] p, float t)
        {
            Vector2 s = Vector2.Zero;
            for (int i = 0; i <= p.Length - 2; i++)
                s += (float) B(p.Length - 2, i, t) * (p[i + 1] - p[i]);
            return s * (p.Length - 1);
        }

        public static double Length(ref Vector2[] p, int d)
        {
            double s = 0;
            Vector2 r = OnBezier(ref p, 0);
            for (int i = 1; i <= d; i++)
            {
                Vector2 t = OnBezier(ref p, (float) i / d);
                s += (t - r).Length;
                r = t;
            }
            return s;
        }
    }
}