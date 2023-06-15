using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public interface ICrosshairHandler
    {
        int CrosshairIndex { get; set;  }
        float Accuracy { get; }

        event UnityAction<int> CrosshairChanged;
        event UnityAction<float> AccuracyChanged;


        void ResetCrosshair();
    }
}