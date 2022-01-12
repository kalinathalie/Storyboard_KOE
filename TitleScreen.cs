using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;
using System;
using System.Drawing;
using System.IO;

namespace StorybrewScripts
{
    public class TitleScreen : StoryboardObjectGenerator
    {
        //Makes K1L4 titles screen at the beginning of the map

        [Configurable]
        public int StartTime = 13;

        [Configurable]
        public int EndTime = 1897;

        private StoryboardLayer layer;
        public override void Generate()
        {
            // Setting layer
            layer = GetLayer("Main");

            // Setting the font
            var font = LoadFont("sb/intro-chars", new FontDescription(){
                FontPath = "fonts/QanelasSoftDEMO-ExtraBold.otf",
                FontSize = 72,
                Color = Color4.White,
                Padding = Vector2.Zero,
                FontStyle = FontStyle.Regular,
                TrimTransparency = true,
                EffectsOnly = false,
                Debug = false,
            },
            new FontOutline(){
                Thickness = 0,
                Color = Color.Transparent,
            },
            new FontShadow(){
                Thickness = 0,
                Color = Color.Transparent,
            });

            var k = font.GetTexture("K");
            var kSprite = layer.CreateSprite(k.Path,OsbOrigin.TopCentre);
            var four = font.GetTexture("4");
            var fourSprite = layer.CreateSprite(four.Path,OsbOrigin.TopCentre);
            var l = font.GetTexture("L");
            var lSprite = layer.CreateSprite(l.Path,OsbOrigin.TopCentre);
            var one = font.GetTexture("1");
            var oneSprite = layer.CreateSprite(one.Path,OsbOrigin.TopCentre);

            setupTitle(kSprite,484, new Vector2(265,240));
            setupTitle(fourSprite,485+50, new Vector2(305,240));
            setupTitle(lSprite,484+100, new Vector2(345,240));
            setupTitle(oneSprite,484+150, new Vector2(375,240));
		    // Vertical bar and mask
            var mask = layer.CreateSprite("sb/pixel.png",OsbOrigin.TopCentre);
            mask.Scale(StartTime,400);
            mask.Fade(StartTime, 484+150+250,1,1);
            mask.Color(StartTime,Color4.Black);

            var line = layer.CreateSprite("sb/pixel.png");
            line.ScaleVec(StartTime,327,0,0,1,1);
            line.ScaleVec(OsbEasing.InOutCirc,327,641,1,1,150,1);
            line.Fade(StartTime,EndTime-200,1,1);
            line.Fade(EndTime-200,EndTime,1,0);

            //Presents

            var presents = layer.CreateSprite("sb/presents.png",OsbOrigin.Centre,new Vector2(320,260));
            presents.Scale(641,0.2);
            presents.Fade(641,641+200,0,1);
            presents.Fade(EndTime-200,EndTime,1,0);

            


        }

        // Makes the letters appear one by one
        public void setupTitle(OsbSprite sprite, int time, Vector2 position)
        {
            sprite.Scale(StartTime,0.6);
            sprite.Fade(StartTime,EndTime-200,1,1);
            sprite.Fade(EndTime-200,EndTime,1,0);
            sprite.Move(OsbEasing.OutCirc, time, time+250, position, position + new Vector2(0,-50));

        }
    }
}
