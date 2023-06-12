namespace IdenticalStudios.WorldManagement
{
    public abstract class WorldManagerBase : Singleton<WorldManagerBase>
    {
        public const float k_DefaultDayDurationInMinutes = 20f;
        public const float k_DefaultTemperatureInCelsius = 20f;


        public virtual bool TimeProgressionEnabled { get; set; }

        public virtual float GetNormalizedTime() => 0f; 
        public virtual TimeOfDay GetTimeOfDay() => TimeOfDay.Day;
        public virtual GameTime GetGameTime() => new GameTime(0f, 0f, 0f);
        public virtual GameDate GetGameDate() => new GameDate(2023, Month.January, 1);
        public virtual float GetTemperature() => k_DefaultTemperatureInCelsius;
        public virtual float GetDayDurationInMinutes() => k_DefaultDayDurationInMinutes;
        public virtual float GetTimeIncrementPerSecond() => 0f;

        /// <summary>
        /// A time to pass of 1 is equal to a full day, duration is seconds.
        /// </summary>
        /// <param name="timeToPass"> normalized time</param>
        /// <param name="duration"> duration in seconds</param>
        public virtual void PassTime(float timeToPass, float duration) { }
    }

    public enum TimeOfDay
    {
        Day, Night
    }
}