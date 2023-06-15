using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios.UISystem
{
    /// <summary>
    /// Basic UI Panel that can be toggled on and off.
    /// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	[AddComponentMenu("IdenticalStudios/UserInterface/Panels/Panel")]
	public class PanelUI : MonoBehaviour
	{
		public CanvasGroup CanvasGroup => m_CanvasGroup;
		public bool IsVisible { get; private set; }

		[SerializeField, FormerlySerializedAs("m_ShowOnEnable")]
		private bool m_ShowOnStart;

		[SerializeField]
		private CanvasGroup m_CanvasGroup;


		/// <summary>
		/// Show/Hide the panel.
		/// </summary>
		public void Show(bool show)
		{
			if (IsVisible != show)
				Show_Internal(show);
		}

		protected void Show_Internal(bool show)
		{
#if UNITY_EDITOR
			if (UnityUtils.IsQuittingPlayMode)
				return;
#endif

			m_CanvasGroup.interactable = show;
			m_CanvasGroup.blocksRaycasts = show;

			bool wasVisible = IsVisible;
			IsVisible = show;

			if (show) ShowPanel();
			else HidePanel();
		}

		protected virtual void Start() => Show_Internal(m_ShowOnStart);
		protected virtual void ShowPanel() => m_CanvasGroup.alpha = 1f;
		protected virtual void HidePanel() => m_CanvasGroup.alpha = 0f;

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();	
		}
#endif
	}
}
