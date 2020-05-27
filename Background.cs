using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System.Linq;
using System.Collections.Generic;

namespace StorybrewScripts{
    public class Background : StoryboardObjectGenerator{

        [Configurable]
        public int StartTime = 0;

        [Configurable]
        public int EndTime = 0;

        [Configurable]
        public double Opacity = 0.2;

        public override void Generate(){
            if (StartTime == EndTime) EndTime = (int)(Beatmap.HitObjects.LastOrDefault()?.EndTime ?? AudioDuration);
            var BackgroundPath = get_bg(Beatmap.Name);
            var bitmap = GetMapsetBitmap(BackgroundPath);
            var bg = GetLayer("").CreateSprite(BackgroundPath, OsbOrigin.Centre);
            bg.Scale(StartTime, 480.0f / bitmap.Height);
            bg.Fade(StartTime - 500, StartTime, 0, Opacity);
            bg.Fade(EndTime, EndTime + 500, Opacity, 0);
        }

        string get_bg(string diff_name){
            Dictionary<string, string> background = new Dictionary<string, string>(){
                {"Airi's Extreme", "Sakura.jpg"},
                {"Kalindraz's Ambivalence", "Kalindraz.jpg"},
                {"LMT's Extreme", "LMT.jpg"},
                {"Maot's Insane", "Maot.jpg"},
                {"reicavera's Light Insane", "reicavera.jpg"},
                {"ShadowX's Extra", "ShadowX.png"},
                {"Cheri's Extra", "Cheri.jpg"},
                {"Satellite's Expert", "Satellite.jpg"},
                {"Nattu's Extra", "Nattu.jpg"},
                {"Dada's Insane", "Dada.jpg"},
                {"Hitsound", "Hitsounding.png"}
            };
            if (background.ContainsKey(diff_name)){
                return background[diff_name];
            }else{
                return "K4L1.jpg";
            }
        }
    }
}
