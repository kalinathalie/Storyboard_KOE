using OpenTK;
using StorybrewCommon.Animations;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;
using System.Collections.Generic;

namespace StorybrewScripts
{
    /// <summary>
    /// An example of a spectrum effect.
    /// </summary>
    public class Spectrum2D : StoryboardObjectGenerator
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
        public int AngleRotation = 0;

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

            var layer = GetLayer("Spectrum2D");
            var barWidth = Width / BarCount;
            for (var i = 0; i < BarCount; i++)
            {
                var keyframes = heightKeyframes[i];
                keyframes.Simplify1dKeyframes(Tolerance, h => h);

                var bar = layer.CreateSprite(SpritePath, SpriteOrigin, new Vector2(Position.X + i * barWidth, Position.Y));
                var colors = get_color(Beatmap.Name);
                var colorStart = int.Parse(colors.Split(',')[0]);
                var colorEnd = int.Parse(colors.Split(',')[1]);
                if(i%2==0)
                    bar.ColorHsb(startTime, endTime, colorStart, 1, 1, colorEnd, 1, 1);
                else
                    bar.ColorHsb(startTime, endTime, colorEnd, 1, 1, colorStart, 1, 1);
                bar.Additive(startTime, endTime);
                bar.Rotate(startTime, MathHelper.DegreesToRadians(30));

                bar.Fade((OsbEasing)6, endTime-tick(0, 1), endTime, 1, 0);

                var scaleX = Scale.X * barWidth / bitmap.Width;
                scaleX = (float)Math.Floor(scaleX * 10) / 10.0f;

                var hasScale = false;
                keyframes.ForEachPair(
                    (start, end) =>
                    {
                        hasScale = true;
                        bar.ScaleVec(start.Time, end.Time,
                            scaleX, start.Value,
                            scaleX, end.Value);
                        bar.Move(start.Time, end.Time,
                            new Vector2(Position.X + i * barWidth+(start.Value*2.5f), Position.Y-(start.Value*4.33f)),
                            new Vector2(Position.X + i * barWidth+(end.Value*2.5f), Position.Y-(end.Value*4.33f)));
                    },
                    MinimalHeight,
                    s => (float)Math.Round(s, CommandDecimals)
                );
                if (!hasScale) bar.ScaleVec(startTime, scaleX, MinimalHeight);
            }
        }

        string get_color(string diff_name){
            Dictionary<string, string> color = new Dictionary<string, string>(){
                {"Airi's Extreme", "180,300"},
                {"Kalindraz's Ambivalence", "180,300"},
                {"LMT's Extreme", "180,300"},
                {"Maot's Insane", "180,300"},
                {"reicavera's Light Insane", "180,300"},
                {"ShadowX's Extra", "180,300"},
                {"Cheri's Extra", "180,300"},
                {"Satellite's Expert", "180,300"},
                {"Nattu's Extra", "180,300"},
                {"Dada's Insane", "180,300"},
                {"Rensia's Hard", "180,300"},
                {"Hitsound", "180,300"}
            };
            if (color.ContainsKey(diff_name)){
                return color[diff_name];
            }else{
                return "180,300";
            }
        }
        double tick(double start, double divisor){
            return Beatmap.GetTimingPointAt((int)start).BeatDuration / divisor;
        }
    }
}
