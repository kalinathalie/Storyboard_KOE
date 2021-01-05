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
    public class AlienBG : StoryboardObjectGenerator
    {
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


        // Generates a line of alien chars with various effects
        public void generateLine(int startTime, int endTime, int verticalPos, bool left)
        {
            int cpt = 0;
            if(left)
            {
                for(int XPos = -80; XPos < 770; XPos += 50)
                {
                    generateChar(startTime + cpt * 40, endTime, new Vector2(XPos, verticalPos), left);
                    cpt++;
                }
            }
            else
            {
                for(int XPos = 770; XPos > -100; XPos -= 50)
                {
                    generateChar(startTime + cpt * 40, endTime, new Vector2(XPos, verticalPos), left);
                    cpt++;
                }
            }
            
        }

        // Generates one character of the line with flickering effect
        public void generateChar(float startTime, float endTime, Vector2 position, bool left)
        {
            float scale = 0.85f;
            float flickDelay = 80;
            float fade = 0.25f;
            Color4 gold = new Color4(218,165,32,1);

            int flickLimit1 = rnd.Next(7,25);
            int flickLimit2 = rnd.Next(7,25);
            int flickLimit3 = rnd.Next(7,25);

            float diff = endTime - startTime;

            float randomMoment1 = startTime + 0.33f*diff + rnd.Next(-2000,+2000);
            float randomMoment2 = startTime + 0.66f*diff + rnd.Next(-2000,+2000);

            for(int i = 0; i < flickLimit1; i++)
            {
                var texture = font.GetTexture(randomiseChar());
                var sprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                
                sprite.Scale(startTime + i*flickDelay, scale*texture.BaseHeight/200);
                sprite.Color(startTime + i*flickDelay,gold);

                if(i == flickLimit1-1)
                {
                    sprite.Fade(startTime + i*flickDelay, randomMoment1,fade,fade);
                }
                else sprite.Fade(startTime + i*flickDelay, startTime + (i+1)*flickDelay,fade,fade);

            }

            startTime = randomMoment1;

            for(int i = 0; i < flickLimit2; i++)
            {
                var texture = font.GetTexture(randomiseChar());
                var sprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                
                sprite.Scale(startTime + i*flickDelay, scale*texture.BaseHeight/200);
                sprite.Color(startTime + i*flickDelay,gold);

                if(i == flickLimit2-1)
                {
                    sprite.Fade(startTime + i*flickDelay, randomMoment2,fade,fade);
                }
                else sprite.Fade(startTime + i*flickDelay, startTime + (i+1)*flickDelay,fade,fade);
            }

            startTime = randomMoment2;

            for(int i = 0; i < flickLimit3; i++)
            {
                var texture = font.GetTexture(randomiseChar());
                var sprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                
                sprite.Scale(startTime + i*flickDelay, scale*texture.BaseHeight/200);
                sprite.Color(startTime + i*flickDelay,gold);

                if(i == flickLimit3-1)
                {
                    sprite.Fade(startTime + i*flickDelay, endTime,fade,fade);
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

    }
}
