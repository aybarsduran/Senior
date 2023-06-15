using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class MessageDisplayerUI : MonoBehaviour
	{
		#region Internal
		private class MessageTemplateData
		{
			public readonly GameObject Root;
			public readonly TextMeshProUGUI Text;
			public readonly Image IconImg;
			public readonly CanvasGroup CanvasGroup;


			public MessageTemplateData(GameObject objectTemplate, Transform spawnRoot)
            {
				GameObject instance = Instantiate(objectTemplate, spawnRoot);
				this.Root = instance;
				this.Text = instance.GetComponentInChildren<TextMeshProUGUI>();
				this.IconImg = instance.transform.Find("Icon").GetComponent<Image>();
				this.CanvasGroup = instance.GetComponentInChildren<CanvasGroup>();
				this.CanvasGroup.alpha = 0f;
            }
		}
        #endregion

		[SerializeField]
		private GameObject m_MessageTemplate;

		[SerializeField, Range(1, 30)]
		private int m_TemplatesCount = 10;

		[SerializeField]
		private Color m_MessageColor;

		[SerializeField, Range(0f, 10f)]
		private float m_FadeDelay = 3f;

		[SerializeField, Range(0f, 10f)]
		private float m_FadeSpeed = 0.3f;

		private List<MessageTemplateData> m_MessageTemplates;
		private int m_CurrentIndex = -1;

		private static MessageDisplayerUI s_Instance;


		private void OnEnable()
        {
			s_Instance = this;
			m_MessageTemplates = new List<MessageTemplateData>();

			for (int i = 0; i < m_TemplatesCount; i++)
				m_MessageTemplates.Add(new MessageTemplateData(m_MessageTemplate, transform));
		}

		private void OnDisable()
        {
			if (s_Instance == this)
			{
				s_Instance = null;
				m_MessageTemplates.Clear();
			}
		}

        public static void PushMessage(string message)
		{
			if (s_Instance != null)
				s_Instance.Internal_PushMessage(message, s_Instance.m_MessageColor, null);
		}

		public static void PushMessage(string message, Sprite sprite)
		{
			if (s_Instance != null)
				s_Instance.Internal_PushMessage(message, s_Instance.m_MessageColor, sprite);
		}

		public static void PushMessage(string message, Color color)
		{
			if (s_Instance != null)
				s_Instance.Internal_PushMessage(message, color, null);
		}

        private void Internal_PushMessage(string message, Color color = default, Sprite sprite = null)
		{
			var template = GetMessageTemplate();

			template.Root.SetActive(true);
			template.Root.transform.SetAsLastSibling();

			template.Text.text = message.ToUpper();
			template.Text.color = new Color(color.r, color.g, color.b, 1f);

			template.IconImg.gameObject.SetActive(sprite != null);
			template.IconImg.sprite = sprite;

			template.CanvasGroup.alpha = color.a;
			StartCoroutine(FadeMessage(template.CanvasGroup));
		}

		private MessageTemplateData GetMessageTemplate() => m_MessageTemplates[(int)Mathf.Repeat(m_CurrentIndex++, m_TemplatesCount)];

		private IEnumerator FadeMessage(CanvasGroup group)
		{
			yield return new WaitForSeconds(m_FadeDelay);
			
			while (group.alpha > Mathf.Epsilon)
			{
				group.alpha = Mathf.MoveTowards(group.alpha, 0f, Time.deltaTime * m_FadeSpeed);
				yield return null;
			}
		}
	}
}
