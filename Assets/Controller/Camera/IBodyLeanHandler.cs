namespace IdenticalStudios
{
    public interface IBodyLeanHandler : ICharacterModule
    {
        BodyLeanState LeanState { get; }

        void SetLeanState(BodyLeanState leanState);
    }

    public enum BodyLeanState
    {
        Center = 0,
        Left = -1,
        Right = 1
    }
}
