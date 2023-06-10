namespace IdenticalStudios.WieldableSystem
{
    public interface IRecoil
    {
        float RecoilHeatRecover { get; }
        float HipfireAccuracyKick { get; }
        float HipfireAccuracyRecover { get; }
        float AimAccuracyKick { get; }
        float AimAccuracyRecover { get; }

        void DoRecoil(bool isAiming, float heatValue, float triggerValue);
        void SetRecoilMultiplier(float multiplier);

        void Attach();
        void Detach();
    }
}