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
    public class BasicParticlesParallax : StoryboardObjectGenerator
    {
        // A simpler and better particle script
        [Configurable]
        public double scaleMagnitude = 0.05;

        [Configurable]
        public String spritePath = "sb/pl.png";

        [Configurable]
        public int startTime = 0;

        [Configurable]
        public int endTime = 100000;

        [Configurable]
        public int particleAmountPerBeat = 15; 

        [Configurable]
        public int xStart = -107;

        [Configurable]
        public int xEnd = 400;


        // Attribues
        StoryboardLayer layer;
        double beatDuration;

        public override void Generate()
        {
            layer = GetLayer("Background");
            beatDuration = Beatmap.GetTimingPointAt(startTime).BeatDuration;

            // Smaller Particles in the background
		    for(double t = startTime; t < endTime-5; t+=4*beatDuration)
            {
                for(int cpt = 0; cpt < particleAmountPerBeat; cpt++)
                {
                    var sprite = layer.CreateSprite(spritePath,OsbOrigin.Centre);
                    sprite.Scale(t,Random(1,10)*0.33*scaleMagnitude);

                    var xPos = Random(xStart,xEnd);
                    var yPos = Random(0,480);
                    var randomTimeShift = Random(-600,600);
                    sprite.Move(t+randomTimeShift,t+4*beatDuration+randomTimeShift,xPos,yPos,xPos+Random(10,50),yPos-Random(60,240));
                    sprite.Fade(t+randomTimeShift,t+200,0,1);
                    sprite.Fade(t+4*beatDuration-100-Random(100,200),t+4*beatDuration+randomTimeShift,1,0);
                    sprite.Additive(t+randomTimeShift,t+4*beatDuration+randomTimeShift);
                }
            }
            
        }
    }
}
