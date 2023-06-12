using UnityEngine;

namespace IdenticalStudios.WorldManagement
{
    public struct GameTime
    {
        public readonly int Hours;
        public readonly int Minutes;
        public readonly int Seconds;


        public GameTime(float realtimeDuration, float inGameDayScale)
        {
            float inGameDuration = realtimeDuration / inGameDayScale;

            Hours = (int)(inGameDuration / 3600);
            Minutes = (int)((inGameDuration - Hours * 3600) / 60);
            Seconds = Mathf.CeilToInt(inGameDuration - Hours * 3600 - Minutes * 60);
        }

        public GameTime(float hours, float minutes, float seconds)
        {
            Hours = Mathf.Clamp((int)hours, 0, 24);
            Minutes = Mathf.Clamp((int)minutes, 0, 60);
            Seconds = Mathf.Clamp((int)seconds, 0, 60);
        }

        public GameTime(int hours, int minutes, int seconds)
        {
            Hours = Mathf.Clamp(hours, 0, 24);
            Minutes = Mathf.Clamp(minutes, 0, 60);
            Seconds = Mathf.Clamp(seconds, 0, 60);
        }

        public float GetNormalized()
        {
            float normalizedTime = (Hours - 1) / 24f;
            normalizedTime += Minutes / 60f;
            normalizedTime += Seconds / 3600f;

            return normalizedTime;
        }

        public string GetTimeToString(bool hours, bool minutes, bool seconds)
        {
            string timeStr = string.Empty;

            if (hours)
                timeStr += Hours.ToString("00");

            if (minutes)
                timeStr += (hours ? ":" : "") + Minutes.ToString("00");

            if (seconds)
                timeStr += (minutes ? ":" : "") + Seconds.ToString("00");

            return timeStr;
        }

        public string GetTimeToStringWithSuffixes(bool hours, bool minutes, bool seconds)
        {
            string timeStr = string.Empty;

            if (hours && Hours > 0)
                timeStr += (Hours + "h ");

            if (minutes && Minutes > 0)
                timeStr += (Minutes + "m ");

            if (seconds && Seconds > 0)
                timeStr += (Seconds + "s ");

            return timeStr;
        }

        public override string ToString() => GetTimeToStringWithSuffixes(true, true, true);
    }
}