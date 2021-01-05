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
using System.Drawing;
using System.Linq;

namespace StorybrewScripts
{
    public class PianoTiles : StoryboardObjectGenerator
    {

        StoryboardLayer layer;
        double beatduration;

        FontGenerator font;

        // Inner class for piano notes with all the infos needed inside.

        public class PianoNote {
            private OsbSprite top;
            private OsbSprite bottom;
            private OsbSprite body;
            private Vector2 position;
            private bool up;
            private float scale;

            public PianoNote(){
            }

            public PianoNote(OsbSprite top, OsbSprite bottom, OsbSprite body, Vector2 position, bool up, float scale)
            {
                this.top = top;
                this.bottom = bottom;
                this.body = body;
                this.position = position;
                this.up = up;
                this.scale = scale;
            }

            public void click(int time)
            {
                float decalage = (float) (25 * scale / 0.1);
                OsbEasing easeOut = OsbEasing.OutExpo;
                OsbEasing easeIn = OsbEasing.None;
                if(up)
                {
                    top.Move(easeOut, time, time + 100, position - new Vector2(0, scale*100), position - new Vector2(0, scale*100) - new Vector2(0, decalage));
                    top.Move(easeIn, time + 100, time + 250, position - new Vector2(0, decalage) - new Vector2(0, scale*100), position - new Vector2(0, scale*100));
                    body.ScaleVec(easeOut, time, time + 100, scale, scale/2, scale, (decalage + scale*100)/200.0);
                    body.ScaleVec(easeIn, time + 100, time + 250, scale, (decalage + scale*100)/200.0, scale, scale/2);
                }
                else
                {
                    bottom.Move(easeOut, time, time + 100, position + new Vector2(0, scale*100) , position + new Vector2(0, scale*100) + new Vector2(0, decalage));
                    bottom.Move(easeIn, time + 100, time + 250, position + new Vector2(0, decalage) + new Vector2(0, scale*100) , position + new Vector2(0, scale*100) );
                    body.ScaleVec(easeOut, time, time + 100, scale, scale/2, scale, (decalage + scale*100)/200.0);
                    body.ScaleVec(easeIn, time + 100, time + 250, scale, (decalage + scale*100)/200.0, scale, scale/2);
                }
            }

            public void fadeIn(int time)
            {
                top.Fade(time-200,time, 0, 1);
                bottom.Fade(time-200,time, 0, 1);
                body.Fade(time-200,time, 0, 1);
            }

            public void fadeOut(int time)
            {
                top.Fade(time-200,time, 1, 0);
                bottom.Fade(time-200,time, 1, 0);
                body.Fade(time-200,time, 1, 0);
            }

            public Vector2 getPos()
            {
                return position;
            }

            
        }
        public override void Generate()
        {
		    layer = GetLayer("Main");
            beatduration = Beatmap.GetTimingPointAt(13).BeatDuration;

            // Loading the big notes

            ArrayList beeg = new ArrayList(); 
            for(int i = 0; i<4; i++)
            {
                Vector2 position = new Vector2(320 - 90 + i*60, 260);
                var top = layer.CreateSprite("sb/piano/top.png",OsbOrigin.BottomCentre, position);
                var bottom = layer.CreateSprite("sb/piano/bottom.png",OsbOrigin.TopCentre, position + new Vector2(0, 0.1f*200));
                var body = layer.CreateSprite("sb/piano/body.png",OsbOrigin.BottomCentre, position);

                top.Scale(2369, 0.2);
                bottom.Scale(2369, 0.2);
                body.ScaleVec(2369, 0.2, 0.1);
                body.Rotate(2369, Math.PI);

                PianoNote pn1 = new PianoNote(top, bottom, body, position, false, 0.2f);
                pn1.fadeIn(2369);
                pn1.fadeOut(20117);
                
                beeg.Add(pn1);

            }

            // Loading the small notes

            ArrayList smol = new ArrayList(); 
            for(int i = 0; i<5; i++)
            {
                Vector2 position = new Vector2(320 - 2*60 + i*60, 220);
                var top = layer.CreateSprite("sb/piano/top-fill.png",OsbOrigin.BottomCentre, position - new Vector2(0, 0.075f*200));
                var bottom = layer.CreateSprite("sb/piano/bottom-fill.png",OsbOrigin.TopCentre, position);
                var body = layer.CreateSprite("sb/piano/body-fill.png",OsbOrigin.BottomCentre, position);

                top.Scale(2369, 0.15);
                bottom.Scale(2369, 0.15);
                body.ScaleVec(2369, 0.15, 0.075);

                PianoNote pn1 = new PianoNote(top, bottom, body, position, true, 0.15f);
                pn1.fadeIn(2369);
                pn1.fadeOut(20117);
                
                smol.Add(pn1);

            }


            // Setting font

            font = LoadFont("sb/PianoNotesRegular", new FontDescription(){
                FontPath = "fonts/Altero-Regular.otf",
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

            

            clickNoteBottom((PianoNote)(beeg[1]), 2526, "G#7");
            clickNoteBottom((PianoNote)(beeg[3]), 2683, "A4");

            clickNoteTop((PianoNote)(smol[1]), 2840, "C#7");
            clickNoteTop((PianoNote)(smol[4]), 2997, "D4");
            clickNoteTop((PianoNote)(smol[0]), 3154, "F1");
            clickNoteTop((PianoNote)(smol[2]), 3311, "B#5");
            clickNoteTop((PianoNote)(smol[3]), 3468, "A1");
            clickNoteTop((PianoNote)(smol[1]), 3625, "B#2");
            
        }

        public void clickNoteTop(PianoNote pianoNote, int time, String notePlayed)
        {
            var texture = font.GetTexture(notePlayed);
            var noteSprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, pianoNote.getPos() - new Vector2(0,80));

            noteSprite.Scale(time, 0.18);
            noteSprite.Fade(time,time+50,0,0.66);
            noteSprite.Fade(time+200,time+350,0.66,0);

            pianoNote.click(time);

        }

        public void clickNoteBottom(PianoNote pianoNote, int time, String notePlayed)
        {
            var texture = font.GetTexture(notePlayed);
            var noteSprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, pianoNote.getPos() + new Vector2(0,110));

            noteSprite.Scale(time, 0.24);
            noteSprite.Fade(time,time+50,0,0.66);
            noteSprite.Fade(time+200,time+350,0.66,0);

            pianoNote.click(time);

        }
    }
}
