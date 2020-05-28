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
    public class PartcilesPointsAndLines : StoryboardObjectGenerator{

        [Configurable]
        public int ParticleQuantity = 0;

        [Configurable]
        public int Speed = 0;

        public override void Generate(){
		    
            
        }
    }
}
