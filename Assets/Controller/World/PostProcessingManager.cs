using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace IdenticalStudios.WorldManagement
{
    [DefaultExecutionOrder(-1000)]
    public sealed class PostProcessingManager : Singleton<PostProcessingManager>
    {
        public static PostProcessVolume OverlayVolume => Instance.m_OverlayVolume;
        public static PostProcessVolume WorldVolume => Instance.m_WorldVolume;

        private DepthOfField DepthOfField
        {
            get
            {
                if (m_DepthOfField == null)
                    m_DepthOfField = m_OverlayVolume.profile.GetSetting<DepthOfField>();

                return m_DepthOfField;
            }
        }

        [SerializeField]
        private PostProcessVolume m_OverlayVolume;

        [SerializeField]
        private PostProcessVolume m_WorldVolume;

        private DepthOfField m_DepthOfField;


        public void EnableDOF(bool enable) => DepthOfField.active = enable;
        public void SetDOFFocalLength(float focalLenght) => DepthOfField.focalLength.Override(Mathf.Max(1f, focalLenght));
    }
}