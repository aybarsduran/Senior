using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [RequireComponent(typeof(Canvas), typeof(ChildOfConstraint))]
    public class BlurBackgroundUI : PlayerUIBehaviour
    {
        private static BlurBackgroundUI s_Instance;


        public void EnableBlurredBackground() => gameObject.SetActive(true);
        public void DisableBlurredBackground() => gameObject.SetActive(false);

        public static void EnableBlur()
        {
            if (s_Instance == null)
                return;

            s_Instance.EnableBlurredBackground();
        }

        public static void DisableBlur()
        {
            if (s_Instance == null)
                return;

            s_Instance.DisableBlurredBackground();
        }

        protected override void OnAttachment()
        {
            var camera = GetModule<ICameraFOVHandler>().UnityWorldCamera;
            GetComponent<ChildOfConstraint>().Parent = camera.transform;
            GetComponent<Canvas>().worldCamera = camera;

            DisableBlurredBackground();

            s_Instance = this;
        }
    }
}