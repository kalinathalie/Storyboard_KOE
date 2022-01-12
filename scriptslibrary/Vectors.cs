using OpenTK;
using System;
using StorybrewCommon.Scripting;

namespace Project.Util
{
    public static class Vectors
    {
        public static Vector2 Centre = new Vector2(320, 240);

        public static Vector3 Centre3D = Centre.withZ(0);
        
        public static Vector2 ScreenSize = new Vector2(854, 480);

        public static Vector2 ScreenTopLeft = new Vector2(-107, 0);

        public static Vector2 ScreenBottomRight = new Vector2(747, 480);

        public static Vector2 up(float distance)
            => new Vector2(0, -distance);

        public static Vector2 down(float distance)
            => new Vector2(0, distance);

        public static Vector2 left(float distance)
            => new Vector2(-distance, 0);

        public static Vector2 right(float distance)
            => new Vector2(distance, 0);

        public static Vector3 inwards(float distance)
            => new Vector3(0, 0, distance);

        public static Vector3 outwards(float distance)
            => new Vector3(0, 0, -distance);

        public static Vector2 up(this Vector2 vector, float distance)
            => vector + up(distance);

        public static Vector2 down(this Vector2 vector, float distance)
            => vector + down(distance);

        public static Vector2 left(this Vector2 vector, float distance)
            => vector + left(distance);

        public static Vector2 right(this Vector2 vector, float distance)
            => vector + right(distance);

        public static Vector2 angle(double angle)
            => new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));

        public static Vector2 polar(double angle, float distance)
            => distance * new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle));

        public static Vector2 polar(this Vector2 vector, double angle, float distance)
            => vector + polar(angle, distance);

        public static Vector2 rotated(this Vector2 vector, double angle, Vector2 origin)
        {
            Vector2 diff = vector - origin;
            double theta = Math.Atan2(diff.Y, diff.X);
            return Vectors.polar(theta + angle, diff.Length) + origin;
        }

        public static Vector2 rotated(this Vector2 vector, double angle)
            => rotated(vector, angle, Vectors.Centre);
        
        public static Vector2 scaled(this Vector2 vector, double scale, Vector2 origin)
        {
            return (vector - origin) * (float) scale + origin;
        }

        public static Vector2 scaled(this Vector2 vector, double scale)
            => scaled(vector, scale, Vectors.Centre);
        
        // the rotation needed for the top of a sprite to point somewhere
        public static float rotationTo(this Vector2 self, Vector2 other)
        {
            Vector2 diff = other - self;
            return (float) (Math.Atan2(diff.Y, diff.X) + Math.PI / 2);
        }

        // these rotation functions use the right-handed rule

        public static Vector3 rotateX(this Vector3 vector, float angle)
        {
            return vector * new Matrix3(
                1, 0,                       0,
                0, (float) Math.Cos(angle), -(float) Math.Sin(angle),
                0, (float) Math.Sin(angle), (float) Math.Cos(angle)
            );
        }

        public static Vector3 rotateX(this Vector3 vector, float angle, Vector3 origin)
            => (vector - origin).rotateX(angle) + origin;

        public static Vector3 rotateY(this Vector3 vector, float angle)
        {
            return vector * new Matrix3(
                (float) Math.Cos(angle),  0, (float) Math.Sin(angle),
                0,                        1, 0,
                -(float) Math.Sin(angle), 0, (float) Math.Cos(angle)
            );
        }

        public static Vector3 rotateY(this Vector3 vector, float angle, Vector3 origin)
            => (vector - origin).rotateY(angle) + origin;

        public static Vector3 rotateZ(this Vector3 vector, float angle)
        {
            return vector * new Matrix3(
                (float) Math.Cos(angle), -(float) Math.Sin(angle), 0,
                (float) Math.Sin(angle), (float) Math.Cos(angle),  0,
                0,                       0,                        1
            );
        }

        public static Vector3 rotateZ(this Vector3 vector, float angle, Vector3 origin)
            => (vector - origin).rotateZ(angle) + origin;


        public static Vector3 rotate3D(this Vector3 vector, Vector3 rotation, Vector3 origin, bool rollPitchYaw = false)
            => rollPitchYaw ? vector.rotateZ(rotation.Z, origin).rotateX(rotation.X, origin).rotateY(rotation.Y, origin)
                : vector.rotateX(rotation.X, origin).rotateY(rotation.Y, origin).rotateZ(rotation.Z, origin);

        public static Vector3 rotate3D(this Vector3 vector, Vector3 rotation, bool rollPitchYaw = false)
            => vector.rotate3D(rotation, Vector3.Zero, rollPitchYaw);

        // the rotation of a vector relative to straight toward the screen
        public static Vector3 getRotation(this Vector3 vector)
            => new Vector3(
                (float) Math.Atan2(vector.Y, vector.Z),
                (float) Math.Atan2(-vector.X, vector.Z),
                0 // no Z rotation because roll doesn't apply
            );

        public static Vector3 randomDirection(Vector3 direction, float angularVariance, Random rng, StoryboardObjectGenerator logger = null)
        {
            // random point on a cylindrical slice
            double z = 1 - (1 - Math.Cos(MathHelper.DegreesToRadians(angularVariance))) * rng.NextDouble();
            double angle = 2 * Math.PI * rng.NextDouble();
            double x = Math.Cos(angle);
            double y = Math.Sin(angle);

            // projected onto a sphere
            double r = Math.Sqrt(1 - z * z);
            x *= r;
            y *= r;

            if (logger != null) logger.Log(direction);
            if (logger != null) logger.Log(direction.getRotation());
            if (logger != null) logger.Log(round(outwards(1).rotate3D(direction.getRotation(), true), 2));
            return new Vector3((float) x, (float) y, (float) z).rotate3D(direction.getRotation(), true);
        }

        public static Vector3 withZ(this Vector2 vector, float z)
            => new Vector3(vector.X, vector.Y, z);

        public static Vector2 round(Vector2 vector, int decimals = 0)
            => new Vector2(
                (float) Math.Round(vector.X, decimals),
                (float) Math.Round(vector.Y, decimals)
            );

        public static Vector3 round(Vector3 vector, int decimals = 0)
            => new Vector3(
                (float) Math.Round(vector.X, decimals),
                (float) Math.Round(vector.Y, decimals),
                (float) Math.Round(vector.Z, decimals)
            );

        public struct ProjectionResult
        {
            public Vector3 vector;
            public Vector3 normalisedVector;
            public Vector2 projectedVector;
            public Vector2 screenSpaceVector;
        }

        public static ProjectionResult project(this Vector3 vector, Vector2 screenSize, Vector2 screenCentre, float fovInDegrees)
        {
            var res = new ProjectionResult();
            float focalDistance = (float) Math.Tan((Math.PI - MathHelper.DegreesToRadians(fovInDegrees)) / 2) * screenSize.X / 2;
            res.vector = vector;
            res.normalisedVector = (vector - screenCentre.withZ(-focalDistance)) * new Vector3(2 / screenSize.X, 2 / screenSize.Y, 1 / focalDistance);
            res.projectedVector = res.normalisedVector.Xy * (1 / res.normalisedVector.Z);
            res.screenSpaceVector = res.projectedVector * ScreenSize / 2 + screenCentre;
            return res;
        }

        public static ProjectionResult project(this Vector3 vector, float fovInDegrees)
            => vector.project(ScreenSize, Centre, fovInDegrees);
    }
}