using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public abstract class CrosshairBaseUI : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public abstract void SetSize(float accuracy);
        public abstract void SetSizeMod(float mod);
        public abstract void SetColor(Color color);
    }
}
