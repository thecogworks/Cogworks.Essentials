using System;
using System.Globalization;

namespace Cogworks.Essentials.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetUnixTimeStamp(this DateTime date)
            => (int)date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public static string ToShortDate(this DateTime date)
            => date.ToString("dd MMM yyyy");

        public static string GetLongMonthName(this DateTime date, CultureInfo culture = null,
            string defaultCultureCode = null)
        {
            if (!defaultCultureCode.HasValue())
            {
                defaultCultureCode = CultureInfo.CurrentCulture.TextInfo.CultureName;
            }

            if (!culture.HasValue())
            {
                culture = CultureInfo.GetCultureInfo(defaultCultureCode);
            }

            return date.ToString("MMMM", culture);
        }

        public static DateTime ToDateTime(this long ticks)
            => new DateTime(ticks);

        public static bool HasValue(this DateTime date)
            => date != default;
    }
}