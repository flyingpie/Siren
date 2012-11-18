using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B3
{
    public class Formatting
    {
        private static string[] TimeUnitsNames = { "milliseconds", "seconds", "minutes", "hours", "days", "months", "years", "decades", "centuries" };

        private static int[] TimeUnitsValue = { 1000, 60, 60, 24, 30, 12, 10, 10 }; // Reference unit is millis

        private const Decimal OneMinute = 60M;
        private const Decimal OneHour = OneMinute * 60M;
        private const Decimal OneDay = OneHour * 24M;

        public static string FormatSeconds(long seconds)
        {
            Decimal time = new Decimal(seconds);

            string suffix;
            if (time > OneDay)
            {
                time /= OneDay;
                suffix = " days";
            }
            else if (time > OneHour)
            {
                time /= OneHour;
                suffix = " hours";
            }
            else if (time > OneMinute)
            {
                time /= OneMinute;
                suffix = " minutes";
            }
            else
            {
                suffix = " seconds";
            }

            return String.Format("{0}{1}", Math.Round(time), suffix);
        }

        private const Decimal OneKiloByte = 1024M;
        private const Decimal OneMegaByte = OneKiloByte * 1024M;
        private const Decimal OneGigaByte = OneMegaByte * 1024M;

        public static String FormatBytes(long bytes, int precision)
        {
            Decimal size = new Decimal(bytes);

            string suffix;
            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = "GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = "MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = "kB";
            }
            else
            {
                suffix = "B";
            }

            return String.Format("{0}{1}", Math.Round(size, precision), suffix);
        }

        public static String FormatBytes(long bytes)
        {
            return FormatBytes(bytes, 2);
        }
    }
}
