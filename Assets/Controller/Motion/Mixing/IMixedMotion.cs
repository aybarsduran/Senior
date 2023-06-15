using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public interface IMixedMotion
    {
        /// <summary>
        /// How much will this motion affect the final result (0 - 1 range).
        /// </summary>
        float WeightMod { get; set; }

        /// <summary>
        /// The parent mixer.
        /// </summary>
        IMotionMixer MotionMixer { get; }

        /// <summary>
        /// Advances this motion by one frame.
        /// </summary>
        /// <param name="deltaTime"></param>
        void TickMotionLogic(float deltaTime);

        /// <summary>
        /// Return the position offset of this motion in the current frame.
        /// </summary>
        Vector3 GetPosition(float deltaTime);

        /// <summary>
        /// Return the rotation offset of this motion in the current frame.
        /// </summary>
        Quaternion GetRotation(float deltaTime);
    }
}