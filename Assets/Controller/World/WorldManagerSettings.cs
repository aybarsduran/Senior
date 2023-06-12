using System;
using UnityEngine;

namespace IdenticalStudios.WorldManagement
{
    [Serializable]
    public sealed class TimeSettings
    {
        [Title("Time & Date")]

        public bool ProgressTime;

        [SpaceArea]

        [Range(0, 24)]
        public int Hour = 12;

        [Range(0, 60)]
        public int Minute;

        [Range(0, 60)]
        public int Second;

        [Title("Cycle Duration")]

        [Range(0, 120)]
        [Tooltip("Day duration in real time minutes.")]
        public float DayDurationInMinutes = 5f;

        [Range(0, 120)]
        [Tooltip("Night duration in real time minutes.")]
        public float NightDurationInMinutes = 5f;
    }

    [Serializable]
    public sealed class LightingSettings
    {
        [Title("References")]
        
        [NotNull]
        public Light SunLight;
        
        [NotNull]
        public Light MoonLight;

        [Title("Light Cycle")]
        
        public LightCycleSettings SunLightCycle;
        public LightCycleSettings MoonLightCycle;

        [Title("Ambient Lights")]
        
        public Gradient AmbientSkyLight;
        public Gradient AmbientEquatorLight;
        public Gradient AmbientGroundLight;
    }

    [Serializable]
    public sealed class SkySettings
    {
        [Title("Sun Motion")]

        [Range(-60, 60)]
        public int SunTilt = 30;

        [Range(-360f, 360f)]
        public int SunRiseAngle = -30;

        [Range(-360f, 360f)]
        public int SunSetAngle = 210;

        [Title("Moon Motion")]

        [Range(-60, 60)]
        public int MoonTilt = 30;
        
        [Range(-360f, 360f)]
        public int MoonRiseAngle = 210;
        
        [Range(-360f, 360f)]
        public int MoonSetAngle = -30;
    }

    [Serializable]
    public sealed class GlobalReflectionsSettings
    {
        public ReflectionProbe ReflectionProbe;

        [Range(0.1f, 1f)]
        public float UpdateInterval = 0.5f;

        [Range(0f, 3f)]
        public float Intensity = 1f;
    }

    [Serializable]
    public sealed class LightCycleSettings
    {
        public Gradient Color;

        [Range(0f, 5f)]
        public float Intensity = 1f;

        public AnimationCurve IntensityCurve;
    }
}