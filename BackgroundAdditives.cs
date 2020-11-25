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
    public class BackgroundAdditives : StoryboardObjectGenerator
    {
        // Makes Basic shapes to contain the format the frame with the background
        // Adapts with the color of the map


        private StoryboardLayer layer;
        public override void Generate()
        {
            layer = GetLayer("Background");
            //Get colors
		    Color4 colorOne = Beatmap.ComboColors.ElementAt(1);
            Color4 colorTwo = Beatmap.ComboColors.ElementAt(1);

            //Main Frame, spawns from left to right
            var pannel1 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel1.ScaleVec(1583,1500,1500);
            pannel1.Rotate(1583,Math.PI/12);
            pannel1.Move(OsbEasing.OutCubic,1583,1583+650,-170,240,850,240);
            pannel1.Fade(1583,2211,1,1);

            var pannel2 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel2.ScaleVec(1740,1500,1500);
            pannel2.Rotate(1740,Math.PI/12);
            pannel2.Move(OsbEasing.OutCubic,1740,1740+650,-170,240,850,240);
            pannel2.Fade(1740,12578,1,1);
            pannel2.Color(1740,colorOne);
            pannel2.ScaleVec(OsbEasing.OutCubic,2054,2054+500,1500,1500,235,1500);

            var pannel3 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel3.ScaleVec(2211,15,1500);
            pannel3.Rotate(2211,Math.PI/12);
            pannel3.Move(OsbEasing.OutCubic,2211,2211+500,-170,240,330,240);
            pannel3.Fade(2211,12578,1,1);
        }
    }
}
