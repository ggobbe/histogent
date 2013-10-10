using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Media;
using System.Threading;

namespace Histogent
{
    public class Program
    {
        static Bitmap _display;
        static Timer _updateClockTimer;
        static readonly Font FontSmall = Resources.GetFont(Resources.FontResources.small);

        public static void Main()
        {
            // initialize our display buffer
            _display = new Bitmap(Bitmap.MaxWidth, Bitmap.MaxHeight);

            // Print "Loading..." for the first 5 seconds
            _display.Clear();
            _display.DrawText("Loading...", FontSmall, Color.White, 40, 60);
            _display.Flush();

            // obtain the current time
            var currentTime = DateTime.Now;

            // set up timer to refresh time every second
            var dueTime = new TimeSpan(0, 0, 0, 4, 1000 - currentTime.Millisecond); // start timer at beginning of next 5th second
            var period = new TimeSpan(0, 0, 0, 1, 0); // update time every second
            _updateClockTimer = new Timer(UpdateTime, null, dueTime, period); // start our update timer

            // go to sleep; time updates will happen automatically every second
            Thread.Sleep(Timeout.Infinite);
        }

        static void UpdateTime(object state)
        {
            // obtain the current time
            var currentTime = DateTime.Now;

            // clear our display buffer
            _display.Clear();

            // Time variables (to be processed by the for loop)
            var time = new int[]
            {
                (currentTime.Hour > 12 ? currentTime.Hour - 12 : currentTime.Hour),
                currentTime.Minute,
                currentTime.Second
            };

            // Time factors (to be processed by the for loop). 
            // NB : double to avoid Euclidean division
            var factors = new double[]
            {
                12d,    // 12 hours max
                60d,    // 60 minutes max
                60d     // 60 seconds max
            };

            // A few variables
            int x, y, width, height, txtX, txtY;

            for (var i = 0; i < time.Length; i++)
            {
                // Calculate rectangle dimensions
                x = 9 + (i * 40);
                y = (int)(Bitmap.MaxHeight - ((time[i] / factors[i]) * Bitmap.MaxHeight));
                width = 30;
                height = Bitmap.MaxHeight - y;

                // Draw rectangle
                _display.DrawRect(Color.White, x, y, width, height);

                // Calculate text position
                txtX = x + (width / 2) - (time[i] > 9 ? 5 : 1);
                txtY = (y > 10 ? y - 12 : y);

                // Draw text on top of the rectangle
                var txtColor = (y > 10 ? Color.White : Color.Black);
                _display.DrawText(time[i].ToString(), FontSmall, txtColor, txtX, txtY);
            }

            // flush the display buffer to the display
            _display.Flush();
        }
    }
}
