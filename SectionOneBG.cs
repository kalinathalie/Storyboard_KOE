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
    public class SectionOneBG : StoryboardObjectGenerator
    {

        StoryboardLayer layer;
        double beatduration;
        public override void Generate()
        {
            layer = GetLayer("Main");
            beatduration = Beatmap.GetTimingPointAt(22630).BeatDuration;


            for(double time = 18861; time < 42730; time += beatduration)
            {
                createSquare("sb/rotPlusSmol.png", time, 0*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 1*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 2*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 3*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 4*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 5*Math.PI/6);

                createSquare("sb/rotPlusSmol.png", time, 6*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 7*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 8*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 9*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 10*Math.PI/6);
                createSquare("sb/rotPlusSmol.png", time, 11*Math.PI/6);
            }
		    

        }

        public void createSquare(String path, double time, double initialAngle)
        {
            var sprite = layer.CreateSprite(path);
            // sprite.Fade(time-1,0);
            bool first = true;
            int cpt = 0;
            double sf = 1.3*480.0/1080;
            for(double t = time; t < 42730; t += beatduration)
            {
                if((t>22552)&&(first))
                {
                    sprite.Fade(t + cpt*6,t+75+cpt*6,0,0.66);
                    first = false;
                }
                if(cpt<13)
                {
                    sprite.Scale(OsbEasing.OutCirc, t, t + 250, sf*cpt*0.1, sf*(cpt+1)*0.1);
                    sprite.Rotate(OsbEasing.OutCirc, t, t + 250, initialAngle + Math.PI/2.0 - cpt*Math.PI/48, initialAngle + Math.PI/2.0 - (cpt+1)*Math.PI/48);
                    cpt++;
                }
            }
        }

        
    }
}
