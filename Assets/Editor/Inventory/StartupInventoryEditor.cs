using Toolbox.Editor;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    [CustomEditor(typeof(InventoryStartupItemsInfo))]
    public class StartupInventoryEditor : ToolboxEditor
    {
        private Inventory m_Inventory;

        private string[] m_ContainerNames;
        private int m_SelectedContainer;

        private SerializedProperty m_Containers;

        private static Character m_Character;
        private static Character m_PrevCharacter;


        protected override void DrawCustomInspector()
        {
            if (m_Character == null)
                EditorGUILayout.HelpBox("Assign the Player prefab.", MessageType.Warning);

            m_Character = EditorGUILayout.ObjectField(m_Character, typeof(Character), true) as Character;

            if (m_Character == null)
                return;

            CustomGUILayout.Separator();

            if (m_Character != null && (m_Inventory == null || (m_Character != m_PrevCharacter)))
            {
                m_Inventory = m_Character.GetComponentInChildren<Inventory>();
                 
                if (m_Inventory != null) 
                {
                    m_Containers = serializedObject.FindProperty("m_ItemContainersStartupItems");
                    CheckContainers();
                }

                m_PrevCharacter = m_Character;
            }

            if (m_Inventory == null)
            {
                EditorGUILayout.HelpBox("No Inventory component found!", MessageType.Error); 

                return;
            }
            else if (m_ContainerNames == null || m_ContainerNames.Length == 0)
            {
                EditorGUILayout.HelpBox("No Inventory Containers found!", MessageType.Error);
                return;
            }

            EditorGUILayout.Space();

            m_SelectedContainer = EditorGUILayout.Popup("Container", m_SelectedContainer, m_ContainerNames);

            CustomGUILayout.Separator();
            EditorGUILayout.Space();

            serializedObject.Update();

            if (m_Inventory.StartupContainers.Length != m_ContainerNames.Length)
                CheckContainers();

            var container = m_Containers.GetArrayElementAtIndex(m_SelectedContainer);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(16f);

                using (new GUILayout.VerticalScope())
                {
                    DoContainerGUI(container);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            m_Character = FindObjectOfType<Player>();
        }

        private void DoContainerGUI(SerializedProperty container)
        {
            GUILayout.Label(string.Format("Startup Items ({0})", container.FindPropertyRelative("Name").stringValue + " Container"), EditorStyles.boldLabel);

            var startupItems = container.FindPropertyRelative("StartupItems");

            int i = 0;
            int itemToRemove = -1;

            foreach (SerializedProperty item in startupItems)
            {
                SerializedProperty method = item.FindPropertyRelative("m_Method");
                SerializedProperty minCount = item.FindPropertyRelative("m_MinCount");
                SerializedProperty maxCount = item.FindPropertyRelative("m_MaxCount");

                using (new GUILayout.VerticalScope("Box"))
                {
                    EditorGUILayout.PropertyField(method);

                    if ((ItemGenerationMethod)method.enumValueIndex == ItemGenerationMethod.Specific)
                    {
                        SerializedProperty specificItem = item.FindPropertyRelative("m_SpecificItem");

                        EditorGUILayout.PropertyField(specificItem, new GUIContent("Item"));
                    }
                    else if ((ItemGenerationMethod)method.enumValueIndex == ItemGenerationMethod.RandomFromCategory)
                    {
                        SerializedProperty category = item.FindPropertyRelative("m_Category");

                        EditorGUILayout.PropertyField(category, new GUIContent("Item"));
                    }

                    EditorGUILayout.PropertyField(minCount, new GUIContent("Min Count"));
                    EditorGUILayout.PropertyField(maxCount, new GUIContent("Max Count"));

                    EditorGUILayout.Space();

                    if (CustomGUILayout.ColoredButton("Remove", CustomGUIStyles.LightRedColor, EditorStyles.miniButton))
                    {
                        itemToRemove = i;
                        break;
                    }

                    EditorGUILayout.Space();
                }

                i++;
            }

            EditorGUILayout.Space();

            using (new GUILayout.HorizontalScope())
            {
                if (itemToRemove != -1)
                    startupItems.DeleteArrayElementAtIndex(itemToRemove);

                if (CustomGUILayout.ColoredButton("Add New", CustomGUIStyles.GreenColor))
                {
                    int addIndex = startupItems.arraySize == 0 ? 0 : startupItems.arraySize - 1;
                    startupItems.InsertArrayElementAtIndex(addIndex);
                }

                if (CustomGUILayout.ColoredButton("Remove All", CustomGUIStyles.RedColor))
                {
                    if (EditorUtility.DisplayDialog("Remove All Items", "Are you sure you want to remove all items?", "Yes", "Cancel"))
                        startupItems.ClearArray();
                }
            }
        }

        private int FindContainerIndex(string contName)
        {
            int i = 0;

            foreach(SerializedProperty cont in m_Containers)
            {
                if(cont.FindPropertyRelative("Name").stringValue == contName)
                    return i;

                i++;
            }

            return -1;
        }

        private void AddNewContainer(string contName)
        {
            int addIndex = m_Containers.arraySize == 0 ? 0 : m_Containers.arraySize - 1;

            m_Containers.InsertArrayElementAtIndex(addIndex);

            m_Containers.GetArrayElementAtIndex(addIndex).FindPropertyRelative("Name").stringValue = contName;
        }

        private void CheckContainers()
        {
            serializedObject.Update();

            PullContainerNames();
            CheckContainersExistence();
            CheckContainersOrder();
            CheckContainersName();

            serializedObject.ApplyModifiedProperties();
        }

        private void PullContainerNames()
        {
            m_ContainerNames = new string[m_Inventory.StartupContainers.Length];

            for(int i = 0;i < m_Inventory.StartupContainers.Length;i++)
                m_ContainerNames[i] = m_Inventory.StartupContainers[i].Name;
        }

        private void CheckContainersExistence()
        {
            if(m_ContainerNames.Length != m_Containers.arraySize)
            {
                foreach(var containerName in m_ContainerNames)
                {
                    int idxOfContainer = FindContainerIndex(containerName);

                    if(idxOfContainer == -1)
                        AddNewContainer(containerName);
                }
            }
        }

        private void CheckContainersOrder()
        {
            for(int i = 0;i < m_ContainerNames.Length;i++)
            {
                int containerIdx = FindContainerIndex(m_ContainerNames[i]);

                // If the order is not right
                if(containerIdx != -1 && containerIdx != i)
                    m_Containers.MoveArrayElement(containerIdx, i);
            }
        }

        private void CheckContainersName()
        {
            int i = 0;

            foreach (SerializedProperty container in m_Containers)
            {
                container.FindPropertyRelative("Name").stringValue = m_ContainerNames[Mathf.Clamp(i, 0, m_ContainerNames.Length - 1)];
                i++;
            }
        }
    }
}