using UnityEngine;

namespace IdenticalStudios
{
    public interface ICameraFOVHandler : ICharacterModule
    {
        Camera UnityWorldCamera { get; }
        Camera UnityOverlayCamera { get; }

        void SetCustomOverlayFOV(float fov, float duration = 0.3f, float delay = 0f);
        void ResetCustomOverlayFOV(bool instantly = false);
        void SetCustomWorldFOVMod(float fovMod, float duration = 0.3f, float delay = 0f);
        void ResetCustomWorldFOV(bool instantly = false);
    }
}