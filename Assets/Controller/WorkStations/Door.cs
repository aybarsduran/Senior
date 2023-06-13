using System.Collections;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
	[RequireComponent(typeof(Collider))]
	public sealed class Door : Interactable, ISaveableComponent
	{
		[Title("Animation")]

		[SerializeField]
		[Tooltip("Open rotation offset (how much should this door rotate when it opens).")]
		private Vector3 m_OpenRotation;

		[SerializeField, Range(0.1f, 30f)]
		[Tooltip("Open animation rotation speed.")]
		private float m_AnimationSpeed = 1f;

		[SerializeField, Range(0f, 3f)]
		[Tooltip("Open/Close door time cooldown")]
		private float m_InteractionThreshold = 0.5f;

		[SerializeField, Range(0f, 10f)]
		[Tooltip("How much time should the locked animation last.")]
		private float m_LockedAnimationDuration = 1f;

		[SerializeField, Range(0f, 100f)]
		[Tooltip("How much should the locked animation move the door.")]
		private float m_LockedAnimationRange = 18f;

		[SerializeField]
		[Tooltip("Locked animation randomness.")]
		private Vector2 m_LockedRandomRotation;

		[Title("Audio")]

		[SerializeField]
		[Tooltip("Audio to play when the door opens")]
		private SoundPlayer m_DoorOpen;

		[SerializeField]
		[Tooltip("Audio to play when the tries to open while locked.")]
		public SoundPlayer m_DoorLocked;

		private Collider m_Collider;
		private AudioSource m_AudioSource;
		private float m_NextTimeCanInteract;
		private Quaternion m_ClosedRotation;
		private bool m_IsLocked;


		public override void OnInteract(ICharacter character)
		{
			if (Time.time < m_NextTimeCanInteract)
				return;

			StopAllCoroutines();

			if (!m_IsLocked)
				StartCoroutine(C_DoAnimation(character));
			else
				StartCoroutine(C_DoLockedAnimation());

			m_NextTimeCanInteract = Time.time + m_InteractionThreshold;
		}

        private void Awake()
        {
			m_ClosedRotation = transform.localRotation;
			m_Collider = GetComponent<Collider>();
			m_AudioSource = GetComponent<AudioSource>();
		}

        private IEnumerator C_DoAnimation(ICharacter character)
		{
			bool open = Quaternion.Angle(m_ClosedRotation, transform.localRotation) > 0.5f;
			Quaternion targetRotation = m_ClosedRotation;

			if (!open)
			{
				bool characterIsInFront = Vector3.Dot(character.transform.forward, transform.forward) > 0f;
				Vector3 modelEulerAngles = transform.localEulerAngles;
				targetRotation = Quaternion.Euler(characterIsInFront ? modelEulerAngles + m_OpenRotation : modelEulerAngles - m_OpenRotation);
			}

			bool shouldMove = Quaternion.Angle(targetRotation, transform.localRotation) > 0.5f;

			// Do move animation
			if (shouldMove)
			{
				// Disable Collider
				m_Collider.enabled = false;

				// Audio
				m_DoorOpen.Play(m_AudioSource);

				while (Quaternion.Angle(targetRotation, transform.transform.localRotation) > 0.5f)
				{
					transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * m_AnimationSpeed);
					yield return null;
				}

				// Re-enable Collider
				m_Collider.enabled = true;
			}
		}

		private IEnumerator C_DoLockedAnimation() 
		{
			float stopTime = Time.time + m_LockedAnimationDuration;
			float range = m_LockedAnimationRange;
			float currentVelocity = 0f;

			// Audio
			m_DoorLocked.Play(m_AudioSource);

			Quaternion localRotation = transform.localRotation;

			Vector2 randomRotationRange = m_LockedRandomRotation;
			Quaternion randomRotation = Quaternion.Euler(new Vector2(
				Random.Range(-randomRotationRange.x, randomRotationRange.x),
				Random.Range(-randomRotationRange.y, randomRotationRange.y)));

			while (Time.time < stopTime)
			{
				transform.localRotation = localRotation * randomRotation * Quaternion.Euler(0, Random.Range(-range, range), 0f);
				range = Mathf.SmoothDamp(range, 0f, ref currentVelocity, stopTime - Time.time);

				yield return null;
			}
		}

        #region Internal
        public void LoadMembers(object[] members)
        {
			m_ClosedRotation = (Quaternion)members[0];
			m_IsLocked = (bool)members[1];

			transform.localRotation = (Quaternion)members[2];
		}

        public object[] SaveMembers()
        {
			return new object[] 
			{
				m_ClosedRotation,
				m_IsLocked,
				transform.localRotation		
			};
		}
        #endregion
    }
}
