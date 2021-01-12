using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StorybrewScripts
{
    public class Cursor : StoryboardObjectGenerator
    {[Configurable]
        public int StartTime = 0;

        [Configurable]
        public int EndTime = 0;

        [Configurable]
        public int BeatDivisor = 8;

        [Configurable]
        public int FadeTime = 200;

        [Configurable]
        public string Line = "sb/pixel.png";

        [Configurable]
        public string OutlinePath = "sb/q2.png";

        [Configurable]
        public string CursorPath = "sb/pl.png";

        [Configurable]
        public double SpriteScale = 1;

        private const int trailCount = 14;

        public override void Generate()
        {
            var hitobjectLayer = GetLayer("");
            var startPos = Beatmap.HitObjects.Where((o) => o.EndTime > StartTime).First().Position;

            ArrayList cursorTrail = new ArrayList();
            for(int i = 0; i < trailCount; i++)
            {
                var trail = hitobjectLayer.CreateSprite("sb/particle.png", OsbOrigin.Centre, startPos);

                //trail.Fade(StartTime, 0.33);
                trail.Additive(StartTime, EndTime);
                trail.Scale(StartTime, 1 - i*0.04);

                cursorTrail.Add(trail);
            }

            var hSprite = hitobjectLayer.CreateSprite(Line, OsbOrigin.Centre, new Vector2(320, startPos.Y));
            //var outline = hitobjectLayer.CreateSprite(OutlinePath, OsbOrigin.Centre, startPos);
            var cursor = hitobjectLayer.CreateSprite(CursorPath, OsbOrigin.Centre, startPos);

            hSprite.ScaleVec(StartTime, 854/2, 15);
            hSprite.Fade(StartTime,0.1);
            //outline.Scale(StartTime,0.04);
            //outline.Fade(StartTime,0.2);
            cursor.Scale(StartTime,SpriteScale);
            cursor.Additive(StartTime, EndTime);
            
            OsuHitObject prevObject = null;
            var circleArray = new List<int>();
            var lineArray = new List<int>();
            //circle and line
            circleArray.Add(StartTime);
            for(int j=0;j<=3;j+=1){
                double c = StartTime+tick(0,0.5) +j*tick(0, (double)1/(double)8);
                for(int m=1;m<=4;m+=1){
                    circleArray.Add((int)Math.Round(c));
                    lineArray.Add((int)Math.Round(c+tick(0,2)));
                    if(j==3 && m>=3) continue;
                    lineArray.Add((int)Math.Round(c+tick(0,2)*2));
                    c+=tick(0,(double)2/(double)3);
                }
            }
            
            foreach (OsuHitObject hitobject in Beatmap.HitObjects)
            {
                if ((StartTime != 0 || EndTime != 0) && 
                    (hitobject.StartTime < StartTime - 5 || EndTime - 5 <= hitobject.StartTime))
                    continue;
                if(circleArray.Contains((int)hitobject.StartTime) || circleArray.Contains((int)hitobject.StartTime+1) || circleArray.Contains((int)hitobject.StartTime -1)){
                    var dropCircle = hitobjectLayer.CreateSprite("sb/q2.png", OsbOrigin.Centre, hitobject.Position);
                    dropCircle.Scale(hitobject.StartTime, hitobject.StartTime+200, 0, 0.4);
                    dropCircle.Fade(hitobject.StartTime, hitobject.StartTime+200, 1, 0);
                }
                if(lineArray.Contains((int)hitobject.StartTime) || lineArray.Contains((int)hitobject.StartTime+1) || lineArray.Contains((int)hitobject.StartTime -1)){
                    var dropLine = hitobjectLayer.CreateSprite("sb/pl.png", OsbOrigin.Centre, hitobject.Position);
                    dropLine.ScaleVec(hitobject.StartTime, hitobject.StartTime+200, 8, 100, 1, 100);
                    dropLine.Rotate(hitobject.StartTime, MathHelper.DegreesToRadians(Random(-7,7)));
                    dropLine.Fade(hitobject.StartTime, hitobject.StartTime+200, 0.6, 0);
                    dropLine.Color(hitobject.StartTime, hitobject.Color);
                }
                if (prevObject != null) 
                {
                    hSprite.MoveY(prevObject.EndTime, hitobject.StartTime, prevObject.EndPosition.Y, hitobject.Position.Y);
                    //outline.Move(prevObject.EndTime, hitobject.StartTime, prevObject.EndPosition, hitobject.Position);
                    cursor.Move(prevObject.EndTime, hitobject.StartTime, prevObject.EndPosition, hitobject.Position);
                    //hSprite.Scale(OsbEasing.In, hitobject is OsuSlider ? hitobject.StartTime : prevObject.EndTime, hitobject.EndTime, SpriteScale, SpriteScale * 0.6);

                    for(int i = 0; i < trailCount; i++)
                    {
                        OsbSprite trail = (OsbSprite) cursorTrail[i];
                        trail.Move(prevObject.EndTime + (i+1)*4, hitobject.StartTime + (i+1)*4, prevObject.EndPosition, hitobject.Position);
                        trail.Color(hitobject.StartTime + (i+1)*4, hitobject.Color);
                        trail.Fade(hitobject.StartTime, 0.33);
                    }
                
                
                }
                cursor.Color(hitobject.StartTime, hitobject.Color);

                if (hitobject is OsuSlider)
                {
                    var timestep = Beatmap.GetTimingPointAt((int)hitobject.StartTime).BeatDuration / BeatDivisor;
                    var startTime = hitobject.StartTime;
                    var slider = (OsuSlider) hitobject;
                    Log(slider.Additions.ToString());
                    
                    
                    while (true)
                    {
                        var endTime = startTime + timestep;

                        var complete = hitobject.EndTime - endTime < 5;
                        if (complete) endTime = hitobject.EndTime;

                        var startPosition = hSprite.PositionAt(startTime);
                        var cursorPos = cursor.PositionAt(startTime);
                        hSprite.MoveY(startTime, endTime, startPosition.Y, hitobject.PositionAtTime(endTime).Y);
                        //outline.Move(startTime, endTime, startPosition, hitobject.PositionAtTime(endTime));
                        cursor.Move(startTime, endTime, cursorPos, hitobject.PositionAtTime(endTime));

                        for(int i = 0; i < trailCount; i++)
                        {
                            OsbSprite trail = (OsbSprite) cursorTrail[i];
                            trail.Move(startTime + (i+1)*4, endTime + (i+1)*4, cursorPos, hitobject.PositionAtTime(endTime));
                        }

                        if (complete) break;
                        startTime += timestep;
                    }
                }
                prevObject = hitobject;
            }
        }
        double tick(double start, double divisor){
            return Beatmap.GetTimingPointAt((int)start).BeatDuration / divisor;
        }
    }
}
