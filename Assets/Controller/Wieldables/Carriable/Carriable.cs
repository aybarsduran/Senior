using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class Carriable : Wieldable, ICarriable
    {
        public DataIdReference<CarriableDefinition> Definition => m_Definition;

        public int CarryCount => m_CarryCount;
        public int MaxCarryCount => m_Objects.Length;

        [Title("Carriable")]

        [SerializeField, NotNull]
        private CarriableActionBehaviour m_UseAction;

        [SerializeField, Range(0.01f, 30f)]
        private float m_BaseWeight = 2f;

        [SerializeField]
        private ObjectDropper.DropSettings m_DropSettings;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        [Help("The length of this collection represents the max carry count.")]
        private GameObject[] m_Objects;

        [Title("Effects")]

        [SerializeField]
        private EffectCollection m_CarryEffects;

        [SerializeField]
        private EffectCollection m_UseEffects;

        [SerializeField]
        private EffectCollection m_DropEffects;

        private DataIdReference<CarriableDefinition> m_Definition;
        private ObjectDropper m_ObjectDropper;
        private int m_CarryCount;


        public override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);

            if (visible)
                UpdateVisuals();
        }

        public void InitializeCarriable(DataIdReference<CarriableDefinition> definition, ObjectDropper objectDropper)
        {
            m_Definition = definition;
            m_ObjectDropper = objectDropper;
        }

        public T GetCarriableActionOfType<T>() where T : CarriableActionBehaviour
        {
            if (m_UseAction is T action)
                return action;

            return null;
        }

        public bool TryAddCarriable(int count)
        {
            if (m_Definition.IsNull || count <= 0 || m_CarryCount == MaxCarryCount)
                return false;

            m_CarryCount = Mathf.Clamp(m_CarryCount + Mathf.Abs(count), 0, MaxCarryCount);
            Weight = m_BaseWeight * m_CarryCount;

            UpdateVisuals();
            m_CarryEffects.PlayEffects(this);

            return true;
        }

        /// <summary>
        /// Tries to use this carriable.
        /// </summary>
        /// <returns> Used count / the amount that will be removed. </returns>
        public bool TryUseCarriable()
        {
            if (m_CarryCount == 0 || IsEquipping())
                return false;

            if (m_UseAction != null && m_UseAction.TryUseCarriable())
            {
                m_CarryCount--;
                UpdateVisuals();
                m_UseEffects.PlayEffects(this);
                return true;
            }

            return false;
        }

        public bool TryDropCarriable(int count, float dropHeight)
        {
            if (count <= 0 || m_CarryCount == 0 || IsEquipping())
                return false;

            m_CarryCount = Mathf.Clamp(m_CarryCount - Mathf.Abs(count), 0, MaxCarryCount);

            if (m_ObjectDropper != null)
            {
                for (int i = 0; i < count; i++)
                    m_ObjectDropper.DropObject(m_DropSettings, m_Definition.Def.Pickup.gameObject, dropHeight);
            }

            UpdateVisuals();
            m_DropEffects.PlayEffects(this);

            return true;
        }

        private void UpdateVisuals()
        {
            for (int i = 0; i < m_Objects.Length; i++)
            {
                if (m_Objects[i] != null)
                {
                    bool showObject = m_CarryCount > i;
                    m_Objects[i].SetActive(showObject);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_UseAction == null)
                m_UseAction = GetComponent<CarriableActionBehaviour>();
        }
#endif
    }
}