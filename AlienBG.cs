using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Animations;
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
    public class AlienBG : StoryboardObjectGenerator
    {
        [Configurable]
        public Color4 color = new Color4(218,165,32,0.25f);

        // Generates Background with alien chars
        private StoryboardLayer layer;
        private double beatduration;
        FontGenerator font;
        Random rnd;

        private const String alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public override void Generate()
        {
		    layer = GetLayer("Main");
            beatduration = Beatmap.GetTimingPointAt(62840).BeatDuration;

            rnd = new Random();

            // Init the font

            font = LoadFont("sb/AlienFontGenerated", new FontDescription(){
                FontPath = "fonts/Alien_lines_1.9.ttf",
                FontSize = 72,
                Color = Color4.White,
                Padding = Vector2.Zero,
                FontStyle = System.Drawing.FontStyle.Regular,
                TrimTransparency = true,
                EffectsOnly = false,
                Debug = false,
            },
            new FontOutline(){
                Thickness = 0,
                Color = Color4.Transparent,
            },
            new FontShadow(){
                Thickness = 0,
                Color = Color4.Transparent,
            });

            generateLine(42735, 62840, 25, true);
            generateLine(42735, 62840, 65, false);
            generateLine(42735, 62840, 105, true);
            generateLine(42735, 62840, 145, false);
            generateLine(42735, 62840, 185, true);
            generateLine(42735, 62840, 225, false);
            generateLine(42735, 62840, 265, true);
            generateLine(42735, 62840, 305, false);
            generateLine(42735, 62840, 345, true);
            generateLine(42735, 62840, 385, false);
            generateLine(42735, 62840, 425, false);
            generateLine(42735, 62840, 465, true);
            
        }

/*
        public List<OsbSprite> generateArray(Vector2 topLeft, Vector2 bottomRight, int xCount, int yCount)
        {
            var sprites = new List<OsbSprite>();
            for (int i = 0; i < yCount; i++)
            {
                for (int j = 0; j < xCount; j++)
                {

                }
            }

        }
*/

        // Generates a line of alien chars with various effects
        public void generateLine(int startTime, int endTime, int verticalPos, bool left)
        {
            int cpt = 0;
            /* interveave
            if(left)
            {
                for (int XPos = -80; XPos < 770; XPos += 50)
                {
                    generateChar(startTime + cpt * 40, endTime, new Vector2(XPos, verticalPos), left);
                    cpt++;
                }
            }
            else
            {
                for (int XPos = 770; XPos > -100; XPos -= 50)
                {
                    generateChar(startTime + cpt * 40, endTime, new Vector2(XPos, verticalPos), left);
                    cpt++;
                }
            }
            */

            // random
            List<int> xPositions = new List<int>();
            for (int i = -80; i < 770; i += 50)
                xPositions.Add(i);
            int count = xPositions.Count();
            for(int i = 0; i < count; i++)
            {
                int r = Random(xPositions.Count());
                int xPos = xPositions.ElementAt(r);
                xPositions.RemoveAt(r);
                generateChar(startTime + cpt * 40, endTime, new Vector2(xPos, verticalPos), left);
                cpt++;
            }
        }

        // Generates one character of the line with flickering effect
        public void generateChar(float startTime, float endTime, Vector2 position, bool left)
        {
            float scale = 0.85f;
            float flickDelay = 80;
            var ease = OsbEasing.In.ToEasingFunction();
            float fade = (float) ease((MathHelper.Clamp((position - new Vector2(320, 240)).Length / 450.0, 0.1, 1))) * color.A;

            int flickLimit1 = rnd.Next(7,25);
            int flickLimit2 = rnd.Next(7,25);
            int flickLimit3 = rnd.Next(7,25);

            float diff = endTime - startTime;

            float randomMoment1 = startTime + 0.33f*diff + rnd.Next(-2000,+2000);
            float randomMoment2 = startTime + 0.66f*diff + rnd.Next(-2000,+2000);


            flicker(position, scale, color, fade, startTime, randomMoment1, flickLimit1, flickDelay);
            flicker(position, scale, color, fade, randomMoment1, randomMoment2, flickLimit2, flickDelay);
            flicker(position, scale, color, fade, randomMoment2, endTime, flickLimit3, flickDelay);
        }

        public void flicker(Vector2 position, float scale, Color4 color, float fade, float startTime, float endTime, int amount, float flickDelay)
        {
            for(int i = 0; i < amount; i++) {
                var texture = font.GetTexture(randomiseChar());
                var sprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                
                sprite.Scale(startTime + i*flickDelay, scale * texture.BaseHeight / 200);
                sprite.Color(startTime + i*flickDelay, color);

                if(i == amount-1)
                {
                    sprite.Fade(OsbEasing.OutCubic, startTime + i*flickDelay, endTime,fade,0);
                }
                else sprite.Fade(startTime + i*flickDelay, startTime + (i+1)*flickDelay,fade,fade);
            }
        }


        // Returns a random letter or number
        public string randomiseChar()
        {
            if(rnd.Next(0,100)>50) {
                int index = rnd.Next(0,52);
                return alphabet[index].ToString();
            }
            else return rnd.Next(0,9).ToString(); 
        }

        FlickeringSprite createFlickeringSprite(StoryboardLayer layer, int startTime, int endTime, int flickerDelay, int maxFlickerAmount, int delayBetweenOccurences, OsbOrigin origin = OsbOrigin.Centre)
        {
            var sprite = new FlickeringSprite(layer, origin);
            for ( int time = startTime + Random(0, delayBetweenOccurences);
                time < endTime;
                time += Random(0, delayBetweenOccurences))
            {
                int flickerAmount = Random(1, maxFlickerAmount);
                time = Random(startTime, endTime - flickerAmount * flickerDelay);
                for (int t = time; t < time + flickerAmount * flickerDelay && t < endTime; t += flickerDelay)
                {
                    var texture = font.GetTexture(randomiseChar());
                    sprite.Flicker(t, texture.Path);
                }
            }
            return sprite;
        }

        // class that manages and keeps track of a "sprite that can swap textures"
        private class FlickeringSprite
        {
            private List<OsbSprite> sprites = new List<OsbSprite>();
            private List<double> swapTimes = new List<double>();

            private StoryboardLayer layer;

            private OsbOrigin origin;

            public FlickeringSprite(StoryboardLayer layer, OsbOrigin origin = OsbOrigin.Centre)
            {
                this.layer = layer;
                this.origin = origin;
            }

            public FlickeringSprite(StoryboardLayer layer, OsbSprite sprite, double time)
            {
                this.layer = layer;
                sprites.Add(sprite);
                origin = sprite.Origin;
            }

            public void Flicker(double time, String newTexturePath)
            {
                sprites.Add(layer.CreateSprite(newTexturePath, origin));
                swapTimes.Add(time);
            }

            private static double lerp(double start, double end, double t)
                => start + (end - start) * t;

            private static double norm(double start, double end, double t)
                => (t - start) / (end - start);

            public void Fade(double startTime, double endTime, double startOpacity, double endOpacity)
            {
                var fadeLevels = new List<double>();
                for (int i = 0; i < swapTimes.Count; i++)
                    fadeLevels.Add(lerp(startOpacity, endOpacity, norm(startTime, endTime, swapTimes[i])));
                for (int i = 0; i < sprites.Count(); i++)
                {
                    if (i == sprites.Count() - 1 && swapTimes[i] >= startTime && swapTimes[i] < endTime)
                        sprites[i].Fade(swapTimes[i], endTime, fadeLevels[i], endOpacity);
                    else if (i == sprites.Count() - 1 && swapTimes[i] < startTime)
                        sprites[i].Fade(startTime, endTime, startOpacity, endOpacity);
                    else if (i == sprites.Count() - 1) break;
                    else if (swapTimes[i] >= startTime && swapTimes[i + 1] < endTime)
                        sprites[i].Fade(swapTimes[i], swapTimes[i + 1], fadeLevels[i], fadeLevels[i + 1]);
                    else if (swapTimes[i] >= startTime)
                        sprites[i].Fade(swapTimes[i], endTime, fadeLevels[i], endOpacity);
                    else if (swapTimes[i + 1] < endTime)
                        sprites[i].Fade(startTime, swapTimes[i + 1], startOpacity, fadeLevels[i + 1]);
                    else if (i != 0 && swapTimes[i] >= endTime) continue;
                    else
                        sprites[i].Fade(startTime, endTime, startOpacity, endOpacity);
                }
            }

            private static Vector2 lerp(Vector2 start, Vector2 end, double t)
                => start + (end - start) * (float) t;

            private static double norm(Vector2 start, Vector2 end, Vector2 t)
                => (t - start).Length / (end - start).Length;

            public void Move(double startTime, double endTime, Vector2 pos1, Vector2 pos2)
            {
                var positions = new List<Vector2>();
                for (int i = 0; i < swapTimes.Count; i++)
                    positions.Add(lerp(pos1, pos2, norm(startTime, endTime, swapTimes[i])));
                for (int i = 0; i < sprites.Count(); i++)
                {
                    if (i == sprites.Count() - 1 && swapTimes[i] >= startTime && swapTimes[i] < endTime)
                        sprites[i].Move(swapTimes[i], endTime, positions[i], pos2);
                    else if (i == sprites.Count() - 1 && swapTimes[i] < startTime)
                        sprites[i].Move(startTime, endTime, pos1, pos2);
                    else if (i == sprites.Count() - 1) break;
                    else if (swapTimes[i] >= startTime && swapTimes[i + 1] < endTime)
                        sprites[i].Move(swapTimes[i], swapTimes[i + 1], positions[i], positions[i + 1]);
                    else if (swapTimes[i] >= startTime)
                        sprites[i].Move(swapTimes[i], endTime, positions[i], pos2);
                    else if (swapTimes[i + 1] < endTime)
                        sprites[i].Move(startTime, swapTimes[i + 1], pos1, positions[i + 1]);
                    else if (i != 0 && swapTimes[i] >= endTime) continue;
                    else
                        sprites[i].Move(startTime, endTime, pos1, pos2);
                }
            }
        }
    }
}
