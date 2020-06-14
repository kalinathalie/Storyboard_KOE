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

namespace StorybrewScripts{
    public class FractalTriangle : StoryboardObjectGenerator{

        [Configurable]
        public string svgPath = "";

        [Configurable]
        public int startTime = 0;

        [Configurable]
        public int endTime = 0;

        [Configurable]
        public Vector2 position = Vector2.Zero;

        public override void Generate(){

            var Layer = GetLayer("FractalTriangle");
            var bitmap = GetMapsetBitmap(svgPath);
            var bg = Layer.CreateSprite(svgPath, OsbOrigin.TopCentre);
            bg.Scale(startTime, endTime, 0.1, 5);
            bg.Fade(startTime, endTime, 1, 1);
            bg.Move(startTime, endTime, new Vector2(320, 0), position);


		    
            
        }
    }
}
