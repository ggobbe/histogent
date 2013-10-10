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
        // Edit these constants to modify the watch face
        const bool ShowSeconds = true;  // if true print a bar for the seconds
        const int BarsWidth = 30;   // max 40 with seconds - max 60 without seconds

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

            // Print the watch directly if seconds are hidden
            if (!ShowSeconds)
                UpdateTime(null);

            // obtain the current time
            var currentTime = DateTime.Now;

            // set up timer to refresh time every second
            var dueTime = new TimeSpan(0, 0, 0, (ShowSeconds ? 4 : 59 - currentTime.Second), 1000 - currentTime.Millisecond); // start timer at beginning of next 5th second
            var period = new TimeSpan(0, 0, (ShowSeconds ? 0 : 1), (ShowSeconds ? 1 : 0), 0); // update time every minute/second
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
            var time = new int[(ShowSeconds ? 3 : 2)];
            time[0] = (currentTime.Hour > 12 ? currentTime.Hour - 12 : currentTime.Hour);
            time[1] = currentTime.Minute;

            if (ShowSeconds)
                time[2] = currentTime.Second;

            // Time factors (to be processed by the for loop). 
            // NB : double to avoid Euclidean division
            var factors = new double[]
            {
                12d,    // 12 hours max
                60d,    // 60 minutes max
                60d     // 60 seconds max
            };

            // A few variables
            int x, y, width = BarsWidth, height, txtX, txtY;

            for (var i = 0; i < time.Length; i++)
            {
                // Calculate rectangle dimensions
                // e.g. 128px screen => 9px empty + 30px bar + 10px empty + 30px bar + 10px empty + 30px bar + 9px empty
                x = (int)(((Bitmap.MaxWidth - (((width + 10)*time.Length) - 10)) / 2.0) + (i * (width + 10)));
                y = (int)(Bitmap.MaxHeight - ((time[i] / factors[i]) * Bitmap.MaxHeight));
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
