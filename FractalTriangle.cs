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
        public Vector2 initalPosition = Vector2.Zero;

        [Configurable]
        public Vector2 finalPosition = Vector2.Zero;

        public override void Generate(){

            var Layer = GetLayer("FractalTriangle");
            var bitmap = GetMapsetBitmap(svgPath);
            var triangle = Layer.CreateSprite(svgPath, OsbOrigin.TopCentre, new Vector2(320, 0));
            var big_triangle = Layer.CreateSprite(svgPath, OsbOrigin.TopCentre, new Vector2(320, 0));
            big_triangle.Rotate(startTime, MathHelper.DegreesToRadians(10));

            var colors = get_color(Beatmap.Name);
            var colorOne = int.Parse(colors.Split(',')[0]);
            var colorTwo = int.Parse(colors.Split(',')[1]);
            var colorOneSa = double.Parse(colors.Split(',')[2], System.Globalization.CultureInfo.InvariantCulture);
            var colorOneBra = double.Parse(colors.Split(',')[3], System.Globalization.CultureInfo.InvariantCulture);
            var colorTwoSa = double.Parse(colors.Split(',')[4], System.Globalization.CultureInfo.InvariantCulture);
            var colorTwoBra = double.Parse(colors.Split(',')[5], System.Globalization.CultureInfo.InvariantCulture);
            int i = 0;

            for(double x = startTime; x <= endTime; x+=tick(0,0.25)){
                big_triangle.Scale((OsbEasing)4, x, x+tick(0,0.25), 0.4, 0.8);
                if(i%2==0)
                    big_triangle.ColorHsb(x, colorOne, colorOneSa, colorOneBra);
                else
                    big_triangle.ColorHsb(x, colorTwo, colorTwoSa, colorTwoBra);
                big_triangle.Move((OsbEasing)4, x, x+tick(0,0.25), initalPosition, finalPosition);
                i+=1;
            }
        }

        double tick(double start, double divisor){
            return Beatmap.GetTimingPointAt((int)start).BeatDuration / divisor;
        }

        string get_color(string diff_name){
            Dictionary<string, string> color = new Dictionary<string, string>(){
                {"Airi's Extreme", "20,0,0.75,1,49,1"},
                {"Kalindraz's Ambivalence", "0,200,0,0.41,0.75,1"},
                {"LMT's Extreme", "255,65,0.66,1,0.37,0.98"},
                {"Maot's Insane", "60,120,0.41,0.99,0.46,0.87"},
                {"reicavera's Light Insane", "60,150,0.6,1,1,0.8"},
                {"ShadowX's Extreme", "180,300,1,1,1,1"},
                {"Cheri's Convalescence", "180,234,0.5,1,0.40,0.93"},
                {"Satellite's Expert", "180,300,1,1,1,1"},
                {"Nattu's Extra", "180,300,1,1,1,1"},
                {"Dada's Expert", "0,200,0,0.41,0.75,1"},
                {"Rensia's Hard", "359,263,0.38,0.96,0.98,0.51"},
                {"Hitsound", "180,300,1,1,1,1"},
                {"Ciyus Miapah's Extreme", "0,200,0,0.41,0.75,1"},
                {"Faito's Extra", "0,200,0,0.41,0.75,1"}
            };
            if (color.ContainsKey(diff_name)){
                return color[diff_name];
            }else{
                return "180,300,1,1,1,1";
            }
        }
    }
}
