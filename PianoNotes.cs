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
using System.Drawing;
using System.Linq;

namespace StorybrewScripts
{
    public class PianoNotes : StoryboardObjectGenerator
    {
        // Adds Piano Notes on the sides with effects according to the intensity of the note

        [Configurable]
        public int xPosition = 520;

        [Configurable]
        public int time = 2526;

        [Configurable]
        public String notePath = "";

        StoryboardLayer layer;
        double beatduration;
        FontGenerator fontNormal, fontBold;
        public override void Generate()
        {
		    layer = GetLayer("Foreground");
            beatduration = Beatmap.GetTimingPointAt(2000).BeatDuration;

            // Setting the fonts
            fontNormal = LoadFont("sb/PianoNotesRegular", new FontDescription(){
                FontPath = "fonts/Altero-Outline.otf",
                FontSize = 72,
                Color = Color4.White,
                Padding = new Vector2(5,5),
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


            fontBold = LoadFont("sb/PianoNotesBold", new FontDescription(){
                FontPath = "fonts/Altero-Regular.otf",
                FontSize = 72,
                Color = Color4.White,
                Padding = new Vector2(5,5),
                FontStyle = FontStyle.Bold,
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

            generateNote("G7", 2526, 360, false);
            generateNote("F#4", 2683, 320, true);

            generateNote("B#3", 3782, 320, false);
            generateNote("D#4", 3939, 280, true);

            generateNote("A4", 4882, 270, true);
            generateNote("F1", 5039, 230, false);
            generateNote("G#7", 5196, 190, true);


            
        }
       

        // A function that generates one note with the bold or the regular font
        public void generateNote(String note, double time, int startingY, bool bold)
        {
            double scale = 0.5;
            OsbSprite sprite;
            if(bold) sprite = layer.CreateSprite(fontBold.GetTexture(note).Path, OsbOrigin.Centre);
            else sprite = layer.CreateSprite(fontNormal.GetTexture(note).Path, OsbOrigin.Centre);
            
            sprite.Scale(time, scale);
            sprite.Color(time-75,Color4.Black);
            sprite.Fade(time-75,time,0,1);
            sprite.Fade(time+4*beatduration-75, time+4*beatduration, 1, 0);

            sprite.Move(OsbEasing.OutCubic, time-75, time, xPosition, startingY-40, xPosition, startingY);
            sprite.Move(OsbEasing.None, time, time + 4*beatduration-75, xPosition, startingY, xPosition, startingY + 40);
            sprite.Move(OsbEasing.OutCubic, time + 4*beatduration - 75, time + 4*beatduration, xPosition, startingY + 40, xPosition, startingY + 80);

        }
    }



    
}
