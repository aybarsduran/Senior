using System;
using UnityEngine;

namespace IdenticalStudios.SaveSystem
{
    [ExecuteAlways]
	public class SaveableObject : GuidComponent
	{
		#region Internal
		[Serializable]
		public struct Data
		{
			public string PrefabID;
			public byte[] SceneID;
			public string Name;

			public TransformData Transform;
			public ChildTransformData[] ChildTransforms;
			public ComponentData[] Components;
        }

		[Serializable]
		public struct TransformData
		{
			public Vector3 Position;
			public Quaternion Rotation;
			public Vector3 LocalScale;

			
			public TransformData(Transform transform)
            {
				Position = transform.localPosition;
				Rotation = transform.localRotation;
				LocalScale = transform.localScale;
			}

			public override string ToString() => $"Position: {Position} | Rotation: {Rotation.eulerAngles} | Scale: {LocalScale}";
		}

		[Serializable]
		public struct ChildTransformData
		{
			public string GameObjectPath;
			public TransformData TransformData;


			public ChildTransformData(string gameObjectPath, TransformData transformData)
			{
				GameObjectPath = gameObjectPath;
				TransformData = transformData;
			}
		}

		[Serializable]
		public struct ComponentData
		{
			public string GameObjectPath;
			public Type ComponentType;
			public object[] Members;


			public ComponentData(string gameObjectPath, Type componentType, object[] members)
			{
				Members = members;

				GameObjectPath = gameObjectPath;
				ComponentType = componentType;
			}
		}
		#endregion

		public string PrefabID 
		{
			get => m_PrefabID;
#if UNITY_EDITOR
			set => m_PrefabID = value;
#endif
		}

		[SerializeField, Disable]
		private string m_PrefabID;

		[SpaceArea]

		[SerializeField]
		private bool m_SavePosition = true;

		[SerializeField]
		private bool m_SaveRotation = true;

		[SerializeField]
		private bool m_SaveScale;

		[SpaceArea]

		[SerializeField]
		private bool m_SaveChildTransforms;

		[SpaceArea]

		[SerializeField, ShowIf(nameof(m_SaveChildTransforms), true), ReorderableList]
		private Transform[] m_ChildrenToSave;


		public Data GetSaveData()
		{
			Data data = new()
			{
				PrefabID = m_PrefabID,
				SceneID = GetGuid().ToByteArray(),
				Name = name,
				Transform = new TransformData(transform)
			};

			// Save components
			var savComponents = GetComponentsInChildren<ISaveableComponent>();

			data.Components = new ComponentData[savComponents.Length];
			for (int i = 0;i < savComponents.Length;i++)
			{
				GameObject gameObj = (savComponents[i] as Component).gameObject;
				string gameObjPath = CalculateTransformPath(transform, gameObj.transform);
				data.Components[i] = new ComponentData(gameObjPath, savComponents[i].GetType(), savComponents[i].SaveMembers());
			}

			// Save child transforms
			data.ChildTransforms = new ChildTransformData[m_ChildrenToSave.Length];
            for (int i = 0; i < m_ChildrenToSave.Length; i++)
            {
				Transform child = m_ChildrenToSave[i];
				string gameObjPath = CalculateTransformPath(transform, child);
				data.ChildTransforms[i] = new ChildTransformData(gameObjPath, new TransformData(child));
			}

            return data;
		}

		public void LoadData(Data data)
		{
			gameObject.name = data.Name;
            SetGuid(data.SceneID);

			LoadTransform(transform, data.Transform);

			// Load components
			if (data.Components != null)
			{
                for (int i = 0; i < data.Components.Length; i++)
				{
                    ComponentData compData = data.Components[i];
                    Transform obj = (compData.GameObjectPath != gameObject.name) ? transform.Find(compData.GameObjectPath) : transform;

					if (obj == null)
						continue;

					Component component = obj.GetComponent(compData.ComponentType);

					if (component == null)
						component = obj.gameObject.AddComponent(compData.ComponentType);

					((ISaveableComponent)component).LoadMembers(compData.Members);
                }
			}

			// Load child transforms
			if (data.ChildTransforms != null)
			{
                for (int i = 0; i < data.ChildTransforms.Length; i++)
				{
                    ChildTransformData childData = data.ChildTransforms[i];
                    Transform obj = (childData.GameObjectPath != gameObject.name) ? transform.Find(childData.GameObjectPath) : null;

					if (obj != null)
						LoadTransform(obj, childData.TransformData);
				}
			}
		}

		private void LoadTransform(Transform transform, TransformData data)
		{
			if (m_SavePosition)
				transform.localPosition = data.Position;

			if (m_SaveRotation)
				transform.localRotation = data.Rotation;

			if (m_SaveScale)
				transform.localScale = data.LocalScale;
		}

		protected override void Awake()
		{
            base.Awake();
			LevelManager.RegisterSaveable(this);
		}

        public override void OnDestroy()
		{
            base.OnDestroy();
			LevelManager.UnregisterSaveable(this);
		}

		private static string CalculateTransformPath(Transform root, Transform target)
		{
			if (target == root)
				return string.Empty;;
			
			string path = target.name;
			Transform parent = target.parent;

			while (parent != null && parent != root)
			{
				path = parent.name + (path != string.Empty ? "/" : "") + path;
				parent = parent.parent;
			}

			return path;
		}
	}
}