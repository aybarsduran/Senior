using System;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    using Object = UnityEngine.Object;

    public class CreateItemPickupPopup : CreateDefinitionPrefabPopup<ItemDefinition>
    {
        #region Internal
        protected enum PickupType
        {
            Standard,
            FirearmWithAttachments
        }

        protected class ItemPickupPrefabInfo : DataDefinitionPrefabInfo
        {
            public PickupType PickupType = PickupType.Standard;

            [Range(1, 24)]
            public int Count = 1;

            public SoundPlayer Sound;

            public bool AddCollider = true;
            public ColliderType ColliderType = ColliderType.Box;
            public bool AddRigidbody = true;
            public float RigidbodyMass = 3f;

            public bool IsSaveable = true;

            [Layer]
            public int Layer = 0;

            public bool AddMaterialEffect = true;
            public MaterialEffectInfo MaterialEffect;

            private static MaterialEffectInfo s_MaterialEffect;
            private static SoundPlayer s_Sound;


            protected override void Awake()
            {
                Layer = LayerMask.NameToLayer("IgnoreCharacter");

                if (s_Sound == null || s_MaterialEffect == null)
                {
                    foreach (var item in ItemDefinition.Definitions)
                    {
                        if (item.Pickup == null)
                            continue;

                        ItemPickup pickup = item.GetPrivateFieldValue<ItemPickup>("m_Pickup");

                        if (s_Sound == null)
                            s_Sound = pickup.GetPrivateFieldValue<SoundPlayer>("m_PickUpSound");

                        if (s_MaterialEffect == null && pickup.TryGetComponent<MaterialEffect>(out var material))
                            s_MaterialEffect = material.GetPrivateFieldValue<MaterialEffectInfo>("m_DefaultEffect");
                    }
                }

                Sound = s_Sound;
                MaterialEffect = s_MaterialEffect;
            }
        }
        #endregion

        private ItemPickupPrefabInfo m_Info;


        public CreateItemPickupPopup(ItemDefinition def, Action<GameObject> getPrefabCallback) : base(def, getPrefabCallback) { }

        protected override DataDefinitionPrefabInfo CreatePrefabInfoInstance()
        {
            m_Info = ScriptableObject.CreateInstance<ItemPickupPrefabInfo>();
            return m_Info;
        }

        protected override string GetPopupName() => "Create Item Pickup";

        protected override void DrawCustomFields()
        {
            using (new CustomGUILayout.VerticalScope("Box"))
            {
                DrawProperty("PickupType");

                if (m_Info.PickupType == PickupType.Standard)
                    DrawProperty("Count");

                DrawProperty("Sound");
            }

            // Collider & Rigidbody
            using (new CustomGUILayout.VerticalScope("Box"))
            {
                DrawProperty("AddCollider");

                if (m_Info.AddCollider)
                {
                    DrawProperty("ColliderType");
                    DrawProperty("AddRigidbody");
                    DrawProperty("RigidbodyMass");
                }
            }

            using (new CustomGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.HelpBox("The default values usually work the best, only change if required.", MessageType.Warning);

                DrawProperty("Layer");
                DrawProperty("IsSaveable");
                DrawProperty("AddMaterialEffect");

                if (m_Info.AddMaterialEffect)
                    DrawProperty("MaterialEffect");
            }
        }

        protected override void HandleCustomComponents(GameObject gameObject, ItemDefinition item)
        {
            gameObject.name = $"(Pickup) {item.Name.Replace(" ", "")}";

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

                if (m_Info.AddRigidbody)
                    gameObject.GetOrAddComponent<Rigidbody>().mass = m_Info.RigidbodyMass;
            }

            // Add material changer..
            if (m_Info.AddMaterialEffect)
                gameObject.GetOrAddComponent<MaterialEffect>().SetFieldValue("m_DefaultEffect", m_Info.MaterialEffect);

            if (m_Info.IsSaveable)
                gameObject.GetOrAddComponent<SaveSystem.SaveableObject>();

            // Set layer..
            gameObject.SetLayerRecursively(m_Info.Layer);


            ItemPickup existingPickup = gameObject.GetComponent<ItemPickup>();
            if (existingPickup != null)
                Object.DestroyImmediate(existingPickup);

            // Add the item pickup component..
            ItemPickup pickup = null;

            switch (m_Info.PickupType)
            {
                case PickupType.Standard:
                    pickup = gameObject.AddComponent<ItemPickup>();
                    break;
                case PickupType.FirearmWithAttachments:
                    pickup = gameObject.AddComponent<FirearmItemPickup>();
                    break;
            }

            pickup.SetFieldValue("m_Item", new DataIdReference<ItemDefinition>(item.Id));
            pickup.SetFieldValue("m_MinCount", m_Info.Count);
            pickup.SetFieldValue("m_MaxCount", m_Info.Count);
            pickup.SetFieldValue("m_PickUpSound", m_Info.Sound);
        }
    }
}