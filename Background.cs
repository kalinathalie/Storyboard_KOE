using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics;

namespace StorybrewScripts{
    public class Background : StoryboardObjectGenerator{

        // AQUI EH TUDO NA MAO PORRA

        public override void Generate(){
            var BackgroundPath = get_bg(Beatmap.Name);
            var bitmap = GetMapsetBitmap(BackgroundPath);
            var bg = GetLayer("").CreateSprite(BackgroundPath, OsbOrigin.Centre);
            
            bg.Fade(1583,2526,0,0.33);
            bg.Fade(21374,22630,0.33,0);
            
            bg.Scale(13, 480.0f / bitmap.Height);
            
            

            var vignette = GetLayer("").CreateSprite("sb/vig.png", OsbOrigin.Centre);
            vignette.Scale(22395,854.0/1900);
            vignette.Fade(91,42578,1,1);

            /*bg.Scale(0, 480.0f / bitmap.Height);
            bg.Fade((OsbEasing)6, 0, 2526, 0, 0.5);
            bg.Fade(2526, 10065, 0.2, 0.2);
            bg.Fade(10065, 12578, 0.2, 0.5);
            bg.Fade(22630, 41478, 0.5, 0.5);
            bg.Fade((OsbEasing)6, 41478, 42735, 0.5, 0);

            bg.Fade(113102, 131950, 0.5, 0.5);
            bg.Fade((OsbEasing)6, 131950, 133206, 0.5, 0);*/

        }

        string get_bg(string diff_name){
            Dictionary<string, string> background = new Dictionary<string, string>(){
                {"Airi's Extreme", "Airi"},
                {"Kalindraz's Ambivalence", "Kalindraz"},
                {"LMT's Extreme", "LMT"},
                {"Maot's Insane", "Maot"},
                {"reicavera's Light Insane", "reicavera"},
                {"ShadowX's Extreme", "ShadowX"},
                {"Cheri's Convalescence", "Cheri"},
                {"Satellite's Extra", "Satellite"},
                {"Nattu's Extra", "Nattu"},
                {"Dada's Expert", "Dada"},
                {"Hitsound", "Hitsound"},
                {"Rhuzerv", "K4L1"},
                {"Advanced", "K4L1"},
                {"Normal", "K4L1"},
                {"Rensia's Hard", "Rensia"},
                {"Faito's Extra", "Faito"},
                {"Ciyus Miapah's Extreme", "Ciyus"}
            };
            if (background.ContainsKey(diff_name)){
                return "sb/bgs/"+background[diff_name]+"/"+background[diff_name]+".jpg";
            }else{
                Log($"{diff_name}");
                return "Fail";
            }
        }
    }
}