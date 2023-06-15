namespace IdenticalStudios.WieldableSystem
{
    public interface IUseInputHandler
    {
        ActionBlockHandler UseBlocker { get; }
        
        void Use(UsePhase usePhase);
    }

    public enum UsePhase
    {
        Start,
        Hold,
        End
    }
}