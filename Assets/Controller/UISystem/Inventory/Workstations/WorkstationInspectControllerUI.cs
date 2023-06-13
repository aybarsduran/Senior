using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public interface IWorkstationInspector
    {
        Type WorkstationType { get; }

        void Inspect(IWorkstation workstation);
        void EndInspection();
    }

    public sealed class WorkstationInspectControllerUI : PlayerUIBehaviour
    {
        private readonly Dictionary<Type, IWorkstationInspector> m_WorkstationInspectors = new();

        [SerializeField, NotNull]
        private GameObject m_InspectorsRoot;

        private IInventoryInspectManager m_InventoryInspector;
        private IWorkstationInspector m_ActiveInspector;


        protected override void OnAttachment()
        {
            GetModule(out m_InventoryInspector);

            m_InventoryInspector.BeforeInspectionStarted += OnInspectionStarted;
            m_InventoryInspector.AfterInspectionEnded += OnInspectionEnded;

            InitializeWorkstations();
        }

        protected override void OnDetachment()
        {
            m_InventoryInspector.BeforeInspectionStarted -= OnInspectionStarted;
            m_InventoryInspector.AfterInspectionEnded -= OnInspectionEnded;
        }

        private void InitializeWorkstations()
        {
            var objInspectors = m_InspectorsRoot.GetComponentsInFirstChildren<IWorkstationInspector>();
            foreach (IWorkstationInspector inspector in objInspectors)
            {
                if (!m_WorkstationInspectors.ContainsKey(inspector.WorkstationType))
                {
                    m_WorkstationInspectors.Add(inspector.WorkstationType, inspector);
                    inspector.EndInspection();
                }
            }
        }

        private void OnInspectionStarted()
        {
            if (m_InventoryInspector.Workstation == null)
                return;

            var workstationType = m_InventoryInspector.Workstation.GetType();

            if (m_WorkstationInspectors.TryGetValue(workstationType, out IWorkstationInspector objInspector))
            {
                objInspector.Inspect(m_InventoryInspector.Workstation);
                m_ActiveInspector = objInspector;
            }
        }

        private void OnInspectionEnded()
        {
            if (m_ActiveInspector != null)
                m_ActiveInspector.EndInspection();

            m_ActiveInspector = null;
        }
    }
}