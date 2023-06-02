using IdenticalStudios.ProceduralMotion;

namespace IdenticalStudios
{
    public interface IWieldableMotionHandler : ICharacterModule
    {
        IMotionMixer MotionMixer { get; }
        IMotionDataHandler DataHandler { get; }
    }
}
