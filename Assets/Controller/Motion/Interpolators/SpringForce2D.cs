using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public struct SpringForce2D
    {
        public Vector2 Force;

        [Range(0, 100)]
        public float Duration;
		
		
        public SpringForce2D(Vector2 force, float frames)
        {
            Force = force;
            Duration = Mathf.Max(0, frames);
        }

        public bool IsEmpty() => Duration == 0;

        public static SpringForce2D Default => s_Default;
        private static SpringForce2D s_Default = new(Vector3.zero, 0.125f);

        public static SpringForce2D operator *(SpringForce2D springForce, float mod)
        {
            springForce.Force *= mod;
            return springForce;
        }
    }
	
    [Serializable]
    public struct DelayedSpringForce2D
    {
        [Range(0f, 10f)]
        public float Delay;
		
        public SpringForce2D SpringForce;


        public DelayedSpringForce2D(SpringForce2D force, float delay)
        {
            Delay = delay;
            SpringForce = force;
        }

        public static DelayedSpringForce2D Default => s_Default;
        private static DelayedSpringForce2D s_Default = new(SpringForce2D.Default, 0f);
    }
}
