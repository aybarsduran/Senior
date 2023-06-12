using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class FPCompassRose : WieldableItemBehaviour
    {
        [SerializeField]
        private Transform m_CompassRose;

        [SerializeField]
        private Vector3 m_RotationAxis = new(0, 0, 1);

        [SerializeField, Range(0f, 100f)]
        private float m_RotationSpeed = 3f;


        [SerializeField]
        private DataIdReference<ItemPropertyDefinition> m_DurabilityProperty;

        private IItemProperty m_Durability;
        private Transform m_Root;
        private float m_Angle;


        protected override void OnItemChanged(IItem item)
        {
            if (item != null)
            {
                m_Durability = item.GetPropertyWithId(m_DurabilityProperty);
                m_Root = Item.Wieldable.Character.transform;
            }
            else
            {
                m_Durability = null;
            }
        }
        
        private void LateUpdate()
        {
            if (m_Durability == null || m_Durability.Float < 0.01f)
                return;

            m_Angle = UpdateRoseAngle(m_Angle, m_RotationSpeed * Time.deltaTime);
            m_CompassRose.localRotation = Quaternion.AngleAxis(m_Angle, m_RotationAxis);
        }

        private float UpdateRoseAngle(float angle, float delta)
        {
            angle = Mathf.LerpAngle(angle, Vector3.SignedAngle(m_Root.forward, Vector3.forward, Vector3.up), delta);
            angle = Mathf.Repeat(angle, 360f);

            return angle;
        }
    }
}