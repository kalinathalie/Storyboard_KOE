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
    public class Parallax : StoryboardObjectGenerator
    {
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
        public OsbEasing EasingType = OsbEasing.InOutCubic;

        public override void Generate(){

            var BgPath = get_bg(Beatmap.Name);
            var hitobjectLayer = GetLayer("Parallax");

            //Get middle position
            var middle = new Vector2(320, 240);
            var bitmap = GetMapsetBitmap(BgPath);

            //Create bg Sprite
            var bg = hitobjectLayer.CreateSprite(BgPath, OsbOrigin.Centre, middle);

            //Scale it to make it bigger than the screen
            bg.Scale(StartTime, 550.0f / bitmap.Height);

            //Set fading options
            bg.Fade(StartTime, Opacity);
            bg.Fade(EndTime - FadeTime, EndTime, Opacity, 0);

            //Define LastObject Position and EndTime
            float LastObjectEndingTime = StartTime;
            Vector2 LastObjectPosition = middle;


            //Loop that passes all the hitobjetcs of the beatmap
            foreach (var hitobject in Beatmap.HitObjects)
            {
                //Ignore objects before and after StartTime and EndTime
                if ((StartTime != 0 || EndTime != 0) && (hitobject.StartTime < StartTime - 5 || EndTime - 5 <= hitobject.StartTime)) continue;

                    //Getting HitObject position
                    var hoPosition = hitobject.Position;

                    //Calling method that calculates the relative position to the middle
                    var newPosition = CalculateDistanceFromMiddle(middle, hoPosition, intensity);
                    
                    //Move the background using all the information above
                    bg.Move(EasingType, LastObjectEndingTime, hitobject.StartTime,  LastObjectPosition, newPosition);
                    
                    //Save last HitObject position
                    LastObjectPosition = newPosition;

                //Check if HitObject is Slider.
                if (hitobject is OsuSlider)
                {
                    //Getting timestep from one beat to another, based on BeatDivisor and HitObject start time.
                    var timestep = Beatmap.GetTimingPointAt((int)hitobject.StartTime).BeatDuration / BeatDivisor;
                    var startTime = (float)hitobject.StartTime;

                    //Infinite loop that completes when SliderEnd is reached.
                    while (true)
                    {
                        //Get EndTime of this step on loop.
                        var endTime = startTime + timestep;

                        //Check if is has reacked SliderEnd
                        var complete = hitobject.EndTime - endTime < 5;
                        if (complete) endTime = hitobject.EndTime;

                        //Get sliderBallPosition before this step
                        var lastSliderPosition = CalculateDistanceFromMiddle(middle, hitobject.PositionAtTime(startTime), intensity);
                        
                        //Get current sliderBallPosition and calcule its position relative to middle
                        hoPosition = hitobject.PositionAtTime(endTime);
                        newPosition = CalculateDistanceFromMiddle(middle, hoPosition, intensity);

                        //Move the background using all the information above
                        bg.Move(EasingType, startTime, endTime, lastSliderPosition, newPosition);

                        //Set lastSliderPosition and LastObjectPosition
                        lastSliderPosition = newPosition;
                        LastObjectPosition = newPosition;

                        //Break loop if slider is over.
                        if (complete) break;

                        //Set new startTime to the next step.
                        startTime += (float)timestep;
                    }
                }
                //Saving last HitObject endtime to use it in the next background movement.
                LastObjectEndingTime = (float)hitobject.EndTime;
            }
        }

        string get_bg(string diff_name){
            Dictionary<string, string> background = new Dictionary<string, string>(){
                {"Airi's Extreme", "Sakura.jpg"},
                {"Kalindraz's Ambivalence", "Kalindraz.jpg"},
                {"LMT's Extreme", "LMT.jpg"},
                {"Maot's Insane", "Maot.jpg"},
                {"reicavera's Light Insane", "reicavera.jpg"},
                {"ShadowX's Extra", "ShadowX.png"},
                {"Cheri's Extra", "Cheri.jpg"},
                {"Satellite's Expert", "Satellite.jpg"},
                {"Nattu's Extra", "Nattu.jpg"},
                {"Dada's Insane", "Dada.jpg"},
                {"Hitsound", "Hitsounding.png"}
            };
            if (background.ContainsKey(diff_name)){
                return background[diff_name];
            }else{
                return "K4L1.jpg";
            }
        }


        //Get hitobject position and compare its distance from the middle
        private Vector2 CalculateDistanceFromMiddle(Vector2 middle, Vector2 hoPosition, float intensity) {
            var distanceFromMiddleX = (hoPosition.X - middle.X);
            var distanceFromMiddleY = (hoPosition.Y - middle.Y);
            var movePercentage = new Vector2(distanceFromMiddleX/427, distanceFromMiddleY/240);
            var NewPosition = new Vector2(middle.X - (movePercentage.X * intensity),  middle.Y - (movePercentage.Y * intensity));
            return NewPosition;
        }
    }
}
