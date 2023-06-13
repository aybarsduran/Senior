using IdenticalStudios.BuildingSystem;
using IdenticalStudios.ProceduralMotion;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class BuildSelectionUI : PlayerUIBehaviour
    {
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0f, 5f)]
        private float m_AlphaLerpDuration = 0.35f;

        [SpaceArea]

        [SerializeField]
        private Image m_PreviousImg;

        [SerializeField]
        private Image m_CurrentImg;

        [SerializeField]
        private Image m_NextImg;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        private GameObject[] m_SocketBuildableObjects;

        [SerializeField]
        private GameObject[] m_FreeBuildableObjects;

        private Tween<float> m_CanvasGroupTween;
        private IBuildingController m_BuildingController;


        protected override void OnAttachment()
        {
            m_CanvasGroupTween = m_CanvasGroup.TweenCanvasGroupAlpha(0f, m_AlphaLerpDuration).SetEase(EaseType.SineInOut).SetFrom(0f);
            m_CanvasGroup.alpha = 0f;

            GetModule(out m_BuildingController);
            m_BuildingController.BuildableChanged += CurrentBuildableChanged;
        }

        protected override void OnDetachment()
        {
            m_BuildingController.BuildableChanged += CurrentBuildableChanged;
        }

        public void CurrentBuildableChanged(BuildableDefinition def)
        {
            if (def == null)
            {
                m_CanvasGroupTween.SetTargetValue(0f);
                m_CanvasGroupTween.Play();
            }
            else
            {
                m_CanvasGroupTween.SetTargetValue(1f);
                m_CanvasGroupTween.Play();

                var mode = m_BuildingController.Mode;

                for (int i = 0; i < m_SocketBuildableObjects.Length; i++)
                    m_SocketBuildableObjects[i].SetActive(mode == BuildingMode.Socket);

                for (int i = 0; i < m_FreeBuildableObjects.Length; i++)
                    m_FreeBuildableObjects[i].SetActive(mode == BuildingMode.Free);

                if (mode == BuildingMode.Socket)
                {
                    var customBuildables = BuildingManager.CustomBuildingDefinitions;

                    int currentIdx = customBuildables.GetIndexOfDefinition(def);
                    int previousIdx = (int)Mathf.Repeat(currentIdx - 1, customBuildables.Length);
                    int nextIdx = (int)Mathf.Repeat(currentIdx + 1, customBuildables.Length);

                    if (currentIdx != -1)
                    {
                        m_CurrentImg.sprite = customBuildables[currentIdx].ParentGroup.Icon;
                        m_PreviousImg.sprite = customBuildables[previousIdx].ParentGroup.Icon;
                        m_NextImg.sprite = customBuildables[nextIdx].ParentGroup.Icon;
                    }
                }
            }
        }
    }
}