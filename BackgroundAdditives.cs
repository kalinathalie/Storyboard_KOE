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
		    Color4 colorOne = Beatmap.ComboColors.ElementAt(0);
            Color4 colorTwo = Beatmap.ComboColors.ElementAt(1);


            //Section 1 (1583 - 12578), color 1
            //Main White Frame, spawns from left to right 
            var pannel1 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel1.ScaleVec(1583,1500,1500);
            pannel1.Rotate(1583,Math.PI/24);
            pannel1.Move(OsbEasing.OutCubic,1583,1583+700,-115,240,850,240);
            pannel1.Fade(1583,2211,1,1);

            //Main Color1 Frame, spawns from left to right 
            var pannel2 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel2.ScaleVec(1740,1500,1500);
            pannel2.Rotate(1740,Math.PI/24);
            pannel2.Move(OsbEasing.OutCubic,1740,1740+700,-115,240,830,240);
            pannel2.Fade(1740,12264,1,1);
            pannel2.Color(1740,colorOne);
            pannel2.ScaleVec(OsbEasing.OutCubic,2211,2211+500,1500,1500,235,1500);

            //Additionnal white frames
            var pannel3 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel3.ScaleVec(2211,6,1500);
            pannel3.Rotate(2211,Math.PI/24);
            pannel3.Move(OsbEasing.OutCubic,2211,2211+500,-115,240,330,240);
            pannel3.Fade(2211,12264,1,1);

            var pannel4 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel4.ScaleVec(2211,23,1500);
            pannel4.Rotate(2211,Math.PI/24);
            pannel4.Move(OsbEasing.OutCubic,2211,2211+500,-115,240,420,240);
            pannel4.Fade(2211,12264,1,1);

            //Section 2 (11950 - 22316)
            var pannel5 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreLeft);
            pannel5.ScaleVec(11950,500,1500);
            pannel5.Rotate(11950,Math.PI/24);
            pannel5.Color(11950,colorTwo);
            pannel5.Move(OsbEasing.OutCubic,11950,11950+500,-900,240,-140,240);
            pannel5.Fade(11950,22316,1,1);
            pannel5.ScaleVec(OsbEasing.OutCirc,12264,12264+420,500,1500,215,1500);

            var pannel6 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel6.ScaleVec(12264,19,1500);
            pannel6.Rotate(12264,Math.PI/24);
            pannel6.Move(OsbEasing.OutCubic,12264,12264+500,820,240,282,240);
            pannel6.Fade(12264,22316,1,1);

            var pannel7 = layer.CreateSprite("sb/pixel.png",OsbOrigin.CentreRight);
            pannel7.ScaleVec(12264,5,1500);
            pannel7.Rotate(12264,Math.PI/24);
            pannel7.Move(OsbEasing.OutCubic,12264,12264+500,850,260,320,240);
            pannel7.Fade(12264,22316,1,1);
            
            //Transition to the next section

            var circleTransit = layer.CreateSprite("sb/to.png", OsbOrigin.Centre, new Vector2(50,240));
            circleTransit.Color(19803,Color4.Black);
            circleTransit.Fade(19803,20745,1,1);
            circleTransit.Scale(OsbEasing.OutCirc,19803,19803+150,0,0.08);
            circleTransit.Scale(OsbEasing.OutCirc,19960,22316,0.3,0.2);

            var circleOutline = layer.CreateSprite("sb/q2.png", OsbOrigin.Centre, new Vector2(50,240));
            circleOutline.Color(19960,Color4.Black);
            circleOutline.Fade(19960,20745,1,1);
            circleOutline.Scale(OsbEasing.OutCirc,19960,22316,0,0.5);

            var line = layer.CreateSprite("sb/pixel.png", OsbOrigin.Centre, new Vector2(50,240));
            line.Color(19960,Color4.Black);
            line.Fade(19960,22316,1,1);
            line.ScaleVec(OsbEasing.InExpo,19960,22316,500,2,1250,0.66);
            line.Rotate(OsbEasing.InExpo,19960,22316,Math.PI/24 + Math.PI/2,-1.66*Math.PI);

            var circleFull = layer.CreateSprite("sb/circle.png", OsbOrigin.Centre, new Vector2(50,240));
            circleFull.Color(21374,Color4.Black);
            circleFull.Scale(OsbEasing.InExpo, 21374, 22395, 0, 9);
            circleFull.Fade(21374, 22395, 1, 1);




        }
    }
}
