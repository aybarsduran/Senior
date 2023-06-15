using System;
using UnityEngine;
using UnityEditor;

namespace IdenticalStudios.BuildingSystem
{
    using Object = UnityEngine.Object;

    public class CreateBuildablePopup : CreateDefinitionPrefabPopup<BuildableDefinition>
    {
        #region Internal
        protected enum BuildableType
        {
            Free,
            SocketBased
        }

        protected class BuildablePrefabInfo : DataDefinitionPrefabInfo
        {
            public BuildableType BuildableType = BuildableType.Free;

            [Range(1, 24)]
            public int Count = 1;

            public SoundPlayer Sound;

            public bool AddCollider = true;
            public ColliderType ColliderType = ColliderType.Box;

            [Layer]
            public int Layer;

            public bool IsSaveable = true;
            public bool AddMaterialEffect = true;


            protected override void Awake()
            {
                Layer = LayerMask.NameToLayer("Buildable");
            }
        }
        #endregion

        private BuildablePrefabInfo m_Info;


        public CreateBuildablePopup(BuildableDefinition def, Action<GameObject> getPrefabCallback) : base(def, getPrefabCallback)
        {
        }

        protected override DataDefinitionPrefabInfo CreatePrefabInfoInstance()
        {
            m_Info = ScriptableObject.CreateInstance<BuildablePrefabInfo>();
            return m_Info;
        }

        protected override string GetPopupName() => "Create Buildable";

        protected override void DrawCustomFields()
        {
            using (new CustomGUILayout.VerticalScope("Box"))
            {
                DrawProperty("BuildableType");
                DrawProperty("AddCollider");

                if (m_Info.AddCollider)
                    DrawProperty("ColliderType");
            }

            using (new CustomGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.HelpBox("The default values usually work the best, only change if required.", MessageType.Warning);

                EditorGUI.indentLevel++;
                DrawProperty("Layer");
                DrawProperty("IsSaveable");
                DrawProperty("AddMaterialEffect");
                EditorGUI.indentLevel--;
            }
        }

        protected override void HandleCustomComponents(GameObject gameObject, BuildableDefinition def)
        {
            // Add collider & rigidbody..
            if (m_Info.AddCollider && !gameObject.HasComponent(typeof(Collider)))
            {
                switch (m_Info.ColliderType)
                {
                    case ColliderType.Box:
                        gameObject.GetOrAddComponent<BoxCollider>();
                        break;
                    case ColliderType.Sphere:
                        gameObject.GetOrAddComponent<SphereCollider>();
                        break;
                    case ColliderType.Capsule:
                        gameObject.GetOrAddComponent<CapsuleCollider>();
                        break;
                    case ColliderType.Mesh:
                        gameObject.GetOrAddComponent<MeshCollider>();
                        break;
                }
            }

            if (m_Info.IsSaveable)
                gameObject.GetOrAddComponent<SaveSystem.SaveableObject>();

            // Add material changer..
            if (m_Info.AddMaterialEffect)
                gameObject.GetOrAddComponent<MaterialEffect>();

            // Set layer..
            gameObject.SetLayerRecursively(m_Info.Layer);

            // Add the buildable component..
            Buildable buildable = gameObject.GetComponent<Buildable>();

            switch (m_Info.BuildableType)
            {
                case BuildableType.Free:
                    if (buildable != null && buildable.GetType() != typeof(FreeBuildable))
                        Object.DestroyImmediate(buildable);

                    buildable = gameObject.AddComponent<FreeBuildable>();
                    break;
                case BuildableType.SocketBased:
                    if (buildable != null && buildable.GetType() != typeof(StructureBuildable))
                        Object.DestroyImmediate(buildable);

                    buildable = gameObject.AddComponent<StructureBuildable>();
                    break;
            }

            buildable.SetFieldValue("m_Definition", new DataIdReference<BuildableDefinition>(def.Id));
        }
    }
}
