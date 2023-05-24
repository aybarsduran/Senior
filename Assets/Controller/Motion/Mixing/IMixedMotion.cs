using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public interface IMixedMotion
    {
        // How much will this motion affect the final result (0 - 1 range).
        float WeightMod { get; set; }

        // The parent mixer.
        IMotionMixer MotionMixer { get; }

        
        // Advances this motion by one frame.
        
        
        void TickMotionLogic(float deltaTime);

       
        // Return the position offset of this motion in the current frame.
        Vector3 GetPosition(float deltaTime);

      
        // Return the rotation offset of this motion in the current frame.
        Quaternion GetRotation(float deltaTime);
    }
}