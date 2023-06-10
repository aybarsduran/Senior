namespace IdenticalStudios.WieldableSystem
{
    public interface IWieldableEffect
    {
#if UNITY_EDITOR
        bool IsInitialized { get; }
#endif

        void OnInitialized(IWieldable wieldable);

        void PlayEffect();
        void PlayEffectDynamically(float value);
    }
}