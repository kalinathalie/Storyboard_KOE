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
    public class LateralAdditives : StoryboardObjectGenerator
    {
        // Adds some background effects to the side pannels
        StoryboardLayer layer;
        double beatduration;
        public override void Generate()
        {
		    layer = GetLayer("Background");
            beatduration = Beatmap.GetTimingPointAt(2000).BeatDuration;

            for(int x = 0; x < 7; x++)
            {
                for(int y = 0; y < 17; y++)
                {
                    OsbSprite circle = layer.CreateSprite("sb/circle.png");
                    circle.Scale(2211,0.07);
                    circle.Fade(2211,2526,0,0.25);
                    circle.Move(2211,12578,395 + x*66 + 2*y, 0+y*50+x, 395 + x*66 +2*y, 0+y*50 + x - 270);
                    circle.Fade(11950,11950+420,0.25,0);
                }
            }
            
        }
    }
}
