using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Animations;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;

namespace StorybrewScripts
{
    /// <summary>
    /// An example of a spectrum effect.
    /// </summary>
    public class DoubleSpectrum : StoryboardObjectGenerator
    {
        [Configurable]
        public int StartTime = 0;

        [Configurable]
        public int EndTime = 10000;

        [Configurable]
        public Vector2 Position = new Vector2(-107, 240);

        [Configurable]
        public float Width = 844;

        [Configurable]
        public int BeatDivisor = 16;

        [Configurable]
        public int BarCount = 96;

        [Configurable]
        public string SpritePath = "sb/bar.png";

        [Configurable]
        public OsbOrigin SpriteOrigin = OsbOrigin.CentreLeft;

        [Configurable]
        public Vector2 Scale = new Vector2(1, 200);

        [Configurable]
        public int LogScale = 600;

        [Configurable]
        public double Tolerance = 0.2;

        [Configurable]
        public int CommandDecimals = 1;

        [Configurable]
        public float MinimalHeight = 0.05f;

        [Configurable]
        public OsbEasing FftEasing = OsbEasing.InExpo;

        public override void Generate()
        {

            var endTime = Math.Min(EndTime, (int)AudioDuration);
            var startTime = Math.Min(StartTime, endTime);
            var bitmap = GetMapsetBitmap(SpritePath);

            var heightKeyframes = new KeyframedValue<float>[BarCount];
            for (var i = 0; i < BarCount; i++)
                heightKeyframes[i] = new KeyframedValue<float>(null);

            var fftTimeStep = Beatmap.GetTimingPointAt(startTime).BeatDuration / BeatDivisor;
            var fftOffset = fftTimeStep * 0.2;
            for (var time = (double)startTime; time < endTime; time += fftTimeStep)
            {
                var fft = GetFft(time + fftOffset, BarCount, null, FftEasing);
                for (var i = 0; i < BarCount; i++)
                {
                    var height = (float)Math.Log10(1 + fft[i] * LogScale) * Scale.Y / bitmap.Height;
                    if (height < MinimalHeight) height = MinimalHeight;

                    heightKeyframes[i].Add(time, height);
                }
            }

            var layer = GetLayer("Spectrum");
            var barWidth = Width / BarCount;
            for (var i = 0; i < BarCount; i++)
            {
                var keyframes = heightKeyframes[i];
                keyframes.Simplify1dKeyframes(Tolerance, h => h);

                var bar = layer.CreateSprite(SpritePath, SpriteOrigin, new Vector2(Position.X + i * barWidth, Position.Y));
                bar.Fade(startTime - 100 - i*10, startTime - i*10, 0, 1);
                bar.Fade(startTime, endTime, 1, 1);


                OsbSprite bar2 = new OsbSprite();
                if(i != 0)
                {
                    bar2 = layer.CreateSprite(SpritePath, SpriteOrigin, new Vector2(Position.X - i * barWidth, Position.Y));
                    bar2.Fade(startTime, endTime, 1, 1);
                    bar2.Fade(startTime - 100 - i*10, startTime - i*10, 0, 1);
                }
                
                /*bar.Additive(startTime, endTime);
                bar2.Additive(startTime, endTime);*/

                var scaleX = Scale.X * barWidth / bitmap.Width;
                scaleX = 0.01f;

                var hasScale = false;
                keyframes.ForEachPair(
                    (start, end) =>
                    {
                        hasScale = true;
                        bar.ScaleVec(start.Time, end.Time,
                            scaleX, start.Value,
                            scaleX, end.Value);
                        if(i != 0)
                        {
                            bar2.ScaleVec(start.Time, end.Time,
                                scaleX, start.Value,
                                scaleX, end.Value);
                        }
                        
                    },
                    MinimalHeight,
                    s => (float)Math.Round(s, CommandDecimals)
                );
                if (!hasScale) 
                {
                    bar.ScaleVec(startTime, scaleX, MinimalHeight);
                    if(i != 0)
                        {
                            bar2.ScaleVec(startTime, scaleX, MinimalHeight);
                        }
                    
                }
            }
        }
    }
}
