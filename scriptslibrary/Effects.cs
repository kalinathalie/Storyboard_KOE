using OpenTK;
using StorybrewCommon.Mapset;
using StorybrewCommon.Curves;
using StorybrewCommon.Animations;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Util
{
    // common effects that might be reused
    static class Effects
    {
        public static SpriteSet Trail(StoryboardLayer layer, OsbSprite sprite, double startTime, double endTime, double timeBetweenSprites, double fadeInTime, double duration, double fadeOutTime, bool cutWhenSpriteDisappears)
        {
            SpriteSet trail = layer.CreateSpriteSet();
            for (double t = startTime; t < endTime; t += timeBetweenSprites)
            {
                if (sprite.OpacityAt(t) == 0) continue;
                var trailSprite = layer.CreateSprite(sprite.TexturePath, OsbOrigin.Centre, sprite.PositionAt(t));
                double s = fadeInTime;
                double d = duration;
                double e = fadeOutTime;
                double up = sprite.OpacityAt(t);
                double down = 0;
                if (cutWhenSpriteDisappears)
                {
                    if (sprite.CommandsEndTime <= t + fadeInTime)
                    { // sprite doesn't get to fade in fully
                        s = sprite.CommandsEndTime - t;
                        up = InterpolatingFunctions.Double(0, up, (sprite.CommandsEndTime - t) / (t + fadeInTime));
                    }
                    else if (sprite.CommandsEndTime <= t + fadeInTime + duration)
                    { // sprite doesn't get to fade out
                        d = sprite.CommandsEndTime - (t + fadeInTime);
                        e = 0;
                    }
                    else if (sprite.CommandsEndTime <= t + fadeInTime + duration + fadeOutTime)
                    { // sprite doesn't get to fade out fully
                        e = sprite.CommandsEndTime - (t + fadeInTime + duration);
                        down = InterpolatingFunctions.Double(up, 0, (sprite.CommandsEndTime - (t + fadeInTime + duration)) / (fadeOutTime));
                    }
                }
                if (s == 0 && e == 0)
                    trailSprite.Fade(t, t + d, 1, 1);
                else
                {
                    if (s != 0 && d != 0) trailSprite.Fade(t, t + s, 0, up);
                    if (e != 0) trailSprite.Fade(t + d, t + d + e, up, down);
                }
                trail.AddSprite(trailSprite);
            }
            return trail;
        }
    }
}