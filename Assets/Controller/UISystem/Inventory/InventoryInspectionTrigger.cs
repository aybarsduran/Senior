using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public class InventoryInspectionTrigger : PlayerUIBehaviour
    {
        public event UnityAction InspectionStarted
        {
            add => m_InspectionStarted.AddListener(value);
            remove => m_InspectionStarted.RemoveListener(value);
        }

        public event UnityAction InspectionStopped
        {
            add => m_InspectionStopped.AddListener(value);
            remove => m_InspectionStopped.RemoveListener(value);
        }

        [SerializeField]
        private UnityEvent m_InspectionStarted;

        [SerializeField]
        private UnityEvent m_InspectionStopped;


        protected override InitMode GetInitMode() => InitMode.Awake;

        protected override void OnAttachment()
        {
            var inspection = GetModule<IInventoryInspectManager>();
            inspection.AfterInspectionStarted += m_InspectionStarted.Invoke;
            inspection.AfterInspectionEnded += m_InspectionStopped.Invoke;

            if (inspection.IsInspecting)
                m_InspectionStarted.Invoke();
        }

        protected override void OnDetachment()
        {
            var inspection = GetModule<IInventoryInspectManager>();
            inspection.AfterInspectionStarted -= m_InspectionStarted.Invoke;
            inspection.AfterInspectionEnded -= m_InspectionStopped.Invoke;
        }
    }
}
