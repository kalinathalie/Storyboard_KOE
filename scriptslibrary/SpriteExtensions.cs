using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Curves;
using StorybrewCommon.Animations;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Project.Util
{
    public static class SpriteExtensions
    {
        /*
        public static void Follow(this OsbSprite sprite, OsbSprite other)
        {
            foreach (var command in other.Commands)
            {
                if (!(command is MoveCommand)) continue;

                var moveCommand = (MoveCommand) command;

                double s = moveCommand.StartTime;
                double e = moveCommand.EndTime;

                if (s == e) continue;

                Vector2 p1 = moveCommand.StartValue;
                Vector2 p2 = moveCommand.EndValue;

                if (p1 == p2) continue;

                OsbEasing easing = moveCommand.Easing;

                sprite.Move(easing, s, e, p1, p2);
            }
        }
        */

        public static void Move(this OsbSprite sprite, Movement movement)
            => movement.ApplyTo(sprite);

        public static void Move3D(
            this OsbSprite sprite,
            OsbEasing easing,
            double startTime,
            double endTime,
            Vector3 startPosition,
            Vector3 endPosition,
            float fovInDegrees,
            Action<OsbSprite, Vectors.ProjectionResult, Vectors.ProjectionResult> secondaryAction = null
        )
        {
            Vectors.ProjectionResult p1 = Vectors.project(startPosition, Vectors.ScreenSize, Vectors.Centre, fovInDegrees);
            Vectors.ProjectionResult p2 = Vectors.project(endPosition, Vectors.ScreenSize, Vectors.Centre, fovInDegrees);
            if (p1.screenSpaceVector != p2.screenSpaceVector) sprite.Move(easing, startTime, endTime, p1.screenSpaceVector, p2.screenSpaceVector);
            if (secondaryAction != null)
                secondaryAction(sprite, p1, p2); // for handling scale, fade, colour, etc. based on coordinates, distance to camera, etc.
        }

        public static void Rotate(this OsbSprite sprite, Func<double, double> easingFunction, double startTime, double endTime, double precision)
        {
            int points = (int) ((endTime - startTime) / precision);
            for (int i = 0; i < points; i++)
            {
                double d = (double) i / points;
                double t = d * (double) (endTime - startTime) + (double) startTime;
                double d2 = (double) (i + 1) / points;
                double t2 = d2 * (double) (endTime - startTime) + (double) startTime;
                double r = easingFunction(d);
                double r2 = easingFunction(d2);

                // the sign of rotation flips after a full rotation so this kinda lazily corrects it
                if (Math.Abs(r2 - r) >= 1.6 * Math.PI) r2 += 2 * Math.PI * -Math.Sign(r2 - r);

                sprite.Rotate(OsbEasing.None, t, t2, r, r2);
            }
        }

        public static void Scale(this OsbSprite sprite, Func<double, double> easingFunction, double startTime, double endTime, double precision)
        {
            int points = (int) ((endTime - startTime) / precision);
            for (int i = 0; i < points; i++)
            {
                double d = (double) i / points;
                double t = d * (double) (endTime - startTime) + (double) startTime;
                double d2 = (double) (i + 1) / points;
                double t2 = d2 * (double) (endTime - startTime) + (double) startTime;
                sprite.Scale(OsbEasing.None, t, t2, easingFunction(d), easingFunction(d2));
            }
        }

        public static void ScaleVec(this OsbSprite sprite, Func<double, double> easingFunctionX, Func<double, double> easingFunctionY, double startTime, double endTime, double precision)
        {
            int points = (int) ((endTime - startTime) / precision);
            for (int i = 0; i < points; i++)
            {
                double d = (double) i / points;
                double t = d * (double) (endTime - startTime) + (double) startTime;
                double d2 = (double) (i + 1) / points;
                double t2 = d2 * (double) (endTime - startTime) + (double) startTime;
                sprite.Scale(OsbEasing.None, t, t2, easingFunctionX(d), easingFunctionY(d2));
            }
        }

        public static void Scale(this OsbSprite sprite, double scale)
            => sprite.Scale(sprite.CommandsStartTime, scale);

        public static void ScaleVec(this OsbSprite sprite, Vector2 scale)
            => sprite.ScaleVec(sprite.CommandsStartTime, scale);

        public static void ScaleToFill(this OsbSprite sprite)
        {
            var bitmap = Bitmap.FromFile(sprite.GetTexturePathAt(sprite.CommandsStartTime));
            sprite.Scale(
                bitmap.Width / bitmap.Height < 480.0 / 854.0 ?
                480.0 / bitmap.Height
                : 854.0 / bitmap.Width
            );
        }

        public static void ScaleToFit(this OsbSprite sprite)
        {
            var bitmap = Bitmap.FromFile(sprite.GetTexturePathAt(sprite.CommandsStartTime));
            sprite.Scale(
                bitmap.Width / bitmap.Height > 480.0 / 854.0 ?
                480.0 / bitmap.Height
                : 854.0 / bitmap.Width
            );
        }

        public static void Move(this OsbSprite sprite, Vector2 position)
            => sprite.Move(sprite.CommandsStartTime, position);

        public static void Rotate(this OsbSprite sprite, double rotation)
            => sprite.Rotate(sprite.CommandsStartTime, rotation);

        public static void Color(this OsbSprite sprite, Color4 color)
            => sprite.Color(sprite.CommandsStartTime, color);

        public static void Additive(this OsbSprite sprite)
            => sprite.Additive(sprite.CommandsStartTime, sprite.CommandsEndTime);

        public static void UseScaleOf(this OsbSprite sprite, OsbSprite other, bool useYScale = false)
            => sprite.Scale(sprite.CommandsStartTime, useYScale ? other.ScaleAt(sprite.CommandsStartTime).Y : other.ScaleAt(sprite.CommandsStartTime).X);

        public static void UseScaleVecOf(this OsbSprite sprite, OsbSprite other)
            => sprite.ScaleVec(sprite.CommandsStartTime, other.ScaleAt(sprite.CommandsStartTime));

        public static void UsePositionOf(this OsbSprite sprite, OsbSprite other)
            => sprite.InitialPosition = other.PositionAt(sprite.CommandsStartTime);

        public static void UseRotationOf(this OsbSprite sprite, OsbSprite other)
            => sprite.Rotate(sprite.CommandsStartTime, other.RotationAt(sprite.CommandsStartTime));

        public static void UseColorOf(this OsbSprite sprite, OsbSprite other)
            => sprite.Color(sprite.CommandsStartTime, other.ColorAt(sprite.CommandsStartTime));

        public static void UseScaleOf(this OsbSprite sprite, OsbSprite other, double time, bool useYScale = false)
            => sprite.Scale(sprite.CommandsStartTime, useYScale ? other.ScaleAt(time).Y : other.ScaleAt(time).X);

        public static void UseScaleVecOf(this OsbSprite sprite, OsbSprite other, double time)
            => sprite.ScaleVec(sprite.CommandsStartTime, other.ScaleAt(time));

        public static void UsePositionOf(this OsbSprite sprite, OsbSprite other, double time)
            => sprite.InitialPosition = other.PositionAt(time);

        public static void UseRotationOf(this OsbSprite sprite, OsbSprite other, double time)
            => sprite.Rotate(sprite.CommandsStartTime, other.RotationAt(time));

        public static void UseColorOf(this OsbSprite sprite, OsbSprite other, double time)
            => sprite.Color(sprite.CommandsStartTime, other.ColorAt(time));
    }
}