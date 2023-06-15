using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace IdenticalStudios
{
    /// <summary>
    /// Utility Audio Class
    /// </summary>
    [CreateAssetMenu(menuName = "Identical Studios/Managers/Audio", fileName = "AudioManager")]
    public sealed class AudioManager : Manager<AudioManager>
	{
		#region Internal
		public enum MixerOutputGroup
		{
			Master = 0,
			Effects = 1,
			Ambient = 2,
			Music = 3
		}

		private class RuntimeObject : MonoBehaviour
		{
			public AudioSource AudioSource { get; private set; }

			private void Awake() => AudioSource = gameObject.AddComponent<AudioSource>();
		}
		#endregion
		
		#region Initialization
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Init() => SetInstance();
		
		protected override void OnInitialized()
		{
			m_RuntimeObject = CreateRuntimeObject<RuntimeObject>("AudioRuntimeObject");
			m_RuntimeObject.AudioSource.outputAudioMixerGroup = m_EffectsGroup;
		}
		#endregion
		
		[SerializeField]
		private AudioMixer m_AudioMixer;

		[SerializeField]
		private AudioMixerGroup m_EffectsGroup;

		private static Dictionary<string, AudioMixerGroup> s_AudioMixerGroups;
		private RuntimeObject m_RuntimeObject;

		
		public static void Play2D(AudioClip clip, float volume)
		{
			Instance.m_RuntimeObject.AudioSource.PlayOneShot(clip, volume);
		}

		public static AudioSource CreateAudioSource(GameObject objectToAddTo, bool is2D = false, float startVolume = 1f, float minDistance = 1f, MixerOutputGroup outputGroup = MixerOutputGroup.Master)
		{
			AudioSource audioSource = objectToAddTo.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.volume = startVolume;
			audioSource.spatialBlend = is2D ? 0f : 1f;
			audioSource.minDistance = minDistance;

			SetAudioSourceOutput(audioSource, outputGroup.ToString());

			return audioSource;
		}

		public static AudioSource CreateAudioSource(string sourceName, Transform parent, Vector3 localPosition, bool is2D = false, float startVolume = 1f, float minDistance = 1f, MixerOutputGroup outputGroup = MixerOutputGroup.Master) 
		{
			GameObject audioObject = new GameObject(sourceName, typeof(AudioSource));
			
			audioObject.transform.parent = parent;
			audioObject.transform.localPosition = localPosition;
			AudioSource audioSource = audioObject.GetComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.volume = startVolume;
			audioSource.spatialBlend = is2D ? 0f : 1f;
			audioSource.minDistance = minDistance;

			SetAudioSourceOutput(audioSource, outputGroup.ToString());

			return audioSource;
		}
		
		private static void SetAudioSourceOutput(AudioSource audioSource, string outputGroupName = "Master")
		{
			if (s_AudioMixerGroups == null)
				GenerateAudioMixerGroupsDictionary();

			if (s_AudioMixerGroups.TryGetValue(outputGroupName, out AudioMixerGroup outputGroup))
				audioSource.outputAudioMixerGroup = outputGroup;
			else
				Debug.LogError($"The audio mixer group ''{outputGroupName}'' could not be found in the mixer!");
		}

		private static void GenerateAudioMixerGroupsDictionary()
		{
			s_AudioMixerGroups = new Dictionary<string, AudioMixerGroup>();
			var audioMixerGroups = Instance.m_AudioMixer.FindMatchingGroups("");

			foreach (var group in audioMixerGroups)
				s_AudioMixerGroups.Add(group.name, group);
		}
	}
}