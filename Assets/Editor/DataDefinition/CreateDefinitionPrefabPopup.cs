using UnityEditor;
using System;
using UnityEngine;

namespace IdenticalStudios
{
    using Object = UnityEngine.Object;

    public abstract class CreateDefinitionPrefabPopup<T> : PopupWindowContent where T : DataDefinition<T>
    {
        #region Internal
        protected abstract class DataDefinitionPrefabInfo : ScriptableObject
        {
            public GameObject Model;


            protected abstract void Awake();
        }

        protected enum ColliderType
        {
            Box,
            Sphere,
            Capsule,
            Mesh
        }
        #endregion

        private readonly T m_Definition;
        private readonly DataDefinitionPrefabInfo m_Info;
        private readonly SerializedObject m_Object;
        private readonly Action<GameObject> m_PrefabCallback;

        private Vector2 m_ScrollPos;


        public CreateDefinitionPrefabPopup(T def, Action<GameObject> getPrefabCallback)
        {
            this.m_Info = CreatePrefabInfoInstance();
            this.m_Object = new SerializedObject(m_Info);
            this.m_Definition = def;
            this.m_PrefabCallback = getPrefabCallback;
        }

        public override Vector2 GetWindowSize() => new Vector2(400f, 500f);

        public override void OnGUI(Rect rect)
        {
            m_Object.Update();

            // Title
            using (new CustomGUILayout.HorizontalScope("Box"))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(GetPopupName());
                GUILayout.FlexibleSpace();
            }

            // Main
            using (new CustomGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.HelpBox("Corresponding mesh or prefab", MessageType.Info);
                DrawProperty("Model");
            }

            using (var scroll = new GUILayout.ScrollViewScope(m_ScrollPos))
            {
                m_ScrollPos = scroll.scrollPosition;

                if (IsEnabled())
                    DrawCustomFields();

                GUILayout.FlexibleSpace();
            }

            GUI.enabled = IsEnabled();

            if (CustomGUILayout.ColoredButton("Create", CustomGUIStyles.GreenColor))
                CreateItem();

            GUI.enabled = true;

            m_Object.ApplyModifiedProperties();
        }

        private void CreateItem()
        {
            // Instantiate model..
            GameObject objToInstantiate = m_Info.Model;
            GameObject gameObject = Object.Instantiate(objToInstantiate);

            gameObject.name = $"({DataDefinitionUtility.GetAssetNamePrefix(typeof(T))}) {m_Definition.Name.Replace(" ", "")}";

            // Add the custom components to it (e.g. colliders, rigidbody etc.)
            HandleCustomComponents(gameObject, m_Definition);

            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, GetCreationPath(gameObject), InteractionMode.UserAction);

            editorWindow.Close();

            m_PrefabCallback?.Invoke(prefab);
        }

        protected abstract DataDefinitionPrefabInfo CreatePrefabInfoInstance();
        protected abstract string GetPopupName();
        protected abstract void DrawCustomFields();
        protected abstract void HandleCustomComponents(GameObject gameObject, T def);

        protected virtual string GetCreationPath(GameObject gameObject)
        {
            if (!AssetDatabase.IsValidFolder("Assets/IdenticalStudios/_Custom"))
                AssetDatabase.CreateFolder("Assets/IdenticalStudios", "_Custom");

            string localPath = "Assets/IdenticalStudios/_Custom/" + gameObject.name + ".prefab";

            return AssetDatabase.GenerateUniqueAssetPath(localPath);
        }

        protected bool IsEnabled() => m_Info.Model != null;
        protected void DrawProperty(string name) => EditorGUILayout.PropertyField(m_Object.FindProperty(name));
    }
}
