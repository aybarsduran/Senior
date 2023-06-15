namespace IdenticalStudios.WieldableSystem
{
    public interface IReloadInputHandler
    {
        bool IsReloading { get; }
        ActionBlockHandler ReloadBlocker { get; }

        void StartReloading();
        void CancelReloading();
    }
}