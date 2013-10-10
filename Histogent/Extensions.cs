using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using Math = System.Math;

namespace Histogent
{
    static class Extensions
    {
        public static void DrawRect(this Bitmap bitmap, Color color, int x, int y, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                bitmap.DrawLine(color, 1, x, y + i, x + width, y + i);
            }
        }

        public static Color Reverse(this Color color)
        {
            if (color == Color.White)
                return Color.Black;

            return Color.White;
        }
    }
}
