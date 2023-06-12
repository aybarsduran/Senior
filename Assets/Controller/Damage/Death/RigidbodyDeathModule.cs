using UnityEngine;

namespace IdenticalStudios
{
    public sealed class RigidbodyDeathModule : MonoBehaviour, IDeathModule
    {
        [SerializeField]
        private Transform m_CharacterView;

        [SerializeField]
        private Rigidbody m_HeadRigidbody;

        [SerializeField]
        private Collider m_HeadCollider;

        private Transform m_CameraStartParent;
        private Quaternion m_CameraStartRotation; 
        private Vector3 m_CameraStartPosition;

        private Vector3 m_HeadStartPosition;
        private Quaternion m_HeadStartRotation;


        public void DoDeathEffects(ICharacter character)
        {
            Vector3 velocity = character.GetModule<ICharacterMotor>().Velocity;

            m_HeadCollider.isTrigger = false;
            m_HeadRigidbody.gameObject.SetActive(true);
            m_HeadRigidbody.isKinematic = false;
            m_HeadRigidbody.AddForce(Vector3.ClampMagnitude(velocity * 0.5f, 1f), ForceMode.Impulse);
            m_HeadRigidbody.AddRelativeTorque(new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * 35, ForceMode.Impulse);

            m_CharacterView.transform.SetParent(m_HeadRigidbody.transform, false);
            m_CharacterView.localRotation = Quaternion.identity;
            m_CharacterView.localPosition = Vector3.zero;
        }

        public void DoRespawnEffects(ICharacter character)
        {
            m_CharacterView.SetParent(m_CameraStartParent, true);
            m_CharacterView.localRotation = m_CameraStartRotation;
            m_CharacterView.localPosition = m_CameraStartPosition;

            m_HeadCollider.isTrigger = true;
            m_HeadRigidbody.isKinematic = true;
            m_HeadRigidbody.transform.localPosition = m_HeadStartPosition;
            m_HeadRigidbody.transform.localRotation = m_HeadStartRotation;
            m_HeadRigidbody.gameObject.SetActive(false);
        }

        private void Awake()
        {
            m_HeadCollider.isTrigger = true;
            m_HeadRigidbody.isKinematic = true;
            m_HeadRigidbody.gameObject.SetActive(false);

            // Camera set up
            m_CameraStartRotation = m_CharacterView.localRotation;
            m_CameraStartPosition = m_CharacterView.localPosition;
            m_CameraStartParent = m_CharacterView.parent;

            // Player head set up
            m_HeadStartPosition = m_HeadRigidbody.transform.localPosition;
            m_HeadStartRotation = m_HeadRigidbody.transform.localRotation;
        }
    }
}
