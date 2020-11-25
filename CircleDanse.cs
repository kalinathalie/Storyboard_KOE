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

    public class CircleC {
        public List<OsbSprite> circles;
        public Vector2 position;
        private int circleNum = 0;
        public CircleC() {}
        public CircleC(Vector2 pos,int numberOfCircles, int radius, StoryboardLayer layer, int time) {
            
            circles  = new List<OsbSprite>();
            for(double tet = 0 ; tet < 360 ; tet += 360.0/ numberOfCircles)
            {
                var circle = layer.CreateSprite("sb/pl.png", OsbOrigin.Centre, new Vector2(pos.X + (float) (radius*Math.Cos(tet / 180.0 * Math.PI)),pos.Y - (float) (radius*Math.Sin(tet / 180.0 * Math.PI))));
                circle.Scale(time, (8.0/numberOfCircles) * radius*0.012);
                circle.Fade(time,0);
                circles.Add(circle);
            }

            circleNum = numberOfCircles;
        }

        public void fadeIn(int time, int duration, double startFade, double endFade)
        {
            foreach(OsbSprite sp in circles)
            {
                sp.Fade(time, time+duration, startFade, endFade);
            }
        }

        public void circleFade(int time, int fadeDuration,double delay, int numberOfTurns)
        {
            int cpt = 0;
            for(int i = 0; i < numberOfTurns+0; i++)
            {
                cpt = 0;
                foreach(OsbSprite sp in circles)
                {
                    sp.Fade(OsbEasing.None,time + i*circleNum*delay + cpt*delay, time + fadeDuration + i*circleNum*delay + cpt*delay, 1, 0);
                    cpt++;
                }
            }

        }

    }
    public class CircleDanse : StoryboardObjectGenerator
    {
        StoryboardLayer layer;
        double beatduration;
        public override void Generate()
        {
		    layer = GetLayer("Main");
            beatduration = Beatmap.GetTimingPointAt(12578).BeatDuration;
            var circleC1 = new CircleC(new Vector2(320,240), 32, 60, layer, 1269);

            circleC1.circleFade(12578,500,0.125*beatduration,6);
            
        }
    }
}
