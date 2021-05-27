namespace Cogworks.Essentials.Constants
{
    public static class DateTimeConstants
    {
        public static class TimeInMillisecondsConstants
        {
            public const int Second = 1000;
            public const int Minute = 60 * Second;
            public const int Hour = 60 * Minute;
            public const int Day = 24 * Hour;
        }

        public static class TimeInSecondsConstants
        {
            public const int Second = 1;
            public const int Minute = 60 * Second;
            public const int Hour = 60 * Minute;
            public const int Day = 24 * Hour;
            public const int Year = Day * 365;
        }
    }
}