using UnityEngine;

namespace IdenticalStudios.Demo
{
    public class TeleportPlayerBehaviour : MonoBehaviour
    {
        [SerializeField, ReorderableList(childLabel:"point", HasLabels = false)]
        private Transform[] m_TeleportPoints;


        public void TeleportPlayer(ICharacter character)
        {
            if (character.TryGetModule(out ICharacterMotor motor))
            {
                Transform teleportPoint = m_TeleportPoints.SelectRandom();
                motor.Teleport(teleportPoint.position, teleportPoint.rotation);
            }
        }
    }
}