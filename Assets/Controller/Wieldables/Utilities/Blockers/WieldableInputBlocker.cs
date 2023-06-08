using System.Collections;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [RequireComponent(typeof(IWieldablesController))]
    public abstract class WieldableInputBlocker : CharacterBehaviour
    {
        [SerializeField]
        [Tooltip("How fast should this behaviour be updated, set it to a lower value for more consistency and higher for better performance.")]
        [Range(0.01f, 10f)]
        private float m_TickRate = 0.15f;

        [SerializeField]
        [Tooltip("For how much time should the corresponding action be put to 'sleep' (unable to start) after it's been blocked.")]
        [Range(0.01f, 10f)]
        private float m_Cooldown = 0.35f;

        private float m_NextTimeCanDoAction;
        private bool m_IsBlocked;

        private WaitForSeconds m_UpdateWait;
        private ActionBlockHandler m_BlockHandler;


        protected override void OnBehaviourEnabled()
        {
            m_UpdateWait = new WaitForSeconds(m_TickRate);

            var controller = GetModule<IWieldablesController>();
            controller.WieldableEquipStarted += OnWieldableEquipped;

            if (controller.ActiveWieldable != null)
                OnWieldableEquipped(controller.ActiveWieldable);
        }

        protected override void OnBehaviourDisabled()
        {
            var controller = GetModule<IWieldablesController>();
            controller.WieldableEquipStarted -= OnWieldableEquipped;

            if (m_IsBlocked && m_BlockHandler != null)
            {
                m_BlockHandler.RemoveBlocker(this);
                m_IsBlocked = false;
            }

            OnWieldableEquipped(null);
        }

        private void OnWieldableEquipped(IWieldable wieldable)
        {
            m_IsBlocked = false;
            m_NextTimeCanDoAction = 0f;

            if (wieldable != null)
            {
                if (m_BlockHandler == null)
                    StartCoroutine(C_Update());
                else
                {
                    if (m_IsBlocked)
                        m_BlockHandler.RemoveBlocker(this);
                }

                m_BlockHandler = GetBlockHandler(wieldable);
            }
            else
            {
                StopAllCoroutines();
                m_BlockHandler = null;
            }
        }

        private IEnumerator C_Update() 
        {
            yield return null;

            while (true)
            {
                if (m_IsBlocked && Time.time < m_NextTimeCanDoAction)
                {
                    yield return null;
                    continue;
                }

                yield return m_UpdateWait;

                if (m_BlockHandler == null)
                    break;

                if (!m_IsBlocked)
                    OnUpdate();

                bool isValid = IsInputValid();

                if (!isValid)
                    m_NextTimeCanDoAction = Time.time + m_Cooldown;

                if (!m_IsBlocked && !isValid)
                {
                    m_IsBlocked = true;
                    m_BlockHandler.AddBlocker(this);
                }
                else if (m_IsBlocked && isValid)
                {
                    m_IsBlocked = false;
                    m_BlockHandler.RemoveBlocker(this);
                }
            }
        }

        protected abstract ActionBlockHandler GetBlockHandler(IWieldable wieldable);
        protected abstract bool IsInputValid();

        /// <summary>
        /// Called only when this action is not blocked.
        /// </summary>
        protected virtual void OnUpdate() { }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            m_UpdateWait = new WaitForSeconds(m_TickRate);
        }
#endif
    }
}