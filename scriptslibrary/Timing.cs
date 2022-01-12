using OpenTK;
using System;
using StorybrewCommon.Mapset;

namespace Project.Util
{
    public class Timing
    {
        private Beatmap Beatmap;

        public Timing(Beatmap beatmap)
        {
            Beatmap = beatmap;
        }

        public double beat(double start){
            return Beatmap.GetTimingPointAt((int)start).BeatDuration;
        }
    }
}