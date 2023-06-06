using IdenticalStudios;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public sealed class InteractionHandler : CharacterBehaviour, IInteractionHandler
    {
        public bool InteractionEnabled
        {
            get => m_InteractionEnabled;
            set
            {
                if (m_InteractionEnabled == value || UnityUtils.IsQuittingPlayMode)
                    return;

                m_InteractionEnabled = value;

                if (m_InteractionEnabled)
                {
                    UpdateManager.AddFixedUpdate(UpdateBehaviour);
                    UpdateBehaviour();
                }
                else
                    UpdateManager.RemoveFixedUpdate(UpdateBehaviour);

                InteractionEnabledChanged?.Invoke(value);
            }
        }

        public HoverInfo HoverInfo => m_HoverInfo;

        public float HoveredObjectDistance
        {
            get
            {
                if (m_HoverInfo.Collider != null)
                    return m_RaycastHit.distance;

                return float.MaxValue;
            }
        }

        public float InteractProgress
        {
            get => m_InteractionProgress;
            private set
            {
                m_InteractionProgress = value;
                InteractProgressChanged?.Invoke(m_InteractionProgress);
            }
        }

        public event UnityAction<IInteractable> Interacted;
        public event UnityAction<HoverInfo> HoverInfoChanged;
        public event UnityAction<float> InteractProgressChanged;
        public event UnityAction<bool> InteractionEnabledChanged;

        [SerializeField]
        [Tooltip("The transform used in raycasting.")]
        private Transform m_View;

        [SerializeField, Range(0.01f, 25f)]
        [Tooltip("The raycast max distance, anything further away will be ignored.")]
        private float m_RaycastDistance = 2.5f;

        [SerializeField]
        [Tooltip("The trigger colliders interaction mode.")]
        private QueryTriggerInteraction m_TriggerInteraction = QueryTriggerInteraction.Ignore;

        [SerializeField]
        [Tooltip("Interaction layer mask, everything this handler can 'see'.")]
        private LayerMask m_LayerMask;

        [SerializeField]
        private StandardSoundWithCooldown m_FailedSound;

        private bool m_IsHoverNull;
        private HoverInfo m_HoverInfo;
        private RaycastHit m_RaycastHit;
        private HoverInfo m_LastHoveredInfo;

        private IInteractable m_Interactable;
        private float m_InteractionProgress;
        private bool m_InteractionEnabled;


        #region Interaction
        public void StartInteraction()
        {
            if (m_Interactable != null)
                return;

            if (m_HoverInfo.IsHoverable && m_HoverInfo.Collider.TryGetComponent(out m_Interactable))
            {
                if (m_Interactable.HoldDuration > 0.01f)
                    StartCoroutine(C_DelayedInteraction());
                else
                    Interact();
            }
            else if (m_FailedSound.Volume > 0.01f)
                Character.AudioPlayer.PlaySound(m_FailedSound);
        }

        public void StopInteraction()
        {
            if (m_Interactable == null)
                return;

            StopAllCoroutines();

            InteractProgress = 0f;
            m_Interactable = null;
        }

        private void Interact()
        {
            if (m_Interactable == null)
                return;

            m_Interactable.OnInteract(Character);
            Interacted?.Invoke(m_Interactable);
        }

        private IEnumerator C_DelayedInteraction()
        {
            float endTime = Time.time + m_Interactable.HoldDuration;

            while (true)
            {
                float time = Time.time;
                InteractProgress = 1 - ((endTime - time) / m_Interactable.HoldDuration);

                if (time < endTime)
                    yield return null;
                else
                    break;
            }

            Interact();
            StopInteraction();
        }
        #endregion

        #region Detection
        private void UpdateBehaviour()
        {
            m_LastHoveredInfo = m_HoverInfo;
            UpdateDetection();
            UpdateHovering();
        }

        private void UpdateHovering()
        {
            if (m_HoverInfo.Collider == m_LastHoveredInfo.Collider)
                return;

            // Hover Start
            if (m_HoverInfo.IsHoverable)
                m_HoverInfo.Hoverable.OnHoverStart(Character);

            // Hover End
            if (m_LastHoveredInfo.IsHoverable)
                m_LastHoveredInfo.Hoverable.OnHoverEnd(Character);

            // Force Stop Interaction
            if (m_Interactable != null)
                StopInteraction();
        }

        private void UpdateDetection()
        {
            var ray = new Ray(m_View.position, m_View.forward);

            if (Physics.Raycast(ray, out m_RaycastHit, m_RaycastDistance, m_LayerMask, m_TriggerInteraction))
            {
                var hitCollider = m_RaycastHit.collider;

                if (m_LastHoveredInfo.Collider != hitCollider)
                {
                    m_HoverInfo = new HoverInfo(hitCollider, hitCollider.GetComponent<IHoverable>());
                    HoverInfoChanged?.Invoke(HoverInfo);
                    m_IsHoverNull = false;
                }
            }
            else if (m_IsHoverNull == false)
            {
                m_HoverInfo = HoverInfo.Default;
                m_IsHoverNull = true;
                HoverInfoChanged?.Invoke(HoverInfo);
            }
        }
        #endregion
    }
}