using System;

namespace WaymarkOCDPlugin
{
    public class PolarCoordinatesHelper
    {
        public static (int, int) GetXYCoordinatesFromPolar(double distance, double angle)
        {
            // force north to be angle 0
            angle -= 90;

            double angleInRadians = angle * Math.PI / 180.0;

            int x = (int)Math.Round(Math.Cos(angleInRadians) * distance, 0);
            int y = (int)Math.Round(Math.Sin(angleInRadians) * distance, 0);
            return (x, y);
        }

        public static (double, double) GetPolarCoordinatesFromXY(int x, int y)
        {
            double distance = GetDistanceFromCenter(x, y);
            double angle = GetAngleFromXY(x, y);
            return (distance, angle);
        }

        private static double GetDistanceFromCenter(int x, int y)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        private static double GetAngleFromXY(int x, int y)
        {
            double angle = (Math.Atan2(y, x) * 180.0 / Math.PI);

            // by default, angle 0 is straight east
            // by adding 90, we force north to be angle 0
            angle += 90;

            // by default the angle is between [-180, 180)
            // lets make it positive, between [0, 360)
            angle += 360;
            if (angle >= 360) angle -= 360;

            return angle;
        }
    }
}
