using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Subtitles;
using System;
using System.Drawing;
using System.IO;

namespace StorybrewScripts{
    public class Lyrics : StoryboardObjectGenerator{

        [Configurable]
        public string SubtitlesPath = "lyrics.srt";

        [Configurable]
        public string FontName = "Verdana";

        [Configurable]
        public string SpritesPath = "sb/f";

        [Configurable]
        public string TimestampSplit = "";

        [Configurable]
        public string TracePath = "";

        [Configurable]
        public int FontSize = 26;

        [Configurable]
        public float FontScale = 0.5f;

        [Configurable]
        public Color4 FontColor = Color4.White;

        [Configurable]
        public FontStyle FontStyle = FontStyle.Regular;

        [Configurable]
        public int OutlineThickness = 3;

        [Configurable]
        public Color4 OutlineColor = new Color4(50, 50, 50, 200);

        [Configurable]
        public int ShadowThickness = 0;

        [Configurable]
        public Color4 ShadowColor = new Color4(0, 0, 0, 100);

        [Configurable]
        public Vector2 Padding = Vector2.Zero;

        [Configurable]
        public float SubtitleX = 240;

        [Configurable]
        public float SubtitleY = 400;

        [Configurable]
        public int StartTrace = 0;

        [Configurable]
        public int EndTrace = 0;

        [Configurable]
        public bool TrimTransparency = true;

        [Configurable]
        public bool EffectsOnly = false;

        [Configurable]
        public bool Debug = false;

        [Configurable]
        public OsbOrigin Origin = OsbOrigin.Centre;

        public override void Generate(){

            var layer = GetLayer("Lyrics");

            var font = LoadFont(SpritesPath, new FontDescription(){
                FontPath = FontName,
                FontSize = FontSize,
                Color = FontColor,
                Padding = Padding,
                FontStyle = FontStyle,
                TrimTransparency = TrimTransparency,
                EffectsOnly = EffectsOnly,
                Debug = Debug,
            },
            new FontOutline(){
                Thickness = OutlineThickness,
                Color = OutlineColor,
            },
            new FontShadow(){
                Thickness = ShadowThickness,
                Color = ShadowColor,
            });

            var subtitles = LoadSubtitles(SubtitlesPath);
            generatePerCharacter(font, subtitles, layer, TimestampSplit);
        }

        public void generatePerCharacter(FontGenerator font, SubtitleSet subtitles, StoryboardLayer layer, string TimestampSplit){
            
            var TimestampArray = Array.ConvertAll(TimestampSplit.Split(','), s => int.Parse(s));
            var LineStart = 0;
            var LineEnd = 0;
            var RunLine = 0f;
            var aniLyrics = 0;

            var trace = layer.CreateSprite(TracePath, OsbOrigin.Centre);

            if(StartTrace == 49997){
                trace.Move(StartTrace-tick(0, 2)-tick(0,1), StartTrace-tick(0,1), new Vector2(-120,400),new Vector2(70,400));
                trace.Fade(StartTrace-tick(0, 2)-tick(0,1), StartTrace-tick(0,1), 0, 0.65);
                trace.ScaleVec(StartTrace-tick(0, 2)-tick(0,1), StartTrace-tick(0,1), 0, 0.15, 0.22, 0.15);
                trace.Color(StartTrace-tick(0, 2)-tick(0,1), 0, 0, 0);
                trace.Fade(EndTrace-tick(0, 0.5), EndTrace, 0.65, 0);
            }else{
                trace.FlipH(StartTrace-tick(0, 2)-tick(0,1), EndTrace);
                trace.FlipV(StartTrace-tick(0, 2)-tick(0,1), EndTrace);
                trace.Move(StartTrace-tick(0, 2)-tick(0,1), StartTrace-tick(0,1), new Vector2(730,400),new Vector2(540,400));
                trace.Fade(StartTrace-tick(0, 2)-tick(0,1), StartTrace-tick(0,1), 0, 0.65);
                trace.ScaleVec(StartTrace-tick(0, 2)-tick(0,1), StartTrace-tick(0,1), 0, 0.15, 0.22, 0.15);
                trace.Color(StartTrace-tick(0, 2)-tick(0,1), 0, 0, 0);
                trace.Fade(EndTrace-tick(0, 0.5), EndTrace, 0.65, 0);
            }

            foreach (var subtitleLine in subtitles.Lines){
                foreach (var line in subtitleLine.Text.Split('\0')){
                    var letterX = SubtitleX;
                    var letterY = SubtitleY;

                    var lineWidth = 0f;
                    var lineHeight = 0f;

                    for(int x = 0; x<TimestampArray.Length; x++){
                        if(LineStart == TimestampArray[x]) continue;
                        if(TimestampArray[x] <= subtitleLine.StartTime && TimestampArray[x+1] >= subtitleLine.StartTime){
                            LineStart = TimestampArray[x];
                            LineEnd = TimestampArray[x+1];
                            RunLine = 0f;
                            aniLyrics = 0;
                            break;
                        }
                    }

                    foreach (var letter in line){
                        var texture = font.GetTexture(letter.ToString());
                        lineWidth += texture.BaseWidth * FontScale;
                        lineHeight = Math.Max(lineHeight, texture.BaseHeight * FontScale);
                        if (!texture.IsEmpty){
                            var position = new Vector2(letterX+RunLine, letterY)
                            + texture.OffsetFor(Origin) * FontScale;
                            var sprite = layer.CreateSprite(texture.Path, Origin);
                            //begin
                            sprite.Scale(LineStart+aniLyrics-tick(0,0.5)+tick(0,2), FontScale-0.2f);
                            sprite.Fade(LineStart-tick(0,0.75), LineStart-tick(0,1), 0, 0.4);
                            sprite.Move(LineStart-tick(0,0.75), LineStart-tick(0,1), position-new Vector2(0, 20), position);
                            //pulse
                            sprite.Scale(subtitleLine.StartTime - 50, subtitleLine.StartTime + 50, FontScale-0.2f, FontScale);
                            sprite.Fade(subtitleLine.StartTime - 50, subtitleLine.StartTime + 50, 0.4, 1);
                            //end
                            sprite.Scale(subtitleLine.StartTime + 50, subtitleLine.EndTime+tick(0, 1), FontScale, FontScale-0.2f);
                            sprite.Fade(subtitleLine.StartTime + 50, subtitleLine.EndTime+tick(0, 1), 1, 0);
                        }
                        letterX += texture.BaseWidth * FontScale;
                    }
                    RunLine+=letterX-SubtitleX;
                    letterY += lineHeight;
                    aniLyrics+=40;
                }
            }
        }
        double tick(double start, double divisor){
            return Beatmap.GetTimingPointAt((int)start).BeatDuration / divisor;
        }
    }
}
