using IdenticalStudios.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    /// <summary>
    /// Handles any type of inventory inspection (e.g. Backpack, external containers etc.)
    /// </summary>
    public sealed class InventoryInspectManager : CharacterBehaviour, IInventoryInspectManager
    {
        public bool IsInspecting => m_IsInspecting;

        public IWorkstation Workstation { get; private set; }

        public event UnityAction BeforeInspectionStarted;
        public event UnityAction AfterInspectionStarted;
        public event UnityAction BeforeInspectionEnded;
        public event UnityAction AfterInspectionEnded;

        [SerializeField]
        private InputContextGroup m_InventoryContext;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How often can the inventory inspection be toggled (e.g. open/close backpack).")]
        private float m_ToggleThreshold = 0.35f;

        private bool m_IsInspecting;
        private float m_NextAllowedToggleTime;
        private UnityAction m_EndInspectionCallback;
        
        
        public void StartInspection(IWorkstation workstation, UnityAction inspectionStartCallback = null, UnityAction inspectionEndCallback = null)
        {
            bool sameWorkstation = workstation != null && Workstation == workstation;

            if (m_IsInspecting || Time.time < m_NextAllowedToggleTime || sameWorkstation)
                return;

            m_EndInspectionCallback = inspectionEndCallback;

            Workstation = workstation;

            m_IsInspecting = true;
            m_NextAllowedToggleTime = Time.time + m_ToggleThreshold;

            CursorLocker.AddCursorUnlocker(this);
            InputManager.PushEscapeCallback(ForceStopInspection);
            InputManager.PushContext(m_InventoryContext);

            BeforeInspectionStarted?.Invoke();
            inspectionStartCallback?.Invoke();
            AfterInspectionStarted?.Invoke();
        }

        public void StopInspection()
        {
            if (!m_IsInspecting)
                return;

            m_EndInspectionCallback?.Invoke();

            Workstation = null;

            m_IsInspecting = false;
            m_NextAllowedToggleTime = Time.time + m_ToggleThreshold;

            CursorLocker.RemoveCursorUnlocker(this);
            InputManager.PopEscapeCallback(ForceStopInspection);
            InputManager.PopContext(m_InventoryContext);

            BeforeInspectionEnded?.Invoke();
            AfterInspectionEnded?.Invoke();
        }

        private void ForceStopInspection() => StopInspection();
    }
}