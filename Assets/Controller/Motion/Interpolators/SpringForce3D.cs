using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    using Random = UnityEngine.Random;

    [Serializable]
    public struct SpringForce3D
    {
        public Vector3 Force;

        [Range(0f, 1f)]
        public float Duration;


        public SpringForce3D(Vector3 force, float duration)
        {
            Force = force;
            Duration = Mathf.Max(0f, duration);
        }

        public bool IsEmpty() => Duration < 0.01f;

        public static SpringForce3D Default => s_Default;
        private static SpringForce3D s_Default = new(Vector3.zero, 0.125f);

        public static SpringForce3D operator +(SpringForce3D springForce, Vector3 force)
        {
            springForce.Force += force;
            return springForce;
        }

        public static SpringForce3D operator *(SpringForce3D springForce, float mod)
        {
            springForce.Force *= mod;
            return springForce;
        }
    }

    [Serializable]
    public struct DelayedSpringForce3D
    {
        [Range(0f, 10f)]
        public float Delay;

        public SpringForce3D SpringForce;


        public DelayedSpringForce3D(SpringForce3D force, float delay)
        {
            Delay = delay;
            SpringForce = force;
        }

        public static DelayedSpringForce3D Default => s_Default;
        private static DelayedSpringForce3D s_Default = new(SpringForce3D.Default, 0f);
    }

    [Serializable]
    public struct RandomSpringForce3D
    {
        public bool RandomSign;

        public Vector3 MinForce;
        public Vector3 MaxForce;

        [Range(0, 100)]
        public float Duration;


        public RandomSpringForce3D(Vector3 minForce, Vector3 maxForce, float duration = 0.125f, bool randomSign = false)
        {
            MinForce = minForce;
            MaxForce = maxForce;

            Duration = duration;
            RandomSign = randomSign;
        }

        public static implicit operator SpringForce3D(RandomSpringForce3D randomSpring)
        {
            var minForce = randomSpring.MinForce;
            var maxForce = randomSpring.MaxForce;

            var x = Random.Range(minForce.x, maxForce.x);
            var y = Random.Range(minForce.y, maxForce.y);
            var z = Random.Range(minForce.z, maxForce.z);

            if (randomSpring.RandomSign)
            {
                x *= Mathf.Sign(Random.Range(-100, 100));
                y *= Mathf.Sign(Random.Range(-100, 100));
                z *= Mathf.Sign(Random.Range(-100, 100));
            }

            return new SpringForce3D(new Vector3(x, y, z), randomSpring.Duration);
        }

        public static RandomSpringForce3D Default => s_Default;
        private static RandomSpringForce3D s_Default = new(Vector3.zero, Vector3.zero);
    }
}