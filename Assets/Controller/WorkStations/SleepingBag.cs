using IdenticalStudios.WorldManagement;
using System.Collections;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    public sealed class SleepingBag : Interactable, ISleepingPlace
    {
        public Vector3 SleepPosition => transform.position + transform.TransformVector(m_SleepPositionOffset);
        public Vector3 SleepRotation => (Quaternion.LookRotation(transform.up, transform.right) * Quaternion.Euler(m_SleepRotationOffset)).eulerAngles;

        [SerializeField]
        [Help("Position offset for the player sleeping handler. (Where the camera will be positioned).", UnityMessageType.None)]
        private Vector3 m_SleepPositionOffset;

        [SpaceArea(8f)]

        [SerializeField]
        [Help("Rotation offset for the player sleeping handler. (In which direction will the camera be pointed).", UnityMessageType.None)]
        private Vector3 m_SleepRotationOffset;


        public override void OnInteract(ICharacter character)
        {
            if (character.TryGetModule(out ISleepHandler sleepHandler))
            {
                base.OnInteract(character);

                sleepHandler.Sleep(this);
            }
        }

        public override void OnHoverStart(ICharacter character)
        {
            if (!InteractionEnabled)
                return;

            base.OnHoverStart(character);

            StopAllCoroutines();
            StartCoroutine(C_UpdateDescription());
        }

        private IEnumerator C_UpdateDescription()
        {
            while (HoverActive)
            {
                Description = WorldManagerBase.Instance.GetGameTime().GetTimeToString(true, true, false);
                yield return null;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var prevColor = Gizmos.color;
            Gizmos.color = Color.green;

            Gizmos.DrawSphere(SleepPosition, 0.1f);

            Gizmos.color = prevColor;
        }
#endif

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            UnityUtils.SafeOnValidate(this, () =>
            {
                if (TryGetComponent(out FreeBuildable buildable))
                {
                    Title = buildable.Definition.Name;
                    Description = buildable.Definition.Description;
                }
            });
        }
#endif
    }
}