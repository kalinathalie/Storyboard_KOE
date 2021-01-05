using OpenTK;
using OpenTK.Graphics;
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
    public class Cursor : StoryboardObjectGenerator
    {[Configurable]
        public int StartTime = 0;

        [Configurable]
        public int EndTime = 0;

        [Configurable]
        public int BeatDivisor = 8;

        [Configurable]
        public int FadeTime = 200;

        [Configurable]
        public string SpritePath = "sb/glow.png";

        [Configurable]
        public double SpriteScale = 1;

        public override void Generate()
        {
            var hitobjectLayer = GetLayer("");
            var startPos = Beatmap.HitObjects.Where((o) => o.EndTime > StartTime).First().Position;
            var hSprite = hitobjectLayer.CreateSprite(SpritePath, OsbOrigin.Centre, startPos);
            hSprite.Scale(StartTime, SpriteScale);
            
            OsuHitObject prevObject = null;
            foreach (OsuHitObject hitobject in Beatmap.HitObjects)
            {
                if ((StartTime != 0 || EndTime != 0) && 
                    (hitobject.StartTime < StartTime - 5 || EndTime - 5 <= hitobject.StartTime))
                    continue;
                
                if (prevObject != null) 
                {
                    hSprite.Move(prevObject.EndTime, hitobject.StartTime, prevObject.EndPosition, hitobject.Position);
                    //hSprite.Scale(OsbEasing.In, hitobject is OsuSlider ? hitobject.StartTime : prevObject.EndTime, hitobject.EndTime, SpriteScale, SpriteScale * 0.6);
                }
                //hSprite.Color(hitobject.StartTime, hitobject.Color);

                if (hitobject is OsuSlider)
                {
                    var timestep = Beatmap.GetTimingPointAt((int)hitobject.StartTime).BeatDuration / BeatDivisor;
                    var startTime = hitobject.StartTime;
                    var slider = (OsuSlider) hitobject;
                    Log(slider.Additions.ToString());
                    
                    
                    while (true)
                    {
                        var endTime = startTime + timestep;

                        var complete = hitobject.EndTime - endTime < 5;
                        if (complete) endTime = hitobject.EndTime;

                        var startPosition = hSprite.PositionAt(startTime);
                        hSprite.Move(startTime, endTime, startPosition, hitobject.PositionAtTime(endTime));

                        if (complete) break;
                        startTime += timestep;
                    }
                }
                prevObject = hitobject;
            }
        }
    }
}
