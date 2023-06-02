using IdenticalStudios.ProceduralMotion;

namespace IdenticalStudios
{
    public interface ICameraMotionHandler : ICharacterModule
    {
        IMotionMixer MotionMixer { get; }
        IMotionDataHandler DataHandler { get; }
    }
}