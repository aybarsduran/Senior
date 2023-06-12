namespace IdenticalStudios.WieldableSystem
{
    public interface IMeleeSwing
    {
        float SwingDuration { get; }
        float AttackEffort { get; }


        bool GetSwingValidity(float accuracy);
        void DoSwing(float accuracy);
        void CancelSwing();
    }
}