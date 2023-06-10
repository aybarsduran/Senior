namespace IdenticalStudios.WieldableSystem
{
    public interface IAimer
    {
        bool IsAiming { get; }

        float HipAccuracyMod { get; }
        float AimAccuracyMod { get; }

        void StartAim();
        void EndAim();

        void Attach();
        void Detach();
    }
}