using System;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public abstract class WorkstationInspectorBaseUI<T> : PlayerUIBehaviour, IWorkstationInspector where T : Workstation
    {
        public Type WorkstationType => typeof(T);

        [SerializeField, NotNull]
        private SelectableTabUI m_Tab;

        protected T m_Workstation;


        public void Inspect(IWorkstation workstation)
        {
            m_Workstation = (T)workstation;

            m_Tab.TabName = workstation.WorkstationName;
            m_Tab.gameObject.SetActive(true);
            m_Tab.Select();

            OnInspectionStarted(m_Workstation);
        }

        public void EndInspection()
        {
            if (m_Workstation != null)
                OnInspectionEnded(m_Workstation);

            m_Workstation = null;
            m_Tab.gameObject.SetActive(false);
        }

        protected override InitMode GetInitMode() => InitMode.Awake;
        protected abstract void OnInspectionStarted(T workstation);
        protected abstract void OnInspectionEnded(T workstation);

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            m_Tab = GetComponent<SelectableTabUI>();
        }
#endif
    }
}
