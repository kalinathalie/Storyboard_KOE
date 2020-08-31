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
                var colorOne = int.Parse(colors.Split(',')[0], System.Globalization.CultureInfo.InvariantCulture);
                var colorTwo = int.Parse(colors.Split(',')[1], System.Globalization.CultureInfo.InvariantCulture);
                var colorOneSa = double.Parse(colors.Split(',')[2], System.Globalization.CultureInfo.InvariantCulture);
                var colorOneBra = double.Parse(colors.Split(',')[3], System.Globalization.CultureInfo.InvariantCulture);
                var colorTwoSa = double.Parse(colors.Split(',')[4], System.Globalization.CultureInfo.InvariantCulture);
                var colorTwoBra = double.Parse(colors.Split(',')[5], System.Globalization.CultureInfo.InvariantCulture);
                if(i%2==0)
                    bar.ColorHsb(startTime, colorOne, colorOneSa, colorOneBra);
                else
                    bar.ColorHsb(startTime, colorTwo, colorTwoSa, colorTwoBra);
                bar.Additive(startTime, endTime);
                bar.Rotate(startTime, MathHelper.DegreesToRadians(AngleRotation));

                bar.Fade((OsbEasing)6, endTime-tick(0, 1), endTime, 1, 0);

                var scaleX = Scale.X * barWidth / bitmap.Width;
                scaleX = (float)Math.Floor(scaleX * 10) / 10.0f;

                var hasScale = false;
                keyframes.ForEachPair(
                    (start, end) =>
                    {
                        hasScale = true;
                        bar.ScaleVec((OsbEasing)4, start.Time, end.Time,
                            scaleX, start.Value,
                            scaleX, end.Value);
                        bar.Move((OsbEasing)4, start.Time, end.Time,
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
                {"Airi's Extreme", "20,0,0.75,1,49,1"},
                {"Kalindraz's Ambivalence", "0,200,0,0.41,0.75,1"},
                {"LMT's Extreme", "255,65,0.66,1,0.37,0.98"},
                {"Maot's Insane", "60,120,0.41,0.99,0.46,0.87"},
                {"reicavera's Light Insane", "60,150,0.6,1,1,0.8"},
                {"ShadowX's Extreme", "180,300,1,1,1,1"},
                {"Cheri's Convalescence", "180,234,0.5,1,0.40,0.93"},
                {"Satellite's Expert", "180,300,1,1,1,1"},
                {"Nattu's Extra", "180,300,1,1,1,1"},
                {"Dada's Expert", "0,200,0,0.41,0.75,1"},
                {"Rensia's Hard", "359,263,0.38,0.96,0.98,0.51"},
                {"Hitsound", "180,300,1,1,1,1"},
                {"Ciyus Miapah's Extreme", "0,200,0,0.41,0.75,1"},
                {"Faito's Extra", "0,200,0,0.41,0.75,1"}
            };
            if (color.ContainsKey(diff_name)){
                return color[diff_name];
            }else{
                return "180,300,1,1,1,1";
            }
        }
        double tick(double start, double divisor){
            return Beatmap.GetTimingPointAt((int)start).BeatDuration / divisor;
        }
    }
}
