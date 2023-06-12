using System.Collections;
using UnityEngine;

namespace IdenticalStudios.WorldManagement
{
    public sealed class AmbienceController : MonoBehaviour 
	{
		[SerializeField, Range(0.1f, 10f)]
		private float m_TickRate = 1f;

		[Title("Day")]

		[SerializeField]
		private AudioSource m_DayAudioSrc;

		[SerializeField, Range(0f, 1f)]
		private float m_PeakDayVolume = 0.7f;

		[Title("Night")]

		[SerializeField]
		private AudioSource m_NightAudioSrc;

		[SerializeField, Range(0f, 1f)]
		private float m_PeakNightVolume = 0.7f;

		private WaitForSeconds m_TickRateWait;


        private void Start()
        {
			m_TickRateWait = new WaitForSeconds(m_TickRate);
			StartCoroutine(C_UpdateAmbientAudio());
		}

		private IEnumerator C_UpdateAmbientAudio() 
		{
			while (true)
			{
				float currentTime = WorldManagerBase.Instance.GetNormalizedTime();

				m_DayAudioSrc.volume = (Mathf.PingPong(currentTime, 0.5f) * 2) * m_PeakDayVolume;
				m_NightAudioSrc.volume = GetVolumeAtTime(currentTime, 0.25f, 0.75f) * m_PeakNightVolume;

				yield return m_TickRateWait;
			}
		}

		private float GetVolumeAtTime(float time, float minTime, float maxTime)
		{
			if (time <= minTime)
				return 1f - (time * (1 / minTime));
			else if (time >= maxTime)
				return (time - maxTime) * (1 / minTime);

			return 0f;
		}

#if UNITY_EDITOR
		private void OnValidate() => m_TickRateWait = new WaitForSeconds(m_TickRate);
#endif
	}
}
