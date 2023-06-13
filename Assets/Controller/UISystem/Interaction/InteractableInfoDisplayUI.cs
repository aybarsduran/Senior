using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    #region Internal
    public interface IInteractableInfoDisplayer
    {
        IEnumerable<Type> HoverableTypes { get; }

        void ShowInfo(IHoverable hoverable);
        void UpdateInfo(IHoverable hoverable);
        void SetInteractionProgress(float interactProgress);
        void HideInfo();
    }
    #endregion

    public sealed class InteractableInfoDisplayUI : PlayerUIBehaviour
    {
        [SerializeField, NotNull]
        private GameObject m_InfoDisplayersRoot;

        private readonly Dictionary<Type, IInteractableInfoDisplayer> m_ObjectInfoDisplayers = new();

        private IInteractableInfoDisplayer m_ActiveInfoDisplayer;
        private IInteractableInfoDisplayer m_DefaultInfoDisplayer;
        private IInteractionHandler m_InteractionHandler;


        protected override void OnAttachment()
        {
            InitInfoDisplayers();

            GetModule(out m_InteractionHandler);
            m_InteractionHandler.HoverInfoChanged += OnHoverInfoChanged;
            m_InteractionHandler.InteractProgressChanged += SetInteractionProgress;
            m_InteractionHandler.InteractionEnabledChanged += OnEnabledStateChanged;
        }

        protected override void OnDetachment()
        {
            m_InteractionHandler.HoverInfoChanged -= OnHoverInfoChanged;
            m_InteractionHandler.InteractProgressChanged -= SetInteractionProgress;
            m_InteractionHandler.InteractionEnabledChanged -= OnEnabledStateChanged;
        }

        private void InitInfoDisplayers()
        {
            var objInfoDisplayers = m_InfoDisplayersRoot.GetComponentsInFirstChildren<IInteractableInfoDisplayer>();
            for (int i = 0; i < objInfoDisplayers.Count; i++)
            {
                IInteractableInfoDisplayer infoDisplayer = objInfoDisplayers[i];
                foreach (var interactableType in infoDisplayer.HoverableTypes)
                {
                    if (!m_ObjectInfoDisplayers.ContainsKey(interactableType))
                        m_ObjectInfoDisplayers.Add(interactableType, infoDisplayer);
                }
            }

            m_DefaultInfoDisplayer = m_ObjectInfoDisplayers.GetValueOrDefault(typeof(IInteractable));
        }

        private void OnEnabledStateChanged(bool enable)
        {
            if (m_ActiveInfoDisplayer == null)
                return;

            if (enable)
            {
                var hoverable = m_InteractionHandler.HoverInfo.Hoverable;

                if (hoverable.IsHoverable)
                {
                    m_ActiveInfoDisplayer.ShowInfo(hoverable);
                    m_ActiveInfoDisplayer.UpdateInfo(hoverable);
                }
            }
            else
                m_ActiveInfoDisplayer.HideInfo();
        }

        private void SetInteractionProgress(float progress)
        {
            if (m_ActiveInfoDisplayer != null)
                m_ActiveInfoDisplayer.SetInteractionProgress(progress);
        }

        private void OnHoverInfoChanged(HoverInfo hoverInfo)
        {
            if (hoverInfo.IsHoverable)
            {
                if (!m_ObjectInfoDisplayers.TryGetValue(hoverInfo.Hoverable.GetType(), out var infoDisplayer))
                    infoDisplayer = m_DefaultInfoDisplayer;

                if (infoDisplayer != m_ActiveInfoDisplayer)
                {
                    m_ActiveInfoDisplayer?.HideInfo();
                    m_ActiveInfoDisplayer = infoDisplayer;

                    if (m_ActiveInfoDisplayer == null)
                        return;

                    m_ActiveInfoDisplayer.ShowInfo(hoverInfo.Hoverable);
                    m_ActiveInfoDisplayer.UpdateInfo(hoverInfo.Hoverable);
                }
                else
                {
                    m_ActiveInfoDisplayer?.UpdateInfo(hoverInfo.Hoverable);
                }
            }
            else
            {
                m_ActiveInfoDisplayer?.HideInfo();
                m_ActiveInfoDisplayer = null;
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_InfoDisplayersRoot = gameObject;
        }
#endif
    }
}