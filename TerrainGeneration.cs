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
    public class TerrainGeneration : StoryboardObjectGenerator
    {
        // General
        [Configurable]
        public int StartTime;

        [Configurable]
        public int EndTime;

        [Configurable]
        public int Frequency = 5000;

        [Configurable]
        public string PointSpritePath = "sb/pl.png";

        [Configurable]
        public float PointSpriteSize = 0.2f;

        [Configurable]
        public string LineSpritePath = "sb/pixel.png";

        [Configurable]
        public float LineSpriteWidth = 1f;

        // Colors
        [Configurable]
        public float ColorHueStart = 200f;

        [Configurable]
        public float ColorHueStep = 0.5f;

        // Grid Management
        [Configurable]
        public int GridSize;
        
        [Configurable]
        public float GridGap = 10f;

        [Configurable]
        public float GridRandomHeight = 40f;

        // Camera Management
        [Configurable]
        public float CameraHeight;

        [Configurable]
        public float CameraX = 320f;

        [Configurable]
        public float CameraY = 240f;
        
        [Configurable]
        public float CameraAngle = 85f;
        
        [Configurable]
        public float StartRotation = -10f;

        [Configurable]
        public float EndRotation = 10f;

        public override void Generate()
        {
            var terrain = RandomTerrain();
		    for (var x = 0; x < terrain.GetLength(0); x++)
            {
                for (var y = 0; y < terrain.GetLength(1); y++)
                {
                    DrawPointLines(terrain, x, y);
                }
            }
        }

        /*
         * Drawing Lines and Points
         */
        public void DrawPointLines(Vector3[,] terrain, int x, int y)
        {
            var thisP = terrain[x, y];
            var startPv3 = ProjectRotation(thisP, StartTime);
            startPv3 = ProjectPoint(startPv3);

            var startP = new Vector2(startPv3.X, startPv3.Y);
            if (startP == Vector2.Zero)
                return;

            startP += new Vector2(CameraX, CameraY);

            var sprite = GetLayer("").CreateSprite(PointSpritePath, OsbOrigin.Centre, startP);
            for (var time = StartTime; time < EndTime; time += Frequency)
            {
                var curP1v3 = ProjectRotation(thisP, time);
                curP1v3 = ProjectPoint(curP1v3);
                var curP2v3 = ProjectRotation(thisP, time + Frequency);
                curP2v3 = ProjectPoint(curP2v3);

                var curP1 = new Vector2(CameraX + curP1v3.X, CameraY + curP1v3.Y);
                var curP2 = new Vector2(CameraX + curP2v3.X, CameraY + curP2v3.Y);

                sprite.Move(time, time + Frequency, curP1, curP2);
            }
            //sprite.Move(StartTime, EndTime, p1, p2);
            sprite.Scale(StartTime, PointSpriteSize);
            sprite.ColorHsb(StartTime, GetColorHue(x, y), 1, 1);
            DrawPointLine(terrain, x, y, 0, 1);
            DrawPointLine(terrain, x, y, 1, 0);
            //DrawPointLine(terrain, x, y, 1, 0);
            //DrawPointLine(terrain, x, y, 0, -1);
            DrawPointLine(terrain, x, y, 1, -1);
            DrawPointLine(terrain, x, y, 1, 1);
        }

        public void DrawPointLine(Vector3[,] terrain, int x, int y, int dx, int dy)
        {
            try
            {
                var thisP = terrain[x, y];
                var thatP = terrain[x + dx, y + dy];

                if (thisP == Vector3.Zero || thatP == Vector3.Zero)
                    return;

                var sp1v3 = ProjectRotation(thisP, StartTime);
                sp1v3 = ProjectPoint(sp1v3);
                var sp2v3 = ProjectRotation(thatP, StartTime);
                sp2v3 = ProjectPoint(sp2v3);

                var sp1 = new Vector2(CameraX + sp1v3.X, CameraY + sp1v3.Y);
                var sp2 = new Vector2(CameraX + sp2v3.X, CameraY + sp2v3.Y);

                var sAng = GetAngle(sp1, sp2);
                var sSize = VectorDistance(sp1, sp2);

                var bitmap = GetMapsetBitmap(LineSpritePath);
                var sizeDiv = 1f / bitmap.Width;
                //Log(sizeDiv.ToString());
                var sprite = GetLayer("").CreateSprite(LineSpritePath, OsbOrigin.TopCentre, sp1);
                sprite.Rotate(StartTime, sAng);
                sprite.ScaleVec(StartTime, sizeDiv * LineSpriteWidth, sizeDiv * sSize);
                for (var time = StartTime; time < EndTime; time += Frequency)
                {
                    var curP1v3 = ProjectRotation(thisP, time);
                    curP1v3 = ProjectPoint(curP1v3);
                    var curP2v3 = ProjectRotation(thisP, time + Frequency);
                    curP2v3 = ProjectPoint(curP2v3);

                    var otherPv3 = ProjectRotation(thatP, time + Frequency);
                    otherPv3 = ProjectPoint(otherPv3);

                    var curP1 = new Vector2(CameraX + curP1v3.X, CameraY + curP1v3.Y);
                    var curP2 = new Vector2(CameraX + curP2v3.X, CameraY + curP2v3.Y);

                    var otherP = new Vector2(CameraX + otherPv3.X, CameraY + otherPv3.Y);

                    var angle = GetAngle(curP2, otherP);
                    if (sprite.RotationAt(time) - angle > Math.PI)
                        angle += Math.PI * 2;
                    else if (sprite.RotationAt(time) - angle < -Math.PI)
                        angle -= Math.PI * 2;
                    var scale = VectorDistance(curP2, otherP);
                    sprite.Rotate(time, time + Frequency, sprite.RotationAt(time), angle);
                    sprite.ScaleVec(time, time + Frequency, LineSpriteWidth * sizeDiv, sprite.ScaleAt(time).Y, LineSpriteWidth * sizeDiv, scale * sizeDiv);
                    sprite.Move(time, time + Frequency, curP1, curP2);
                }
                sprite.ColorHsb(StartTime, GetColorHue(x, y), 1, 1);

            }
            catch (IndexOutOfRangeException)
            {

            }
        }

        /*
         * Vector Math
         */

        public Vector3 ProjectPoint(Vector3 point)
        {
            return Vector3.Transform(point, Matrix3.CreateRotationX(CameraAngle * (float)Math.PI / 180));
        }

        public float VectorDistance(Vector2 a, Vector2 b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            return (float)Math.Sqrt(dx*dx + dy*dy);
        }

        public Vector3 ProjectRotation(Vector3 vec, int time)
        {
            var dr = EndRotation - StartRotation;
            var timeDiv = (float)(time - StartTime) / (EndTime - StartTime);
            var rotation = StartRotation + dr * timeDiv;

            return Vector3.Transform(vec, Matrix3.CreateRotationZ(rotation*(float)Math.PI/180));
        }

        public double GetAngle(Vector2 p1, Vector2 p2)
        {
            var ang = VectorHelper.GetAngle(p2, p1, new Vector2(p1.X, p1.Y - 1));
            if (p1.X > p2.X)
                ang = Math.PI * 2 - ang;
            ang += Math.PI;
            return ang;
        }

        public double GetColorHue(int x, int y)
        {
            return (double)(ColorHueStart + x * y * ColorHueStep);
        }

        /*
         * Interpolation Methods
         */
        public Vector2 GetTrackedPosition(Vector2 ps, Vector2 pe, int time)
        {
            var dx = pe.X - ps.X;
            var dy = pe.Y - ps.Y;
            var timeDiv = (float)(time - StartTime) / (EndTime - StartTime);

            return new Vector2(ps.X + dx * timeDiv, ps.Y + dy * timeDiv);
        }


        
        /*
        * Terrain Generation
        */
        public Vector3[,] RandomTerrain()
        {
            var points = new Vector3[GridSize + 1, GridSize + 1];
            for (var x = 0; x < GridSize; x++)
            {
                for (var y = 0; y < GridSize; y++)
                {
                    var randomHeight = Random(-GridRandomHeight, GridRandomHeight);
                    //randomHeight = 0;
                    var startLoc = new Vector3((x - GridSize/2) * GridGap, (y - GridSize/2) * GridGap, -CameraHeight + (float)randomHeight);
                    points[x, y] = startLoc;
                }
            }
            return points;
        }

        
    }
}
