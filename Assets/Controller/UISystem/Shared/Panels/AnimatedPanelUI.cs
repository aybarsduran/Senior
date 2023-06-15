using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [RequireComponent(typeof(Animator))]
	[AddComponentMenu("IdenticalStudios/UserInterface/Panels/Animated Panel")]
	public class AnimatedPanelUI : AudioPanelUI
	{
		[Title("Animation")]

		[SerializeField]
		private Animator m_Animator;

		[SerializeField, Range(0f, 2f)]
		private float m_ShowSpeed = 1f;

		[SerializeField, Range(0f, 2f)]
		private float m_HideSpeed = 1f;

		private static readonly int m_HashedShowTrigger = Animator.StringToHash("Show");
		private static readonly int m_HashedHideTrigger = Animator.StringToHash("Hide");
		private static readonly int m_HashedShowSpeedTrigger = Animator.StringToHash("Show Speed");
		private static readonly int m_HashedHideSpeedTrigger = Animator.StringToHash("Hide Speed");


		protected override void ShowPanel()
        {
			base.ShowPanel();
			m_Animator.SetTrigger(m_HashedShowTrigger);
		}

        protected override void HidePanel()
        {
			base.HidePanel();
			m_Animator.SetTrigger(m_HashedHideTrigger);
		}	

        protected override void Start()
        {
			m_Animator.SetFloat(m_HashedShowSpeedTrigger, m_ShowSpeed);
			m_Animator.SetFloat(m_HashedHideSpeedTrigger, m_HideSpeed);

			base.Start();
		}

#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			m_Animator = GetComponent<Animator>();
		}
#endif
	}
}