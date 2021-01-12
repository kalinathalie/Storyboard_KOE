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
    public class DropSetup : StoryboardObjectGenerator
    {
        public override void Generate()
        {
		    var blackbar1 = GetLayer("Main").CreateSprite("sb/pixel.png", OsbOrigin.TopCentre, new Vector2(320,0));
            var blackbar2 = GetLayer("Main").CreateSprite("sb/pixel.png", OsbOrigin.BottomCentre, new Vector2(320,480));

            blackbar1.Color(62211, Color4.Black);
            blackbar2.Color(62211, Color4.Black);

            blackbar1.ScaleVec(OsbEasing.OutExpo,62211,62840,854.0/2,0,854.0/2,120);
            blackbar2.ScaleVec(OsbEasing.OutExpo,62211,62840,854.0/2,0,854.0/2,120);

            var bg = GetLayer("Main").CreateSprite("sb/bgs/K4L1/K4L1_blured.jpg");
            bg.Scale(62840,480.0/1080);
            bg.Fade(62840,63154,1,0.66);
            bg.Fade(72107,72578,0.66,0);

            var flash = GetLayer("Main").CreateSprite("sb/pixel.png");
            flash.ScaleVec(62840,854/2,480/2);
            flash.Fade(OsbEasing.OutCirc,62840,63154,0.7,0);

            
        }
    }
}
