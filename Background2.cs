using OpenTK;
using OpenTK.Graphics;
using Project.Util;
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
    public class Background2 : StoryboardObjectGenerator
    {
        [Configurable]
        public Color4 BackgroundColor = new Color4(0.15f, 0.15f, 0.15f, 1);

        [Configurable]
        public Color4 BackgroundColor2 = new Color4(0.15f, 0.15f, 0.15f, 1);
        Timing Timing;

        public override void Generate()
        {
            Timing = new Timing(Beatmap);

            var bg = GetLayer("").CreateSprite("sb/gold/bg-tex.jpg");
            bg.Scale(42578, 854.0 / 3888);
            bg.Fade(42578, 42578 + 120, 0, BackgroundColor.A);
            bg.Color(42578, BackgroundColor);
            bg.Fade(OsbEasing.OutQuint, 62840 - 2 * Timing.beat(62840), 62840, BackgroundColor.A, BackgroundColor2.A);

            bg.Color(62840, BackgroundColor2);
            bg.Fade(72892, 0);
            bg.Additive(62840, 72892);

            var vig = GetLayer("").CreateSprite("sb/vig.png");
            vig.Scale(22395,854.0/1900);
            vig.Fade(OsbEasing.OutExpo, 62840, 62840 + Timing.beat(62840), 0, 1);
            vig.Fade(72892, 0);
        }
    }
}
