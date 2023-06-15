using System.Collections;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
	public sealed class DamageUI : PlayerUIBehaviour
	{
		[SerializeField, Help("How much will the alpha of the effects be affected by the amount of damage received."), Range(0f, 1f)]
		[Tooltip("How much will the alpha of the effects be affected by the amount of damage received.")]
		private float m_AlphaDamageWeight;

		[Title("Blood Screen")]

		[SerializeField]
		[Tooltip("Image fading settings for the blood screen.")]
		private ImageFaderUI m_BloodScreenFader;

		[SerializeField, Range(0f, 100f)]
		[Tooltip("How much damage does the player have to take for the blood screen effect to show. ")]
		private float m_BloodScreenDamageThreshold = 5f;

		[Title("Directional Indicator")]

		[SerializeField]
		[Tooltip("Image fading settings for the directional damage indicator.")]
		private ImageFaderUI m_DirectionalIndicatorFader;

		[SerializeField, Range(0f, 1024)]
		[Tooltip("Damage indicator distance (in pixels) from the screen center.")]
		private int m_DamageIndicatorDistance = 128;

		private Vector3 m_LastHitPoint;
		private RectTransform m_DamageIndicatorRT;
		private IHealthManager m_HealthManager;

		private Coroutine m_SpriteDirectionHandler;


		protected override void OnAttachment()
		{
			GetModule(out m_HealthManager);

			m_HealthManager.DamageTakenFullContext += OnTakeDamage;
			m_DamageIndicatorRT = m_DirectionalIndicatorFader.Image.rectTransform;
		}

		private void OnTakeDamage(float damage, DamageContext dmgInfo)
		{
			float targetAlpha =  ((m_HealthManager.PrevHealth - m_HealthManager.Health) / 100f) - m_AlphaDamageWeight;

			if (damage > 1f)
			{
				if (dmgInfo.HitPoint != Vector3.zero)
				{
					m_LastHitPoint = dmgInfo.HitPoint;
					m_DirectionalIndicatorFader.DoFadeCycle(this, targetAlpha);

					if (m_SpriteDirectionHandler != null)
						StopCoroutine(m_SpriteDirectionHandler);

					m_SpriteDirectionHandler = StartCoroutine(C_UpdateDirectionalSpriteDirection());
				}

				if (damage > m_BloodScreenDamageThreshold)
					m_BloodScreenFader.DoFadeCycle(this, targetAlpha);
			}
		}

		private IEnumerator C_UpdateDirectionalSpriteDirection() 
		{
			while (m_DirectionalIndicatorFader.Fading)
			{
				Vector3 lookDir = Vector3.ProjectOnPlane(Player.ViewTransform.forward, Vector3.up).normalized;
				Vector3 dirToPoint = Vector3.ProjectOnPlane(m_LastHitPoint - Player.transform.position, Vector3.up).normalized;

				Vector3 rightDir = Vector3.Cross(lookDir, Vector3.up);

				float angle = Vector3.Angle(lookDir, dirToPoint) * Mathf.Sign(Vector3.Dot(rightDir, dirToPoint));

				m_DamageIndicatorRT.localEulerAngles = Vector3.forward * angle;
				m_DamageIndicatorRT.localPosition = m_DamageIndicatorRT.up * m_DamageIndicatorDistance;

				yield return null;
			}

			m_SpriteDirectionHandler = null;
		}
	}
}
