using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public class SurvivalBookUI : PlayerUIBehaviour
    {
        protected IWieldableSurvivalBookHandler SurvivalBook
        {
            get => GetModule<IWieldableSurvivalBookHandler>();
        }

        [SerializeField]
        private Canvas m_MenuCanvas;

        [SerializeField]
        private CanvasGroup m_MenuCanvasGroup;

        [SerializeField]
        private ChildOfConstraint m_MenuParent;

        [SpaceArea]

        [SerializeField]
        private Canvas m_ContentCanvas;

        [SerializeField]
        private CanvasGroup m_ContentCanvasGroup;

        [SerializeField]
        private ChildOfConstraint m_ContentParent;

        [SpaceArea]

        [SerializeField]
        private UnityEvent m_ShowEvent;

        [SerializeField]
        private UnityEvent m_HideEvent;

        private ChildOfLookHandler m_LookHandler;


        public void StartBookInspection() => SurvivalBook.TryStartInspection();
        public void StopBookInspection() => SurvivalBook.TryStopInspection(null);

        public void StartCustomBuilding()
        {
            var customBuildings = BuildingManager.CustomBuildingDefinitions;
            var buildableDef = customBuildings[(int)Mathf.Repeat(BuildingManager.DefaultBuildableIndex, customBuildings.Length)];
            SurvivalBook.TryStopInspection(buildableDef);
        }

        protected override void OnAttachment()
        {
            var cam = GetModule<ICameraFOVHandler>().UnityOverlayCamera;
            var book = SurvivalBook;

            m_MenuCanvas.enabled = false;
            m_ContentCanvas.enabled = false;

            m_MenuCanvas.worldCamera = cam;
            m_ContentCanvas.worldCamera = cam;

            m_MenuCanvasGroup.interactable = false;
            m_ContentCanvasGroup.interactable = false;

            m_MenuParent.Parent = book.LeftPages;
            m_ContentParent.Parent = book.RightPages;

            m_LookHandler = book.gameObject.GetComponent<ChildOfLookHandler>();
            var updateMode = m_LookHandler == null ? ChildOfConstraint.UpdateMode.Automatic : ChildOfConstraint.UpdateMode.Manual;

            m_MenuParent.UpdateType = updateMode;
            m_ContentParent.UpdateType = updateMode;

            book.InspectionStarted += ShowUI;
            book.InspectionEnded += HideUI;

        }

        protected override void OnDetachment()
        {
            var book = SurvivalBook;
            book.InspectionStarted -= ShowUI;
            book.InspectionEnded -= HideUI;
        }

        private void ShowUI()
        {
            m_LookHandler.OnMove += m_MenuParent.UpdateTransform;
            m_LookHandler.OnMove += m_ContentParent.UpdateTransform;

            m_MenuCanvas.enabled = true;
            m_ContentCanvas.enabled = true;

            m_MenuCanvasGroup.interactable = true;
            m_ContentCanvasGroup.interactable = true;

            m_ShowEvent.Invoke();

            BlurBackgroundUI.EnableBlur();
        }

        private void HideUI()
        {
            m_MenuCanvasGroup.interactable = false;
            m_ContentCanvasGroup.interactable = false;

            StartCoroutine(C_HideUI());
            m_HideEvent.Invoke();

            BlurBackgroundUI.DisableBlur();
        }

        private IEnumerator C_HideUI()
        {
            yield return new WaitForSeconds(SurvivalBook.AttachedWieldable.HolsterDuration);

            m_MenuCanvas.enabled = false;
            m_ContentCanvas.enabled = false;

            m_LookHandler.OnMove -= m_MenuParent.UpdateTransform;
            m_LookHandler.OnMove -= m_ContentParent.UpdateTransform;
        }
    }
}
