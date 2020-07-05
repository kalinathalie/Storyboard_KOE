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
    public class Parallax : StoryboardObjectGenerator{
        
        [Configurable]
        public int StartTime = 0;

        [Configurable]
        public int EndTime = 0;

        [Configurable]
        public int BeatDivisor = 8;

        [Configurable]
        public int FadeTime = 200;        
        
        [Configurable]

        public float Opacity = 1f;

        [Configurable]
        public float intensity = 1;      

        [Configurable]
        public string FlashPath = "";  

        [Configurable]
        public OsbEasing EasingType = OsbEasing.InOutCubic;

        public override void Generate(){

            var BgPath = get_bg(Beatmap.Name);
            var hitobjectLayer = GetLayer("Parallax");

            var middle = new Vector2(320, 240);
            var bitmap = GetMapsetBitmap(BgPath);

            var bg = hitobjectLayer.CreateSprite(BgPath, OsbOrigin.Centre, middle);

            //Pulse
            var pulse = hitobjectLayer.CreateSprite(BgPath, OsbOrigin.Centre, middle);
            pulse.Scale(StartTime, StartTime+tick(0,0.25), 480.0f / bitmap.Height, 550.0f / bitmap.Height);
            pulse.Fade(StartTime, StartTime+tick(0,0.25), 0.7, 0);

            //Flash
            var flash = hitobjectLayer.CreateSprite(FlashPath, OsbOrigin.Centre);
            for(double i = StartTime; i<=EndTime-10; i+=tick(0,0.125)){
                flash.Fade((OsbEasing)10, i, i+tick(0,0.125), 0.4, 0);
            }


            bg.Scale(StartTime+tick(0,0.25), 550.0f / bitmap.Height);

            bg.Fade(StartTime, Opacity);
            bg.Fade(EndTime - FadeTime, EndTime, Opacity, 0);

            float LastObjectEndingTime = StartTime;
            Vector2 LastObjectPosition = middle;


            foreach (var hitobject in Beatmap.HitObjects){

                if ((StartTime != 0 || EndTime != 0) && (hitobject.StartTime < StartTime - 5 || EndTime - 5 <= hitobject.StartTime)) continue;

                    var hoPosition = hitobject.Position;

                    var newPosition = CalculateDistanceFromMiddle(middle, hoPosition, intensity);
                    
                    bg.Move(EasingType, LastObjectEndingTime, hitobject.StartTime,  LastObjectPosition, newPosition);
                    
                    LastObjectPosition = newPosition;

                if (hitobject is OsuSlider){

                    var timestep = Beatmap.GetTimingPointAt((int)hitobject.StartTime).BeatDuration / BeatDivisor;
                    var startTime = (float)hitobject.StartTime;

                    while (true){
                        
                        var endTime = startTime + timestep;

                        var complete = hitobject.EndTime - endTime < 5;
                        if (complete) endTime = hitobject.EndTime;

                        var lastSliderPosition = CalculateDistanceFromMiddle(middle, hitobject.PositionAtTime(startTime), intensity);
                        
                        hoPosition = hitobject.PositionAtTime(endTime);
                        newPosition = CalculateDistanceFromMiddle(middle, hoPosition, intensity);

                        bg.Move(EasingType, startTime, endTime, lastSliderPosition, newPosition);

                        lastSliderPosition = newPosition;
                        LastObjectPosition = newPosition;

                        if (complete) break;

                        startTime += (float)timestep;
                    }
                }
                LastObjectEndingTime = (float)hitobject.EndTime;
            }
        }

        string get_bg(string diff_name){
            Dictionary<string, string> background = new Dictionary<string, string>(){
                {"Airi's Extreme", "Airi"},
                {"Kalindraz's Ambivalence", "Kalindraz"},
                {"LMT's Extreme", "LMT"},
                {"Maot's Insane", "Maot"},
                {"reicavera's Light Insane", "reicavera"},
                {"ShadowX's Extra", "ShadowX"},
                {"Cheri's Convalescence", "Cheri"},
                {"Satellite's Extra", "Satellite"},
                {"Nattu's Extra", "Nattu"},
                {"Dada's Insane", "Dada"},
                {"Hitsound", "Hitsound"},
                {"Rhuzerv", "K4L1"},
                {"Advanced", "K4L1"},
                {"Rensia's Hard", "Rensia"},
                {"Faito's Extra", "Faito"},
                {"Ciyus Miapah's Extreme", "Ciyus"}
            };
            if (background.ContainsKey(diff_name)){
                return "sb/bgs/"+background[diff_name]+"/"+background[diff_name]+".jpg";
            }else{
                Log($"{diff_name}");
                return "Fail";
            }
        }
        double tick(double start, double divisor){
            return Beatmap.GetTimingPointAt((int)start).BeatDuration / divisor;
        }


        private Vector2 CalculateDistanceFromMiddle(Vector2 middle, Vector2 hoPosition, float intensity) {
            var distanceFromMiddleX = (hoPosition.X - middle.X);
            var distanceFromMiddleY = (hoPosition.Y - middle.Y);
            var movePercentage = new Vector2(distanceFromMiddleX/427, distanceFromMiddleY/240);
            var NewPosition = new Vector2(middle.X - (movePercentage.X * intensity),  middle.Y - (movePercentage.Y * intensity));
            return NewPosition;
        }
    }
}
