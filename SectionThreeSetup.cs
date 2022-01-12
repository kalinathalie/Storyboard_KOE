using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Animations;
using System.Drawing;
using System.Drawing.Imaging;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StorybrewScripts
{
    public class SectionThreeSetup : StoryboardObjectGenerator
    {

        // Basic setup for 42735 - 62840

        [Configurable]
        public Color4 Color = new Color4(240 / 255f, 234 / 255f, 100 / 255f, 1);

        [Configurable]
        public bool Additive = false;

        [Configurable]
        public Color4 BlurColor = new Color4(240 / 255f, 234 / 255f, 100 / 255f, 1);

        [Configurable]
        public int ScaleStart = 54672;

        [Configurable]
        public int ScaleEnd = 57814;
        
        [Configurable]
        public OsbEasing ScaleEasing = OsbEasing.InOutQuad;

        [Configurable]
        public int BlurRadius = 6;

        [Configurable]
        public double BlurPower = 5;

        StoryboardLayer layer;

        double beatduration;

        OsbEasing MovementEasing = OsbEasing.None;

        public double lerp(double start, double end, double t)
        {
            return start + (end - start) * t;
        }

        public override void Generate()
        {

            layer = GetLayer("Main");
            beatduration = Beatmap.GetTimingPointAt(62840).BeatDuration;

            var circleTransition = layer.CreateSprite("sb/circle.png", OsbOrigin.Centre, new Vector2(50,240));
            circleTransition.Scale(OsbEasing.InCirc,42264,42578,0,10);
            circleTransition.Fade(42264,42578+120,1,1);

            String losange_thicker_blurred = "sb/gold/blur/losange_thicker.png";
            String full_blurred = "sb/gold/blur/full.png";
            String circle_thick_blurred = "sb/gold/blur/circle_thick.png";

            circleCircles("sb/gold/losange_thicker.png", losange_thicker_blurred,42735, 62840, new Vector2(320,240), 170, Math.PI / 2, -3 * Math.PI / 2, 0.015, 8*6, true, 4, -30);
            circleCircles("sb/gold/full.png", full_blurred, 52787, 62840, new Vector2(320,240), 200, 3 * Math.PI / 2, -Math.PI / 2, 0.007, 8*6, false, 4, -15);
            circleCircles("sb/gold/circle_thick.png", circle_thick_blurred, 53416,62840, new Vector2(320,240), 230, Math.PI / 3, -5 * Math.PI / 3, 0.01, 8*6, true, 4, 7.5);
            circleCircles("sb/gold/losange_thicker.png", losange_thicker_blurred, 54044, 62840, new Vector2(320,240), 270, 2 * Math.PI / 3, -4 * Math.PI / 3, 0.019, 8*6, false, 4, 40);
            circleCircles("sb/gold/circle_thick.png", circle_thick_blurred, 54672, 62840, new Vector2(320,240), 320, 4 * Math.PI / 3, -2 * Math.PI / 3, 0.019, 8*6, true, 4, 80);
            circleCircles("sb/gold/full.png", full_blurred, 55300, 62840, new Vector2(320,240), 366, 5 * Math.PI / 3, -Math.PI / 3, 0.01, 8*6, false, 4, 160);

            
        }

        public String blur(String path, int radius, double power)
        {
            var bitmap = GetMapsetBitmap(path);
            var blurredBitmap = BitmapHelper.Blur(bitmap, radius, power);
            String directory = Path.Combine(MapsetPath, Path.GetDirectoryName(path), "blur");
            if (!System.IO.Directory.Exists(directory)) System.IO.Directory.CreateDirectory(directory);
            String filename = Path.Combine(directory, Path.GetFileName(path));
            blurredBitmap.Bitmap.Save(filename, ImageFormat.Png);
            return filename;
        }

        public void circleCircles(String path, String blurredPath, int startTime, int endTime, Vector2 position, int radius, double startAngle, double endAngle, double scale, double steps, bool up, int beatsToMove, double scaleDistance)
        {
            int cpt = 0;
            bool big = true;
            //Initial Circles

            for(double angle = startAngle; angle > endAngle + 0.001; angle -= (startAngle - endAngle) / steps)
            {
                oneCircle(path, blurredPath, startTime, endTime, cpt*20, position, radius, startAngle, angle, endAngle, scale, steps, up, big, beatsToMove, cpt, scaleDistance);
                big = !big;
                cpt++;
            }
        }

        //Creates one element of a circle chain
        public void oneCircle(String path, String blurredPath, int startTime, int endTime, int fadeOffset, Vector2 position, double radius, double startAngle, double initialAngle, double endAngle, double scale, double steps, bool up,  bool big, int beatsToMove, int cpt, double scaleDistance)
        {
            double angleOffset = (startAngle - endAngle) / steps;

            double angle = initialAngle;

            double radius1 = radius;
            double radius2 = radius;

            Vector2 initialPosition = new Vector2((float) (position.X + radius * Math.Cos(initialAngle)), (float) (position.Y + radius * Math.Sin(initialAngle)));

            var circle = layer.CreateSprite(path, OsbOrigin.Centre, initialPosition);
            circle.Color(startTime, Color);

            //var blurredCircle = layer.CreateSprite(blurredPath, OsbOrigin.Centre, initialPosition);
            //blurredCircle.Color(startTime, BlurColor);

            double fadeStart = startTime + fadeOffset - 70;
            double fadeEnd = startTime + fadeOffset + 500;
            circle.Fade(
                fadeStart,
                fadeEnd,
                0, Color.A
            );
            circle.Fade(endTime - 2 * beatduration, endTime, Color.A, 0);

            /*
            // crossfade command overlap untanglement logic
            if (fadeStart < ScaleStart)
            {
                circle.Fade(
                    fadeStart,
                    fadeEnd >= ScaleStart ? ScaleStart : fadeEnd,
                    0, fadeEnd >= ScaleStart ? Color.A * (ScaleStart - fadeStart) / (fadeEnd - fadeStart) : Color.A
                );
                circle.Fade(
                    ScaleEasing,
                    fadeEnd >= ScaleStart ? ScaleStart : fadeEnd,
                    ScaleEnd,
                    fadeEnd >= ScaleStart ? Color.A * (ScaleStart - fadeStart) / (fadeEnd - fadeStart) : Color.A, 0
                );
            }
            else
            {
                circle.Fade(
                    ScaleEasing,
                    fadeStart,
                    (fadeStart + fadeEnd) / 2,
                    Color.A / 2, 0
                );
                circle.Fade(
                    ScaleEasing,
                    (fadeStart + fadeEnd) / 2,
                    fadeEnd,
                    Color.A / 2, 0
                );
            }
            */

            /*
            blurredCircle.Fade(
                ScaleEasing,
                ScaleStart < fadeStart ? fadeStart : ScaleStart,
                ScaleEnd,
                0, Color.A
            );
            blurredCircle.Fade(endTime, 0);
            */

            if (Additive) circle.Additive(fadeStart, endTime);

            double delta = beatsToMove * beatduration;
            for(double time = startTime; time < endTime - 5; time += delta)
            {
                // some logic for the scaling part
                if (time >= ScaleStart - 2 * beatduration - 5 && time < ScaleEnd)
                {
                    // movement resolution is increased so the easing can feel natural.
                    // this is done 2 beats earlier, because the different rings start
                    // at different times and would be misaligned when the scaling starts
                    delta = beatsToMove * beatduration / 8.0;
                    angleOffset = (startAngle - endAngle)/steps / 8.0;

                    if (time >= ScaleStart - 5)
                    {
                        double t1 = (time - ScaleStart) / (ScaleEnd - ScaleStart);
                        double t2 = (time + delta - ScaleStart) / (ScaleEnd - ScaleStart);
                        t1 = ScaleEasing.ToEasingFunction()(t1);
                        t2 = ScaleEasing.ToEasingFunction()(t2);
                        if (t1 <= 0.00001) t1 = 0;
                        if (t2 >= 0.99999) t2 = 1;
                        radius1 = radius + scaleDistance * t1;
                        radius2 = radius + scaleDistance * t2;
                    }
                }
                else if (time >= ScaleEnd)
                {
                    radius1 = radius2;
                    delta = beatsToMove * beatduration;
                    angleOffset = (startAngle - endAngle) / steps;
                }

                int dir = up ? 1 : -1;
                Vector2 p1 = new Vector2(
                    (float) (position.X + radius1*Math.Cos(angle)),
                    (float) (position.Y + radius1*Math.Sin(angle))
                );
                Vector2 p2 = new Vector2(
                    (float) (position.X + radius2*Math.Cos(angle + dir * angleOffset)),
                    (float) (position.Y + radius2*Math.Sin(angle + dir * angleOffset))
                );

                circle.Move(MovementEasing, time, time + delta, p1, p2);
                //if (blurredCircle.OpacityAt(time) > 0) blurredCircle.Move(MovementEasing, time, time + delta, p1, p2);

                angle += dir * angleOffset;
            }

            circle.Scale(ScaleEasing, ScaleStart, ScaleEnd, scale, scale * radius2 / radius);
            circle.Rotate(startTime, endTime, initialAngle - Math.PI/2, angle - Math.PI/2);

            double blurredScale = scale * GetMapsetBitmap(path).Width / (double) GetMapsetBitmap(blurredPath).Width;
            //blurredCircle.Scale(ScaleEasing, ScaleStart, ScaleEnd, blurredScale, blurredScale * radius2 / radius);
        }
    }
}
