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
    public class DancingCircles : StoryboardObjectGenerator
    {
        StoryboardLayer layer;
        double beatduration;

        OsbEasing outEase = OsbEasing.OutCirc;
        OsbEasing inEase = OsbEasing.InCirc;

        public override void Generate()
        {
		    layer = GetLayer("Main");
            beatduration = Beatmap.GetTimingPointAt(22630).BeatDuration;
            Vector2 pos = new Vector2(50,240);

            circleCircles("sb/circle.png",22630, 42735, pos, 270, Math.PI/2, -Math.PI/2, 0.11, 14, true, true, 1);
            circleCircles("sb/circle.png",22630, 42735, pos, 310, Math.PI/2, -Math.PI/2, 0.06, 23, false, false, 1);
            circleCircles("sb/to.png",    22630, 42735, pos, 420, Math.PI/3, -Math.PI/3, 0.05, 15, false, false, 1);
            circleCircles("sb/q2.png",    22630, 42735, pos, 365, Math.PI/3, -Math.PI/3, 0.04, 16, true, false, 2);
            circleCircles("sb/circle.png",22630, 42735, pos, 500, Math.PI/4, -Math.PI/4, 0.33, 6 , false, false, 2);

            //circleCircles("sb/gold/losange_thicker.png",42735,62840, new Vector2(320,240), 100, Math.PI/2, -Math.PI/2, 0.01, 24, true, false, 1);

            
        }


        // Create a circle of circles, with different parrameters
        public void circleCircles(String path, int startTime, int endTime, Vector2 position, int radius, double startAngle, double endAngle, double scale, double steps, bool up, bool wiggle, int beatsToMove)
        {
            int cpt = 0;
            bool big = true;
            //Initial Circles

            for(double angle = startAngle; angle > endAngle; angle -= (startAngle - endAngle)/steps)
            {
                oneCircle(path, startTime, endTime, cpt*10, position, radius, startAngle, angle, endAngle, scale, steps, up, wiggle, big, beatsToMove);
                big = !big;
                cpt++;
            }
        }

        //Creates one element of a circle chain
        public void oneCircle(String path,int startTime, int endTime, int fadeOffset, Vector2 position, int radius, double startAngle, double initialAngle, double endAngle, double scale, double steps, bool up, bool wiggle, bool big, int beatsToMove)
        {
            double angleOffset = (startAngle - endAngle)/steps;


            var circle = layer.CreateSprite(path, OsbOrigin.Centre, new Vector2((float) (position.X + radius*Math.Cos(initialAngle)),(float) (position.Y + radius*Math.Sin(initialAngle))));
            circle.Fade(startTime - fadeOffset - 70, startTime - fadeOffset , 0, 1);
            circle.Fade(endTime, 0);
            circle.Scale(startTime+fadeOffset, scale);
            
            for(double time = startTime; time < endTime-5; time += beatsToMove*beatduration)
            {
                if((initialAngle <= startAngle) && (initialAngle > endAngle))
                {
                    if(up)
                    {
                        circle.Move(outEase, time, time+150, (position.X + radius*Math.Cos(initialAngle)),(float) (position.Y + radius*Math.Sin(initialAngle)), (position.X + radius*Math.Cos(initialAngle + angleOffset)),(float) (position.Y + radius*Math.Sin(initialAngle + angleOffset)));
                    }
                    else 
                    {
                        circle.Move(outEase, time, time+150, (position.X + radius*Math.Cos(initialAngle)),(float) (position.Y + radius*Math.Sin(initialAngle)), (position.X + radius*Math.Cos(initialAngle - angleOffset)),(float) (position.Y + radius*Math.Sin(initialAngle - angleOffset)));

                    }
                    if(wiggle)
                    {
                        if(big)
                        {
                            circle.Scale(outEase, time, time+150, 0.75*scale, 1.25*scale);
                        }
                        else
                        {
                            circle.Scale(outEase, time, time+150, 1.25*scale, 0.75*scale);
                        }
                    }
    
                }
            }
        }
    }
}
