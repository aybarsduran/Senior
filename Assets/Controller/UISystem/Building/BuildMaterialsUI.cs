using IdenticalStudios.BuildingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class BuildMaterialsUI : PlayerUIBehaviour
    {
        [Title("Canvas")]

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField, Range(0f, 30f)]
        private float m_AlphaTweenDuration = 0.35f;

        [Title("Build Materials")]

        [SerializeField]
        private RectTransform m_BuildMaterialsRoot;

        [SerializeField]
        private Image m_AddAllMaterialsProgressImg;

        [SerializeField, PrefabObjectOnly]
        private RequirementUI m_BuildMaterialTemplate;

        [SerializeField, Range(3, 30)]
        private int m_CachedBuildMaterialCount = 10;

        [Title("Cancelling")]

        [SerializeField]
        private Image m_CancelProgressImg;

        private RequirementUI[] m_MaterialDisplayers;
        private IStructureDetector m_StructureManager;
        private BuildablePreview m_StructureInView;
        private RectTransform m_RectTransform;
        private Camera m_Camera;
        private float m_TargetAlpha;


        protected override void OnAttachment()
        {
            m_MaterialDisplayers = new RequirementUI[m_CachedBuildMaterialCount];
            for (int i = 0; i < m_CachedBuildMaterialCount; i++)
            {
                m_MaterialDisplayers[i] = Instantiate(m_BuildMaterialTemplate, m_BuildMaterialsRoot);
                m_MaterialDisplayers[i].gameObject.SetActive(false);
            }

            m_Camera = GetModule<ICameraFOVHandler>().UnityWorldCamera;
            m_RectTransform = GetComponent<RectTransform>();

            m_CanvasGroup.alpha = 0f;
            m_CancelProgressImg.fillAmount = 0f;
            m_AddAllMaterialsProgressImg.fillAmount = 0f;

            GetModule(out m_StructureManager);
            m_StructureManager.StructureChanged += OnStructureInViewChanged;
            m_StructureManager.CancelPreviewProgressChanged += OnCancelProgressChanged;
        }

        protected override void OnDetachment()
        {
            if (m_StructureManager != null)
            {
                m_StructureManager.StructureChanged -= OnStructureInViewChanged;
                m_StructureManager.CancelPreviewProgressChanged -= OnCancelProgressChanged;
            }
        }

        private void LateUpdate()
        {
            if (m_StructureInView != null)
            {
                UpdateBuildRequirementsPosition();
                m_TargetAlpha = 1f;
            }
            else
                m_TargetAlpha = 0f;

            m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, m_TargetAlpha, Time.fixedDeltaTime * m_AlphaTweenDuration);
        }

        private void OnCancelProgressChanged(float progress) => m_CancelProgressImg.fillAmount = progress;

        private void OnStructureInViewChanged(BuildablePreview preview)
        {
            if (m_StructureInView != null)
                m_StructureInView.MaterialAdded -= OnMaterialAdded;

            m_StructureInView = preview;
            m_TargetAlpha = preview != null ? 1f : 0f;

            if (m_StructureInView != null)
            {
                m_StructureInView.MaterialAdded += OnMaterialAdded;
                UpdateBuildRequirementsInfo();
            }

            void OnMaterialAdded(bool completed)
            {
                if (completed)
                    m_StructureInView = null;
                else
                    UpdateBuildRequirementsInfo();
            }
        }

        private void UpdateBuildRequirementsPosition()
        {
            Vector2 screenPositionOfPreview = m_Camera.WorldToScreenPoint(m_StructureInView.PreviewCenter);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)m_RectTransform.parent, screenPositionOfPreview, null, out Vector2 positionOfUI))
                m_RectTransform.localPosition = positionOfUI;
        }

        private void UpdateBuildRequirementsInfo()
        {
            for (int i = 0; i < m_MaterialDisplayers.Length; i++)
            {
                RequirementUI displayer = m_MaterialDisplayers[i];

                var buildRequirements = m_StructureInView.GetAllBuildRequirements();

                if (buildRequirements != null && i < buildRequirements.Count)
                {
                    displayer.gameObject.SetActive(true);

                    BuildRequirement requirement = buildRequirements[i];

                    var buildMaterial = BuildMaterialDefinition.GetWithId(requirement.BuildMaterial);

                    if (buildMaterial != null)
                        displayer.Display(buildMaterial.Icon, requirement.CurrentAmount + "/" + requirement.RequiredAmount);
                }
                else
                    displayer.gameObject.SetActive(false);
            }
        }
    }
}