using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [System.Serializable]
    public struct SpringSettings
    {
        [Range(0f, 100f)]
        public float Damping;

        [Range(0f, 1000f)]
        public float Stiffness;

        [Range(0f, 10f)]
        public float Mass;

        [Range(0f, 10f)]
        public float Speed;


        public SpringSettings(float damping, float stiffness, float mass, float speed)
        {
            this.Damping = damping;
            this.Stiffness = stiffness;
            this.Mass = mass;
            this.Speed = speed; ;
        }

        private static SpringSettings s_Default = new(10f, 120f, 1f, 1f);
        public static SpringSettings Default => s_Default;

        private static SpringSettings s_Null = new(0f, 0f, 0f, 0f);
        public static SpringSettings Null => s_Null;

        public readonly bool IsNull()
        {
            bool nullOrEmpty = Damping < 0.01f || Stiffness < 0.01f;
            return nullOrEmpty;
        }
    }
}