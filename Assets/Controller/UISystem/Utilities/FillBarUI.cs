using UnityEngine.UI;
using UnityEngine;
using System;

namespace IdenticalStudios.UISystem
{
	[Serializable]
	public sealed class FillBarUI
	{
		[SerializeField]
		private GameObject m_Background;

		[SerializeField]
		private Image m_FillBar;

		[SerializeField]
		private Gradient m_ColorGradient;

		private bool m_Active = true;


		public void SetActive(bool active)
		{
			if (m_Active == active)
				return;

			if (m_Background != null)
				m_Background.SetActive(active);

			if (m_FillBar != null)
				m_FillBar.enabled = active;

			m_Active = active;
		}

		public void SetFillAmount(float fillAmount)
		{
			if (m_FillBar != null)
			{
				m_FillBar.color = m_ColorGradient.Evaluate(fillAmount);
				m_FillBar.fillAmount = fillAmount;
			}
		}

		public void SetAlpha(float alpha)
		{
			if (m_FillBar != null)
				m_FillBar.color = new Color(m_FillBar.color.r, m_FillBar.color.g, m_FillBar.color.b, alpha);
		}
	}
}