using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public struct SpringForce1D
    {
        public float Force;
        
        [Range(0, 100)]
        public float Duration;
        
        
        public SpringForce1D(float force, float duration)
        {
            Force = force;
            Duration = Mathf.Max(0, duration);
        }
        
        public static SpringForce1D Default => new(0f, 0.125f);
        
        public static SpringForce1D operator *(SpringForce1D springForce, float mod)
        {
            springForce.Force *= mod;
            return springForce;
        }
    }
	
    [Serializable]
    public struct DelayedSpringForce1D
    {
        [Range(0f, 2f)]
        public float Delay;

        public SpringForce1D SpringForce;
		

        public DelayedSpringForce1D(SpringForce1D force, float delay)
        {
            Delay = delay;
            SpringForce = force;
        }

        public static DelayedSpringForce1D Default => s_Default;
        private static DelayedSpringForce1D s_Default = new DelayedSpringForce1D(SpringForce1D.Default, 0f);
    }
}
