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
    public class SectionThreeSetup : StoryboardObjectGenerator
    {

        // Basic setup for 42735 - 62840
        StoryboardLayer layer;
        double beatduration;
        OsbEasing outEase = OsbEasing.None;
        OsbEasing inEase = OsbEasing.InCirc;
        public override void Generate()
        {

            layer = GetLayer("Main");
            beatduration = Beatmap.GetTimingPointAt(62840).BeatDuration;

            var circleTransition = layer.CreateSprite("sb/circle.png", OsbOrigin.Centre, new Vector2(50,240));
            circleTransition.Scale(OsbEasing.InCirc,42264,42578,0,10);
            circleTransition.Fade(42264,42578+120,1,1);

            var bg = GetLayer("Background").CreateSprite("sb/gold/bg-tex.jpg");
            bg.Scale(42578, 854.0 / 3888);
            bg.Fade(42578,42578+120,0,1);
            bg.Fade(62840, 0);

            circleCircles("sb/gold/losange_thicker.png",42735,62840, new Vector2(320,240), 170, Math.PI, -Math.PI, 0.015, 8*6, true, 4);
            circleCircles("sb/gold/full.png",52787,62840, new Vector2(320,240), 200, Math.PI, -Math.PI, 0.007, 8*6, false, 4);
            circleCircles("sb/gold/circle_thick.png",53416,62840, new Vector2(320,240), 230, Math.PI, -Math.PI, 0.01, 8*6, true, 4);
            circleCircles("sb/gold/losange_thicker.png",54044,62840, new Vector2(320,240), 270, Math.PI, -Math.PI, 0.019, 8*6, false, 8);
            circleCircles("sb/gold/circle_thick.png",54672,62840, new Vector2(320,240), 320, Math.PI, -Math.PI, 0.019, 8*6, true, 4);
            circleCircles("sb/gold/full.png",55300,62840, new Vector2(320,240), 366, Math.PI, -Math.PI, 0.01, 8*6, false, 8);

            
        }

        public void circleCircles(String path, int startTime, int endTime, Vector2 position, int radius, double startAngle, double endAngle, double scale, double steps, bool up, int beatsToMove)
        {
            int cpt = 0;
            bool big = true;
            //Initial Circles

            for(double angle = startAngle; angle > endAngle; angle -= (startAngle - endAngle)/steps)
            {
                oneCircle(path, startTime, endTime, cpt*20, position, radius, startAngle, angle, endAngle, scale, steps, up, big, beatsToMove, cpt);
                big = !big;
                cpt++;
            }
        }

        //Creates one element of a circle chain
        public void oneCircle(String path,int startTime, int endTime, int fadeOffset, Vector2 position, int radius, double startAngle, double initialAngle, double endAngle, double scale, double steps, bool up,  bool big, int beatsToMove, int cpt)
        {
            double angleOffset = (startAngle - endAngle)/steps;


            var circle = layer.CreateSprite(path, OsbOrigin.Centre, new Vector2((float) (position.X + radius*Math.Cos(initialAngle)),(float) (position.Y + radius*Math.Sin(initialAngle))));
            circle.Fade(startTime + fadeOffset - 70, startTime + fadeOffset+500 , 0, 1);
            circle.Fade(endTime, 0);
            circle.Scale(startTime+fadeOffset, scale);
            
            for(double time = startTime; time < endTime-5; time += beatsToMove*beatduration)
            {
                if((initialAngle <= startAngle) && (initialAngle > endAngle))
                {
                    if(up)
                    {
                        circle.Move(outEase, time, time+beatsToMove*beatduration, (position.X + radius*Math.Cos(initialAngle)),(float) (position.Y + radius*Math.Sin(initialAngle)), (position.X + radius*Math.Cos(initialAngle + angleOffset)),(float) (position.Y + radius*Math.Sin(initialAngle + angleOffset)));
                        circle.Rotate(outEase, time, time+beatsToMove*beatduration, initialAngle - Math.PI/2, initialAngle+angleOffset - Math.PI/2);
                    }
                    else 
                    {
                        circle.Rotate(outEase, time, time+beatsToMove*beatduration, initialAngle - Math.PI/2, initialAngle-angleOffset - Math.PI/2);
                        circle.Move(outEase, time, time+beatsToMove*beatduration, (position.X + radius*Math.Cos(initialAngle)),(float) (position.Y + radius*Math.Sin(initialAngle)), (position.X + radius*Math.Cos(initialAngle - angleOffset)),(float) (position.Y + radius*Math.Sin(initialAngle - angleOffset)));

                    }

                    /*if(cpt == 0)
                    {
                        circle.Fade(time, time+beatsToMove*beatduration,1,0);
                    }

                    if(cpt == steps)
                    {
                        circle.Fade(time, time+beatsToMove*beatduration,0,1);
                    }*/
    
                }
            }
        }
    }
}
