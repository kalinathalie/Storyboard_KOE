﻿using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System.Linq;
using System.Collections.Generic;

namespace StorybrewScripts{
    public class Background : StoryboardObjectGenerator{

        // AQUI EH TUDO NA MAO PORRA

        public override void Generate(){
            var BackgroundPath = get_bg(Beatmap.Name);
            var bitmap = GetMapsetBitmap(BackgroundPath);
            var bg = GetLayer("").CreateSprite(BackgroundPath, OsbOrigin.Centre);
            bg.Scale(0, 480.0f / bitmap.Height);
            bg.Fade((OsbEasing)6, 0, 2526, 0, 0.5);
            bg.Fade(2526, 10065, 0.2, 0.2);
            bg.Fade(10065, 12578, 0.2, 0.5);
            bg.Fade(22630, 41478, 0.5, 0.5);
            bg.Fade((OsbEasing)6, 41478, 42735, 0.5, 0);

            bg.Fade(113102, 131950, 0.5, 0.5);
            bg.Fade((OsbEasing)6, 131950, 133206, 0.5, 0);

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
