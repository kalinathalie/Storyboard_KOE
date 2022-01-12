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
    // a group of sprites that can be acted upon as a group
    // there can never be enough convenience functions
    class SpriteSet
    {
        private StoryboardLayer layer;

        private List<OsbSprite> sprites = new List<OsbSprite>();

        public IEnumerable<OsbSprite> Sprites
            => sprites;

        public SpriteSet(StoryboardLayer layer)
        {
            this.layer = layer;
        }

        public SpriteSet(SpriteSet other)
        {
            layer = other.layer;
            foreach (OsbSprite sprite in other.sprites)
                sprites.Add(layer.CreateSprite(sprite.TexturePath, sprite.Origin, sprite.InitialPosition));
        }

        public SpriteSet(StoryboardLayer layer, Func<int, String> spritePaths, Func<int, OsbOrigin> origins, Func<int, Vector2> initialPositions, int count, int offset = 0)
        {
            this.layer = layer;
            AddSprites(spritePaths, origins, initialPositions, count, offset);
        }

        public SpriteSet(StoryboardLayer layer, Func<int, String> spritePaths, Func<int, OsbOrigin> origins, int count, int offset = 0)
        {
            this.layer = layer;
            AddSprites(spritePaths, origins, count, offset);
        }

        public SpriteSet(StoryboardLayer layer, Func<int, String> spritePaths, int count, int offset = 0)
        {
            this.layer = layer;
            AddSprites(spritePaths, count, offset);
        }

        public SpriteSet(StoryboardLayer layer, String spritePath, int count, int offset = 0)
        {
            this.layer = layer;
            AddSprites(spritePath, count);
        }

        public static SpriteSet operator +(SpriteSet self, SpriteSet other)
            => new SpriteSet(self.layer).AddSprites(self).AddSprites(other);

        public SpriteSet AddSprite(OsbSprite sprite)
        {
            sprites.Add(sprite);
            return this;
        }

        public SpriteSet AddSprite(String spritePath, OsbOrigin origin, Vector2 initialPosition)
        {
            sprites.Add(layer.CreateSprite(spritePath, origin, initialPosition));
            return this;
        }

        public SpriteSet AddSprite(String spritePath, OsbOrigin origin)
        {
            sprites.Add(layer.CreateSprite(spritePath, origin));
            return this;
        }

        public SpriteSet AddSprite(String spritePath)
        {
            sprites.Add(layer.CreateSprite(spritePath));
            return this;
        }

        public SpriteSet AddSprites(SpriteSet other)
        {
            foreach (var sprite in other.Sprites)
                sprites.Add(sprite);
            return this;
        }

        public SpriteSet AddSprites(Func<int, String> spritePaths, Func<int, OsbOrigin> origins, Func<int, Vector2> initialPositions, int count, int offset = 0)
        {

            for (int i = offset; i < count + offset; i++)
                sprites.Add(layer.CreateSprite(spritePaths(i), origins(i), initialPositions(i)));
            return this;
        }

        public SpriteSet AddSprites(Func<int, String> spritePaths, Func<int, OsbOrigin> origins, int count, int offset = 0)
            => AddSprites(spritePaths, origins, _ => OsbSprite.DefaultPosition, count, offset);

        public SpriteSet AddSprites(Func<int, String> spritePaths, int count, int offset = 0)
            => AddSprites(spritePaths, _ => OsbOrigin.Centre, _ => OsbSprite.DefaultPosition, count, offset);

        public SpriteSet AddSprites(String spritePath, Func<int, OsbOrigin> origins, Func<int, Vector2> initialPositions, int count, int offset = 0)
            => AddSprites(_ => spritePath, origins, initialPositions, count, offset);

        public SpriteSet AddSprites(String spritePath, Func<int, OsbOrigin> origins, int count, int offset = 0)
            => AddSprites(_ => spritePath, origins, _ => OsbSprite.DefaultPosition, count, offset);

        public SpriteSet AddSprites(String spritePath, int count)
            => AddSprites(_ => spritePath, _ => OsbOrigin.Centre, _ => OsbSprite.DefaultPosition, count);

        public OsbSprite Get(int index)
            => Sprites.ElementAt(index);

        public void MoveRelative(OsbEasing easing, double startTime, double endTime, Vector2 startPosition, Vector2 endPosition, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(easing, startTime, endTime, startPosition + (Vector2) sprites[i].PositionAt(startTime), endPosition + (Vector2) sprites[i].PositionAt(startTime));
        }

        public void Move(Movement movement, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(movement);
        }

        public void MoveRelative(Movement movement, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(movement + (Vector2) sprites[i].PositionAt(movement.StartTime));
        }

        public void Move(Func<int, Movement> movements, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(movements(i));
        }

        public void MoveRelative(Func<int, Movement> movements, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(movements(i) + (Vector2) sprites[i].PositionAt(movements(i).StartTime));
        }


        public void Move(OsbEasing easing, double startTime, double endTime, Vector2 startPosition, Vector2 endPosition, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(easing, startTime, endTime, startPosition, endPosition);
        }

        public void Move(double time, Vector2 position, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(time, position);
        }

        public void MoveRelative(double time, Vector2 position, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(time, position + (Vector2) sprites[i].PositionAt(time));
        }

        public void MoveTo(OsbEasing easing, double startTime, double endTime, Func<int, Vector2> positions, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Move(easing, startTime, endTime, sprites[i].PositionAt(startTime), positions(i));
        }

        public void Move3D(
            OsbEasing easing,
            double startTime,
            double endTime,
            Func<int, Vector3> startPositions,
            Func<int, Vector3> endPositions,
            float fov,
            Action<OsbSprite, Vectors.ProjectionResult, Vectors.ProjectionResult> secondaryAction = null,
            int offset = 0,
            int count = -1
        )
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
            {
                sprites[i].Move3D(easing, startTime, endTime, startPositions(i), endPositions(i), fov, secondaryAction);
            }
        }

        public void Scale(OsbEasing easing, double startTime, double endTime, double startScale, double endScale, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Scale(easing, startTime, endTime, startScale, endScale);
        }

        public void Scale(double time, double scale, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Scale(time, scale);
        }

        public void Scale(double scale, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Scale(scale);
        }

        public void Scale(double startTime, double endTime, double startScale, double endScale, int offset = 0, int count = -1)
            => Scale(OsbEasing.None, startTime, endTime, startScale, endScale, offset, count);

        public void UseScaleOf(OsbSprite other, bool useYScale = false, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleOf(other, useYScale);
        }

        public void UseScaleOf(OsbSprite other, double time, bool useYScale = false, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleOf(other, time, useYScale);
        }

        public void UseScalesOf(SpriteSet other, bool useYScale = false, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleOf(other.Get(i), useYScale);
        }

        public void UseScalesOf(SpriteSet other, double time, bool useYScale = false, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleOf(other.Get(i), time, useYScale);
        }

        public void ScaleVec(OsbEasing easing, double startTime, double endTime, Vector2 startScale, Vector2 endScale, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].ScaleVec(easing, startTime, endTime, startScale, endScale);
        }

        public void ScaleVec(double startTime, double endTime, Vector2 startScale, Vector2 endScale, int offset = 0, int count = -1)
            => ScaleVec(OsbEasing.None, startTime, endTime, startScale, endScale, offset, count);

        public void ScaleVec(double time, Vector2 scale, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].ScaleVec(time, scale);
        }

        public void ScaleVec(Vector2 scale, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].ScaleVec(scale);
        }

        public void UseScaleVecOf(OsbSprite other, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleVecOf(other);
        }

        public void UseScaleVecOf(OsbSprite other, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleVecOf(other, time);
        }

        public void UseScaleVecsOf(SpriteSet other, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleVecOf(other.Get(i));
        }

        public void UseScaleVecsOf(SpriteSet other, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseScaleVecOf(other.Get(i), time);
        }

        public void Rotate(OsbEasing easing, double startTime, double endTime, double startRotation, double endRotation, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Rotate(easing, startTime, endTime, startRotation, endRotation);
        }

        public void Rotate(double startTime, double endTime, double startRotation, double endRotation, int offset = 0, int count = -1)
            => Rotate(OsbEasing.None, startTime, endTime, startRotation, endRotation, offset, count);

        public void Rotate(OsbEasing easing, double startTime, double endTime, Func<int, double> startRotations, Func<int, double> endRotations, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Rotate(easing, startTime, endTime, startRotations(i), endRotations(i));
        }

        public void Rotate(double time, Func<int, double> rotations, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Rotate(time, rotations(i));
        }

        public void Rotate(double time, double rotation, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Rotate(time, rotation);
        }

        public void Rotate(Func<int, double> rotations, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Rotate(rotations(i));
        }

        public void Rotate(double rotation, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Rotate(rotation);
        }

        public void UseRotationOf(OsbSprite other, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseRotationOf(other);
        }

        public void UseRotationOf(OsbSprite other, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseRotationOf(other, time);
        }

        public void UseRotationsOf(SpriteSet other, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseRotationOf(other.Get(1));
        }

        public void UseRotationsOf(SpriteSet other, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseRotationOf(other.Get(1), time);
        }

        // so 1.0 refers to the original scales of the sprites in the spriteset
        public void Scale(OsbEasing easing, double startTime, double endTime, double relativeStartScale, double relativeEndScale, Vector2 origin, double delta = 1000.0 / 60)
        {
            foreach (var sprite in Sprites)
            {
                Movement movement = new Movement(
                    easing,
                    startTime,
                    endTime,
                    Vector2.Lerp(origin, sprite.PositionAt(startTime), (float) relativeStartScale),
                    Vector2.Lerp(origin, sprite.PositionAt(startTime), (float) relativeEndScale)
                );

                Vector2 originalScale = sprite.ScaleAt(startTime);
                    if (originalScale.X == originalScale.Y)
                        sprite.Scale(easing, startTime, endTime, originalScale.X * relativeStartScale, originalScale.X * relativeEndScale);
                    else
                        sprite.ScaleVec(easing, startTime, endTime, originalScale * (float) relativeStartScale, originalScale * (float) relativeEndScale);

                sprite.Move(movement);
            }
        }

        public void ScaleAndRotate(OsbEasing easing, double startTime, double endTime, double scaleFactor, Vector2 origin, double rotation = 0, double delta = 1000.0 / 60)
        {
            if (endTime - startTime <= 0 || (scaleFactor == 1 && rotation == 0) || delta == 0) return;
            foreach (var sprite in Sprites)
            {
                Movement movement = new Movement(
                    easing,
                    startTime,
                    endTime,
                    sprite.PositionAt(startTime),
                    Vector2.Lerp(origin, sprite.PositionAt(startTime), (float) scaleFactor)
                );
                if (scaleFactor != 1)
                {
                    Vector2 originalScale = sprite.ScaleAt(startTime);
                    if (originalScale.X == originalScale.Y)
                        sprite.Scale(easing, startTime, endTime, originalScale.X, originalScale.X * scaleFactor);
                    else
                        sprite.ScaleVec(easing, startTime, endTime, originalScale, originalScale * (float) scaleFactor);
                }

                if (rotation != 0)
                {
                    Movement split = movement.ResampledSection(startTime, endTime, delta);
                    foreach (var command in split.Commands)
                    {
                        var ease = easing.ToEasingFunction();
                        command.StartValue = ((Vector2) command.StartValue)
                            .rotated(rotation * ease((command.StartTime - startTime) / (endTime - startTime)), origin);
                        command.EndValue = ((Vector2) command.EndValue)
                            .rotated(rotation * ease((command.EndTime - startTime) / (endTime - startTime)), origin);
                    }
                    sprite.Move(split);
                    sprite.Rotate(easing, startTime, endTime, sprite.RotationAt(startTime), sprite.RotationAt(startTime) + rotation);
                    continue;
                }
                sprite.Move(movement);
            }
        }

        public void Fade(OsbEasing easing, double startTime, double endTime, double startOpacity, double endOpacity, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Fade(easing, startTime, endTime, startOpacity, endOpacity);
        }

        public void Fade(double startTime, double endTime, double startOpacity, double endOpacity, int offset = 0, int count = -1)
            => Fade(OsbEasing.None, startTime, endTime, startOpacity, endOpacity, offset, count);

        public void Fade(double time, double opacity, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Fade(time, opacity);
        }

        public void Fade(OsbEasing easing, Func<int, double> startTimes, Func<int, double> endTimes, double startOpacity, double endOpacity, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Fade(easing, startTimes(i), endTimes(i), startOpacity, endOpacity);
        }

        public void Fade(OsbEasing easing, Func<int, double> startTimes, Func<int, double> endTimes, Func<int, double> startOpacities, Func<int, double> endOpacities, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Fade(easing, startTimes(i), endTimes(i), startOpacities(i), endOpacities(i));
        }

        public void Fade(Func<Vector2, double> pattern, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Fade(time, pattern(sprites[i].PositionAt(time)));
        }

        public void Color(OsbEasing easing, double startTime, double endTime, Color4 startColor, Color4 endColor, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Color(easing, startTime, endTime, startColor, endColor);
        }

        public void Color(double time, Color4 color, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Color(time, color);
        }

        public void Color(double time, Func<int, Color4> colors, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Color(time, colors(i));
        }

        public void Color(Color4 color, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Color(color);
        }

        public void Color(Func<int, Color4> colors, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Color(colors(i));
        }

        public void UseColorOf(OsbSprite other, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseColorOf(other);
        }

        public void UseColorOf(OsbSprite other, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseColorOf(other, time);
        }

        public void UseColorsOf(SpriteSet other, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseColorOf(other.Get(i));
        }

        public void UseColorsOf(SpriteSet other, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].UseColorOf(other.Get(i), time);
        }

        public void Color(Func<Vector2, Color4> colorPattern, double time, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Color(time, colorPattern(sprites[i].PositionAt(time)));
        }

        public void Additive(double startTime, double endTime, int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Additive(startTime, endTime);
        }

        public void Additive(int offset = 0, int count = -1)
        {
            for (int i = offset; i < (count < 0 ? Sprites.Count() : count + offset); i++)
                sprites[i].Additive();
        }
    }

    static class LayerExtensions
    {
        public static SpriteSet CreateSpriteSet(this StoryboardLayer layer)
            => new SpriteSet(layer);

        public static SpriteSet CreateSpriteSet(this StoryboardLayer layer, Func<int, String> spritePaths, Func<int, OsbOrigin> origins, Func<int, Vector2> initialPositions, int count, int offset = 0)
            => new SpriteSet(layer).AddSprites(spritePaths, origins, initialPositions, count, offset);

        public static SpriteSet CreateSpriteSet(this StoryboardLayer layer, Func<int, String> spritePaths, Func<int, OsbOrigin> origins, int count, int offset = 0)
            => new SpriteSet(layer).AddSprites(spritePaths, origins, count, offset);

        public static SpriteSet CreateSpriteSet(this StoryboardLayer layer, Func<int, String> spritePaths, int count, int offset = 0)
            => new SpriteSet(layer).AddSprites(spritePaths, count, offset);

        public static SpriteSet CreateSpriteSet(this StoryboardLayer layer, String spritePath, Func<int, OsbOrigin> origins, Func<int, Vector2> initialPositions, int count, int offset = 0)
            => new SpriteSet(layer).AddSprites(spritePath, origins, initialPositions, count, offset);

        public static SpriteSet CreateSpriteSet(this StoryboardLayer layer, String spritePath, Func<int, OsbOrigin> origins, int count, int offset = 0)
            => new SpriteSet(layer).AddSprites(spritePath, origins, count, offset);

        public static SpriteSet CreateSpriteSet(this StoryboardLayer layer, String spritePath, int count, int offset = 0)
            => new SpriteSet(layer).AddSprites(spritePath, count);
    }
}