using UnityEngine;

namespace IdenticalStudios.WorldManagement
{
    public struct GameDate
    {
        public readonly int Year;
        public readonly Month Month;
        public readonly int Day;


        public GameDate(int year, Month month, int day)
        {
            Year = year;
            Month = month;
            Day = Mathf.Clamp(day, 1, 30);
        }

        /// <summary>
        /// Returns a normalized value (0-1) that's representing of the current yearly time (January 1st = 0, December 30 = 1).
        /// </summary>
        /// <returns></returns>
        public float GetNormalizedYearTime()
        {
            float normalizedTime = ((int)Month - 1) / 12;
            normalizedTime += Day / 360f;

            return normalizedTime;
        }

        /// <summary>
        /// Returns a normalized value (0-1) that's representing of the current monthly time (1st day of a month = 0, Last day of a month = 1).
        /// </summary>
        /// <returns></returns>
        public float GetNormalizedMonthTime()
        {
            float normalizedTime = Day / 30f;
            return normalizedTime;
        }

        public string GetDateToStringWithSuffixes(bool year, bool month, bool day)
        {
            string timeStr = string.Empty;

            if (year)
                timeStr += (Year + "y ");

            if (month)
                timeStr += (Month + "m ");

            if (day)
                timeStr += (Day + "d ");

            return timeStr;
        }
    }

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }
}
