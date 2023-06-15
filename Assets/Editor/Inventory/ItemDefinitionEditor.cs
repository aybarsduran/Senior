using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomEditor(typeof(ItemDefinition))]
    public class ItemDefinitionEditor : DataDefinitionBaseEditor
    {
        private ItemDefinition m_Item;

        private GUIStyle BoxStyle => new GUIStyle("Box");
        private Rect m_CreatePickupPopupRect;


        protected override void DrawCustomInspector()
        {
            if (serializedObject.targetObject == null)
                return;
            
            GUI.color = Color.white;
            serializedObject.Update();

            DrawItemName();
            DrawNonInteractableInfo();
            DrawItemInfo();
            DrawItemData();

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_Item = target as ItemDefinition;
        }

        #region Item Info
        private void DrawItemName() 
        {
            using (new CustomGUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_ItemName");
            }
        }

        private void DrawNonInteractableInfo()
        {
            using (new CustomGUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_Id");

                GUI.enabled = false;
                EditorGUILayout.ObjectField("Asset", m_Item, typeof(ItemDefinition), false);
                GUI.enabled = true;

                DrawCustomProperty("m_ParentGroup");

                EditorGUILayout.Space();
            }
        }

        private void DrawItemInfo() 
        {
            using (new CustomGUILayout.VerticalScope(BoxStyle))
            {
                //Draw icon property..
                if (serializedObject.FindProperty("m_Icon").objectReferenceValue == null)
                {
                    using (new CustomGUILayout.HorizontalScope(BoxStyle))
                    {
                        DrawCustomProperty("m_Icon");

                        if (GUILayout.Button("Find Matching Icon"))
                            FindMatchingIcon();
                    }
                }
                else
                    DrawCustomProperty("m_Icon");

                // Draw pickup property..
                if (serializedObject.FindProperty("m_Pickup").objectReferenceValue == null)
                {
                    using (new CustomGUILayout.HorizontalScope(BoxStyle))
                    {
                        DrawCustomProperty("m_Pickup");

                        if (CustomGUILayout.ColoredButton("Find Matching Pickup", CustomGUIStyles.YellowColor))
                            FindMatchingPickup();

                        if (CustomGUILayout.ColoredButton("Create Pickup", CustomGUIStyles.GreenColor))
                            CreatePickupPrefab();

                        if (Event.current.type == EventType.Repaint) m_CreatePickupPopupRect = GUILayoutUtility.GetLastRect();
                    }
                }
                else
                    DrawCustomProperty("m_Pickup");

                // Draw description property..
                DrawCustomProperty("m_Description");

                GUILayout.Space(3f);

                // Draw description property..
                DrawCustomProperty("m_Weight");

                // Draw stack size property..
                DrawCustomProperty("m_StackSize");
            }
        }

        private void FindMatchingIcon()
        {
            var matchingSprite = AssetDatabaseHelper.FindClosestMatchingObjectWithName<Sprite>(GetCorrectItemName());

            if (matchingSprite != null)
                serializedObject.FindProperty("m_Icon").SetValue(matchingSprite);
        }

        private void FindMatchingPickup()
        {
            var matchingPickup = AssetDatabaseHelper.FindClosestMatchingPrefab<ItemPickup>(GetCorrectItemName(), "(Pickup)");

            if (matchingPickup != null)
                serializedObject.FindProperty("m_Pickup").SetValue(matchingPickup);
        }

        private void CreatePickupPrefab()
        {
            PopupWindow.Show(m_CreatePickupPopupRect, new CreateItemPickupPopup(m_Item, AssignPickupPrefab));
        }

        private void AssignPickupPrefab(GameObject prefab)
        {
            serializedObject.FindProperty("m_Pickup").SetValue(prefab.GetComponent<ItemPickup>());
        }

        private string GetCorrectItemName() 
        {
            if (string.IsNullOrEmpty(m_Item.Name))
                return m_Item.name;

            return m_Item.Name;
        }
        #endregion

        #region Item Properties
        private void DrawItemData()
        {
            using (new CustomGUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_Tag");
            }

            using (new CustomGUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_Actions");

                if (m_Item.IsPartOfGroup)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        var actions = m_Item.ParentGroup.BaseActions;
                        for (int i = 0; i < actions.Length; i++)
                            EditorGUILayout.ObjectField(actions[i], typeof(ItemAction), false);
                    }
                }
            }

            using (new CustomGUILayout.VerticalScope(BoxStyle))
            {
                DrawCustomProperty("m_Properties");
                DrawCustomProperty("m_Data");
            }
        }
        #endregion

        #region Item Data

        //private SerializedProperty m_DataProperty;
        //private static int s_SelectedDataTypeIndex = 0;
        //private static Type[] s_ItemDataTypes;
        //private static string[] s_ItemDataTypeNames;

        //private void DrawItemData()
        //{
        //    if (s_ItemDataTypeNames == null || s_ItemDataTypes == null)
        //        return;

        //    using (new CustomGUILayout.VerticalScope(BoxStyle))
        //    {
        //        m_DataProperty = serializedObject.FindProperty("m_Data");
        //        DrawCustomProperty("m_Data");

        //        using (new CustomGUILayout.HorizontalScope(BoxStyle))
        //        {
        //            s_SelectedDataTypeIndex = EditorGUILayout.Popup(s_SelectedDataTypeIndex, s_ItemDataTypeNames);

        //            GUILayout.Space(5f);

        //            GUI.enabled = CanAddDataType(s_ItemDataTypes[s_SelectedDataTypeIndex]);

        //            if (CustomGUILayout.ColoredButton("Add Selected", CustomGUIStyles.GreenColor))
        //                AddItemDataOfType(s_ItemDataTypes[s_SelectedDataTypeIndex]);

        //            GUI.enabled = true;
        //        }

        //        if (CustomGUILayout.ColoredButton("Delete All", CustomGUIStyles.RedColor))
        //            DeleteAllData();
        //    }

        //    DeleteUnlinkedItemDataAssets();
        //}

        //private void AddItemDataOfType(Type dataType)
        //{
        //    // Create new data scriptable object of the selected type.
        //    var dataSO = ScriptableObject.CreateInstance(dataType);
        //    dataSO.name = dataType.Name.ToUnityLikeNameFormat();

        //    var dataArray = m_DataProperty.GetValue<ItemData[]>();
        //    ArrayUtility.Add(ref dataArray, dataSO as ItemData);
        //    m_DataProperty.SetValue(dataArray);
        //    serializedObject.ApplyModifiedProperties();

        //    // Add the created data as a child to the item definition.
        //    AssetDatabase.AddObjectToAsset(dataSO, m_Item);
        //    AssetDatabase.SaveAssets();

        //    EditorUtility.SetDirty(dataSO);
        //    EditorUtility.SetDirty(m_Item);
        //}

        //private void DeleteAllData() 
        //{
        //    m_DataProperty.ClearArray();
        //    serializedObject.ApplyModifiedProperties();
        //}

        //private void DeleteUnlinkedItemDataAssets() 
        //{
        //    var allChildData = GetAllChildDatas();
        //    var linkedData = m_DataProperty.GetValue<ItemData[]>();
        //    bool dirty = false;

        //    foreach (var data in allChildData)
        //    {
        //        if (!linkedData.Contains(data))
        //        {
        //            if (data != null)
        //            {
        //                Undo.DestroyObjectImmediate(data);
        //                dirty = true;
        //            }
        //        }
        //    }

        //    if (dirty)
        //        AssetDatabase.SaveAssets();
        //}

        //private ItemData[] GetAllChildDatas()
        //{
        //    var assetPath = AssetDatabase.GetAssetPath(m_Item);
        //    var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

        //    List<ItemData> childData = new List<ItemData>();

        //    foreach (var asset in assets)
        //    {
        //        if (asset as ItemData != null)
        //            childData.Add(asset as ItemData);
        //    }

        //    return childData.ToArray();
        //}

        //private bool CanAddDataType(Type type)
        //{
        //    var itemData = m_DataProperty.GetValue<ItemData[]>();

        //    if (itemData == null)
        //        return false;

        //    foreach (var data in itemData)
        //    {
        //        if (data == null)
        //            return false;

        //        if (data.GetType() == type)
        //            return false;
        //    } 

        //    return true;
        //}

        //private void InitializeItemData()
        //{
        //    s_ItemDataTypes = GetAllItemDataTypes();
        //    s_ItemDataTypeNames = new string[s_ItemDataTypes.Length];

        //    for (int i = 0; i < s_ItemDataTypes.Length; i++)
        //        s_ItemDataTypeNames[i] = s_ItemDataTypes[i].Name.ToUnityLikeNameFormat();
        //}

        //private Type[] GetAllItemDataTypes() 
        //{
        //    var itemDataType = typeof(ItemData);
        //    var assembly = Assembly.GetAssembly(itemDataType);
        //    var types = assembly.GetTypes().Where(t => t != itemDataType && itemDataType.IsAssignableFrom(t)).ToArray();

        //    return types;
        //}
        #endregion
    }
}