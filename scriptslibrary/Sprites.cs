using System.Drawing;
using System.Drawing.Imaging;
using StorybrewCommon.Scripting;
using StorybrewCommon.Util;
using System;
using System.IO;

namespace Project.Resources
{
    public static class Sprites
    {
        public static String Circle = "sb/circle.png";
        public static String Ring = "sb/gold/circle-thin.png";
        public static String Pixel = "sb/pixel.png";
        public static String Triangle = "sb/triangle.png";
        public static String Particle = "sb/particle.png";
        public static String TransparentGradient = "sb/gradient-transparent.png";
        public static String SquareOutline = "sb/more_squares/sq_outline.png";
        public static String SquareFilled = "sb/more_squares/sq_filled.png";
    }

    public class SpriteResource
    {
        private StoryboardObjectGenerator effect;
        
        public SpriteResource(StoryboardObjectGenerator effect)
        {
            this.effect = effect;
        }
        public String blur(String path, int radius, double power)
        {
            String directory = Path.Combine(effect.MapsetPath, Path.GetDirectoryName(path), "blur");
            String filename = Path.Combine(directory, Path.GetFileName(path));
            if (System.IO.File.Exists(filename)) return filename;
            if (!System.IO.Directory.Exists(directory)) System.IO.Directory.CreateDirectory(directory);
            var original = effect.GetMapsetBitmap(path);
            var originalBounds = BitmapHelper.FindTransparencyBounds(original);
            int padding = 20;
            var bitmap = new Bitmap(original.Width + padding * 2, original.Height + padding * 2, PixelFormat.Format32bppArgb);
            var blurredBitmap = new Bitmap(original.Width + padding * 2, original.Height + padding * 2, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(bitmap))
                graphics.DrawImage(original, padding, padding, originalBounds.Value, GraphicsUnit.Pixel);
            blurredBitmap = BitmapHelper.Blur(bitmap, radius, power).Bitmap;
            var bounds = BitmapHelper.FindTransparencyBounds(bitmap);
            var trimBounds = bounds.Value;
            var trimmedBitmap = new Bitmap(trimBounds.Width, trimBounds.Height);
            using (var trimGraphics = Graphics.FromImage(trimmedBitmap))
                trimGraphics.DrawImage(blurredBitmap, 0, 0, trimBounds, GraphicsUnit.Pixel);
            trimmedBitmap.Save(filename, ImageFormat.Png);
            trimmedBitmap.Dispose();
            blurredBitmap.Dispose();
            bitmap.Dispose();
            return filename;
        }
    }
}