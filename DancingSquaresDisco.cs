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

    /*
        this effect is a way too complex intro to the storyboard with a bunch of squares doing a coregraphy
        works only on this instance, no need to make it configurable
    */
    public class DancingSquaresDisco : StoryboardObjectGenerator
    {
        public StoryboardLayer layer;
        const String squarePath = "sb/sq.png";
        public double qBeat;
        Vector2 origin;
        const int offset = 35;

        //Square Class, contains all the operations to do on a square
        public class SquareSprite{
            public OsbSprite sprite;
            public Color4 color;
            public Vector2 position;

            private StoryboardLayer layerP;
            public static double quatterBeat;

            //Construct
            public SquareSprite(){}
            public SquareSprite(OsbSprite sqsprite, double time, StoryboardLayer lay)
            {
                sprite = sqsprite;
                sprite.Fade(time,1);
                position = (Vector2) sprite.PositionAt(time);
                sprite.Color(time,Color4.White);
                color = Color4.White;

                layerP = lay;
                //quatterBeat =;
            }

            public void duplicate(int time)
            {
                var sp1 =  layerP.CreateSprite(squarePath,OsbOrigin.Centre, position);
                var sp2 =  layerP.CreateSprite(squarePath,OsbOrigin.Centre, position);

                sp1.Move(OsbEasing.OutCirc,time,time+158,position,position+ new Vector2(offset,0));
                sp2.Move(OsbEasing.OutCirc,time,time+158,position,position+ new Vector2(0,offset));

                sp1.Move(OsbEasing.OutCirc,time+158,time+2*158,position + new Vector2(offset,0),position+ new Vector2(offset,offset));
                sp2.Move(OsbEasing.OutCirc,time+158,time+2*158,position + new Vector2(0,offset),position+ new Vector2(offset,offset));

            }

            public void reverseDuplication(int time)
            {
                var sp1 =  layerP.CreateSprite(squarePath,OsbOrigin.Centre, position);
                var sp2 =  layerP.CreateSprite(squarePath,OsbOrigin.Centre, position);

                sp1.Move(OsbEasing.OutCirc,time,time+2*quatterBeat,position,position+ new Vector2(offset,0));
                sp2.Move(OsbEasing.OutCirc,time,time+2*quatterBeat,position,position+ new Vector2(0,offset));

                sp1.Move(OsbEasing.OutCirc,time+2*quatterBeat,time+4*quatterBeat,position + new Vector2(offset,0),position+ new Vector2(offset,offset));
                sp2.Move(OsbEasing.OutCirc,time+2*quatterBeat,time+4*quatterBeat,position + new Vector2(0,offset),position+ new Vector2(offset,offset));

                sp1.Move(OsbEasing.OutCirc,time+4*quatterBeat,time+6*quatterBeat,position + new Vector2(offset,offset),position+ new Vector2(offset,0));
                sp2.Move(OsbEasing.OutCirc,time+4*quatterBeat,time+6*quatterBeat,position+ new Vector2(offset,offset),position+ new Vector2(0,offset));

                sp1.Move(OsbEasing.OutCirc,time+6*quatterBeat,time+8*quatterBeat,position + new Vector2(offset,0),position);
                sp2.Move(OsbEasing.OutCirc,time+6*quatterBeat,time+8*quatterBeat,position + new Vector2(0,offset),position);
            }

            public void bounce(double time)
            {
                sprite.Scale(OsbEasing.OutCirc,time,time+40,1,1.25);
                sprite.Scale(time+40,time+120,1.25,1);

            }

            public void colorChange(double time, Color4 colorC)
            {
                sprite.Color(time,colorC);
            }

            public void colorFlash(double time, Color4 colorS, Color4 colorE)
            {
                sprite.Color(OsbEasing.InCirc,time,time+120,colorS,colorE);
            }

            public void rotationation(int time, int revolutionNumber)
            {
                for(int i = 0; i < revolutionNumber; i++)
                {
                    sprite.Rotate(OsbEasing.OutSine,time+i*2*quatterBeat, time+(i+1)*2*quatterBeat,0,0.5*Math.PI);
                }
            }

            public void shift(Vector2 shifting,double time)
            {
                sprite.Move(OsbEasing.OutCirc,time,time+2*quatterBeat,position,position+shifting);
                position += shifting;
            }

            public void fadeout(int time)
            {
                sprite.Fade(time,time+1,1,0);
            }
        }

        //*****************************************************************************

        public override void Generate()
        {
            layer = GetLayer("Main");
            qBeat = Beatmap.GetTimingPointAt(12578).BeatDuration/4;
            SquareSprite.quatterBeat = qBeat;
            origin = new Vector2(320 ,240);




            generateSquares(2526,22630);
        }

        public void generateSquares(int time, int endtime)
        {
            SquareSprite[,] gridSquares = new SquareSprite[10,10];
            int limit = 1;
            gridSquares[0,0] = new SquareSprite(layer.CreateSprite(squarePath,OsbOrigin.Centre,origin),time,layer);

            for(int i = 0; i < 1; i++)
            {
                rotateAll(2526,gridSquares,limit,8);

                duplicationTR(3782,gridSquares,limit);
                limit++;

                rotateAll(4096,gridSquares,limit,4);

                duplicationTR(4725,gridSquares,limit);
                limit++;

                duplicationTR(5039,gridSquares,limit);
                limit++;

                rotateAll(5353,gridSquares,limit,6);

                duplicationTR(6295,gridSquares,limit);
                limit++;

                rotateAll(6609,gridSquares,limit,2);

                revDuplicationTR(6923,gridSquares,limit);

                duplicationTR(7552,gridSquares,limit);
                limit++;

                rotateAll(7866,gridSquares,limit,6);

                duplicationTR(8808,gridSquares,limit);
                limit++;

                rotateAll(9122,gridSquares,limit,4);

                duplicationTR(9751,gridSquares,limit);
                limit++;

                duplicationTR(10065,gridSquares,limit);
                limit++;

                rotateAll(10379,gridSquares,limit,6);

                duplicationTR(11321,gridSquares,limit);
                limit++;

                rotateAll(11636,gridSquares,limit,2);

                revDuplicationTR(11950,gridSquares,limit);

            }

            dualTap(12578,4,ref gridSquares[6,6],ref gridSquares[6,7], Color4.DarkCyan,Color4.Cyan);
            snake(12578,gridSquares,Color4.Orange);

            fadeAllOut(22630,gridSquares,limit);

        }

        public void duplicationTR(int time, SquareSprite[,] grid, int lim)
        {
            for(int i = 0; i < lim; i++)
            {
                grid[lim-1,i].duplicate(time);
                grid[lim,i] = new SquareSprite(layer.CreateSprite(squarePath,OsbOrigin.Centre,origin + new Vector2(lim*offset,i*offset)),time+2*qBeat,layer);
            }
            for(int j = 0; j < lim; j++)
            {
                grid[j,lim-1].duplicate(time);
                grid[j,lim] = new SquareSprite(layer.CreateSprite(squarePath,OsbOrigin.Centre,origin + new Vector2(j*offset,lim*offset)),time+2*qBeat,layer);
            }
            grid[lim,lim] = new SquareSprite(layer.CreateSprite(squarePath,OsbOrigin.Centre,origin + new Vector2(lim*offset,lim*offset)),time+4*qBeat,layer);

            for(int i = 0 ; i < lim+1 ; i++)
            {
                for(int j = 0 ; j < lim+1; j ++)
                {
                    grid[i,j].shift(new Vector2(-0.5f*offset,-0.5f*offset),time+4*qBeat);
                    
                }
            }
            origin += new Vector2(-0.5f*offset,-0.5f*offset);
        }

        public void revDuplicationTR(int time, SquareSprite[,] grid, int lim)
        {
            for(int i = 0; i < lim; i++)
            {
                grid[lim-1,i].reverseDuplication(time);
                
            }
            for(int j = 0; j < lim; j++)
            {
                grid[j,lim-1].reverseDuplication(time);
                
            }
            
        }

        public void rotateAll(int time,SquareSprite[,] grid, int lim, int revolNumber)
        {
            for(int i = 0 ; i < lim ; i++)
            {
                for(int j = 0 ; j < lim; j ++)
                {
                    grid[i,j].rotationation(time,revolNumber);
                }
            }
        }

        public void fadeAllOut(int time,SquareSprite[,] grid, int lim)
        {
            for(int i = 0 ; i < lim ; i++)
            {
                for(int j = 0 ; j < lim; j ++)
                {
                    grid[i,j].fadeout(time);
                }
            }
        }

        public void dualTap(int time, int cycles, ref SquareSprite sq1, ref SquareSprite sq2, Color4 color, Color4 endColor)
        {
            bool l = true;
            for(double t = time; t < time + 4*cycles*qBeat; t += qBeat)
            {
                if(l) {
                    sq1.bounce(t);
                    sq1.colorFlash(t,color,endColor);
                }
                else
                {
                    sq2.bounce(t);
                    sq2.colorFlash(t,color,endColor);
                }
                l = !l;

                
            }
            sq2.colorChange(time + 4*cycles*qBeat + 2*qBeat,Color4.White);
            sq1.colorChange(time + 4*cycles*qBeat + 2*qBeat,Color4.White);
        }

        public void snake(double time, SquareSprite[,] grid,Color4 color)
        {
            for(int i = 0 ; i < 10 ; i++)
            {
                for(int j = 0 ; j < 10; j ++)
                {
                    grid[j,i].colorChange(time,color);
                    grid[j,i].bounce(time);
                    time += qBeat;
                }
            }
        }
    }
}
