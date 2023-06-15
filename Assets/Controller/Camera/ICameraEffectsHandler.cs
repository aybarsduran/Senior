namespace IdenticalStudios
{
    public interface ICameraEffectsHandler : ICharacterModule
    {
        void DoAnimationEffect(CameraEffectSettings effect);
    }
}