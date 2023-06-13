using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace IdenticalStudios.SaveSystem
{
	[Serializable]
	public class SceneData
	{
		public string Name;
		public readonly Dictionary<string, SaveableObject.Data> Objects = new Dictionary<string, SaveableObject.Data>();
	}

	[Serializable]
	public class GameData : IDeserializationCallback
	{
		public int SaveId
		{
			get => m_SaveId; 
			set => m_SaveId = value;
		}
		
		public SceneData SceneData => m_SceneData;
		public DateTime DateTime { get => m_DateTime; set => m_DateTime = value; }
		public Texture2D Screenshot => m_Screenshot;

        [SerializeField]
		private int m_SaveId;

        [SerializeField]
        private SceneData m_SceneData;

        [SerializeField]
        private DateTime m_DateTime;

		[NonSerialized]
		private Texture2D m_Screenshot;

		[SerializeField]
		private byte[] m_ScreenshotData;

		[SerializeField]
		private int m_ScreenshotWidth, m_ScreenshotHeight;


		public GameData(int saveId, SceneData sceneData, Texture2D screenshot = null)
		{
			m_SaveId = saveId;
			m_SceneData = sceneData;
			m_DateTime = DateTime.Now;

			SetScreenshot(screenshot);
		}

		public void SetScreenshot(Texture2D screenshot)
		{
			if (screenshot == null)
				return;
			
			m_Screenshot = screenshot;
			m_ScreenshotData = screenshot.EncodeToJPG();
			m_ScreenshotWidth = screenshot.width;
			m_ScreenshotHeight = screenshot.height;
		}

		public void OnDeserialization(object sender)
		{
			if (m_ScreenshotData == null || m_ScreenshotData.Length <= 0)
				return;
			
			m_Screenshot = new Texture2D(m_ScreenshotWidth, m_ScreenshotHeight, TextureFormat.RGB24, true);
			m_Screenshot.LoadImage(m_ScreenshotData);
			m_Screenshot.Apply(true);
		}
	}
}