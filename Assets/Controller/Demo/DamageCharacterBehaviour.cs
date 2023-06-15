using UnityEngine;

namespace IdenticalStudios.Demo
{
    public class DamageCharacterBehaviour : MonoBehaviour
    {
        [SerializeField, MinMaxSlider(0f, 100f)]
        private Vector2 m_Damage;

        [SerializeField, Range(0f, 100f)]
        private float m_HitImpulse = 5f;


        public void DamageCharacter(ICharacter character)
        {
            DamageContext dmgContext = new(DamageType.Cut, transform.position, (transform.position - character.transform.position).normalized * m_HitImpulse, Vector3.zero, null);
            character.HealthManager.ReceiveDamage(m_Damage.GetRandomFloat(), dmgContext);
        }
    }
}