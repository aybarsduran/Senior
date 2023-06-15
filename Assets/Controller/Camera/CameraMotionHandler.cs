using IdenticalStudios.ProceduralMotion;
using UnityEngine;

namespace IdenticalStudios
{
    [RequireComponent( typeof(IMotionDataHandler))]
    public sealed class CameraMotionHandler : MotionMixer, ICameraMotionHandler
    {
        public IMotionMixer MotionMixer => this;
        public IMotionDataHandler DataHandler { get; private set; }
        
        
        private void Awake() => DataHandler = GetComponent<IMotionDataHandler>();
    }
}
