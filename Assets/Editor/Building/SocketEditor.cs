using System.Linq;
using Toolbox.Editor;
using UnityEditor;

using UnityEditorInternal;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Socket))]
    public class SocketEditor : ToolboxEditor
    {
        public enum Axis { X, Z }

		private Socket m_Socket;
        private StructureBuildable m_Buildable;

		private SerializedProperty m_Radius;
        private SerializedProperty m_Offsets;

		private ReorderableList m_OffsetsList;
		private Socket.BuildableOffset m_SelectedOffset;
        private GameObject m_CurrentPreview;

        private static bool s_PreviewEnabled;
        private static int s_SelectedOffsetIndex;
        private static int s_SelectedBuildableIndex = -1;

        private static Axis s_MirrorAxis;
        private static bool s_InvertRotationToggle;
        private static bool s_AlignRotationToggle;

        private static Color s_GreyColor = new(0.9f, 0.9f, 0.9f, 0.9f);
        private static Color s_WhiteColor = Color.white;


        #region Initialization
        private void OnEnable()
        {
            m_Socket = target as Socket;

            if (s_PreviewEnabled)
                Tools.current = Tool.None;

            // Get the parent buildable..
            m_Buildable = m_Socket.GetComponentInParent<StructureBuildable>();

            m_Offsets = serializedObject.FindProperty("m_Offsets");
            m_Radius = serializedObject.FindProperty("m_Radius");

            // Enable draw gizmos..
            EditorWindow.GetWindow<SceneView>(null, false).drawGizmos = true;
            UnityEditor.SceneManagement.PrefabStage.prefabSaving += OnPrefabSave;

            // Initialize the piece list..
            m_OffsetsList = new ReorderableList(serializedObject, m_Offsets)
            {
                drawHeaderCallback = (Rect rect) => GUI.Label(rect, "Categories"),
                drawElementCallback = DrawPieceElement,
            };

            m_OffsetsList.onSelectCallback += OnPieceSelect;

            if (s_SelectedOffsetIndex < m_OffsetsList.count)
                m_OffsetsList.index = s_SelectedOffsetIndex;
            else
                m_OffsetsList.index = 0;

            m_OffsetsList.onSelectCallback.Invoke(m_OffsetsList);
        }

        private void OnDestroy()
        {
            Tools.hidden = false;

            if (m_CurrentPreview != null)
                DestroyImmediate(m_CurrentPreview);
        }

        private void OnPrefabSave(GameObject prefab)
        {
            UnityEditor.SceneManagement.PrefabStage.prefabSaving -= OnPrefabSave;
            OnDestroy();
        }
        #endregion

        #region Inspector

        protected override void DrawCustomInspector()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_Radius);
            CustomGUILayout.Separator();
            EditorGUILayout.Space();

            GUI.color = s_PreviewEnabled ? Color.grey : Color.white;

            if (s_PreviewEnabled && m_SelectedOffset == null)
            {
                m_OffsetsList.index = Mathf.Clamp(s_SelectedOffsetIndex, 0, m_OffsetsList.count - 1);
                m_OffsetsList.onSelectCallback.Invoke(m_OffsetsList);
            }

            if (GUILayout.Button("Enable Preview"))
            {
                if (!s_PreviewEnabled)
                    EnablePreview();
                else
                    DisablePreview();
            }

            GUI.color = Color.white;

            if (Selection.objects.Length == 1)
                m_OffsetsList.DoLayoutList();

            DrawSocketTools();

            if (serializedObject.ApplyModifiedProperties())
                m_OffsetsList.onSelectCallback.Invoke(m_OffsetsList);
        }

        private void EnablePreview()
        {
            if (s_PreviewEnabled)
                return;

            Tools.current = Tool.None;

            s_PreviewEnabled = true;
            m_OffsetsList.onSelectCallback.Invoke(m_OffsetsList);
        }

        private void DisablePreview()
        {
            if (!s_PreviewEnabled)
                return;

            Tools.current = Tool.Move;

            if (m_CurrentPreview != null)
                DestroyImmediate(m_CurrentPreview);

            s_PreviewEnabled = false;
        }

        private void DrawPieceElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.BeginChangeCheck();

            bool canSelect = true;
            var categoryRef = m_Offsets.GetArrayElementAtIndex(index).FindPropertyRelative("m_Category");
            var category = categoryRef.GetValue<DataIdReference<BuildableCategoryDefinition>>().Def;

            if (category == null || !category.HasMembers)
            {
                GUI.contentColor = s_GreyColor;
                canSelect = false;
            }

            float xOffset = rect.width / 10f;
            Rect newRect = new Rect(rect.x + xOffset, rect.y, rect.width - xOffset, 16f);
            EditorGUI.PropertyField(newRect, categoryRef);

            if (canSelect && EditorGUI.EndChangeCheck())
                TrySelectPiece(index);

            GUI.contentColor = s_WhiteColor;
        }

        private void OnPieceSelect(ReorderableList list) => TrySelectPiece(list.index);

        private void TrySelectPiece(int index)
        {
            if (!s_PreviewEnabled)
                return;

            if (m_OffsetsList.count > 0)
            {
                if (m_CurrentPreview != null)
                    DestroyImmediate(m_CurrentPreview);

                index = Mathf.Clamp(index, 0, m_OffsetsList.count - 1);
                m_SelectedOffset = m_Socket.Offsets[index];

                m_OffsetsList.index = index;
                s_SelectedOffsetIndex = index;

                Buildable nextBuildable = null;

                if (m_SelectedOffset != null && !m_SelectedOffset.Category.IsNull)
                {
                    var buildableDef = m_SelectedOffset.Category.Def.Members.Select(ref s_SelectedBuildableIndex, SelectionType.Sequence);
                    if (BuildingManager.CustomBuildingDefinitions.Contains(buildableDef))
                        nextBuildable = buildableDef.Prefab;
                }

                CreatePreview(nextBuildable);
            }
        }

        private void CreatePreview(Buildable targetBuildable)
        {
            if (targetBuildable != null)
            {
                GameObject preview = Instantiate(targetBuildable.gameObject, m_Buildable.transform);
                preview.hideFlags = HideFlags.HideAndDontSave;
                preview.name = "(Preview)";
                var components = preview.GetComponentsInChildren<Component>();

                foreach (var rootComponent in components)
                {
                    if (rootComponent == null)
                        continue;

                    if (rootComponent.GetType() == typeof(Socket))
                        DestroyImmediate(rootComponent.gameObject);
                    else if (rootComponent.GetType() != typeof(Transform) && rootComponent.GetType() != typeof(MeshFilter) && rootComponent.GetType() != typeof(MeshRenderer))
                        DestroyImmediate(rootComponent);
                }

                m_CurrentPreview = preview;
            }
            else
                m_CurrentPreview = null;
        }
        #endregion

        #region Socket Tools
        private void DrawSocketTools()
        {
            CustomGUILayout.Separator();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Mirror Socket"))
            {
                Vector3 mirrorAxisVector = Vector3.right;

                if (s_MirrorAxis == Axis.Z)
                    mirrorAxisVector = Vector3.forward;

                MirrorSocket(mirrorAxisVector);
            }

            GUI.color = s_GreyColor;

            Rect rect = EditorGUILayout.GetControlRect();

            Rect popupRect = new Rect(rect.x + rect.width * 0.75f, rect.y, rect.width * 0.25f, rect.height);
            Rect labelRect = new Rect(rect.xMax - popupRect.width - 72, rect.y, 72, rect.height);

            s_MirrorAxis = (Axis)EditorGUI.EnumPopup(popupRect, s_MirrorAxis);

            EditorGUI.LabelField(labelRect, "Mirror Axis: ");

            GUI.color = s_WhiteColor;

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Perpendicular Socket (90 Degrees)"))
                CreatePerpendicularSocket();

            GUI.color = s_GreyColor;

            rect = EditorGUILayout.GetControlRect();

            popupRect = new Rect(rect.x + rect.width * 0.75f, rect.y, rect.width * 0.25f, rect.height);
            labelRect = new Rect(rect.xMax - popupRect.width - 44, rect.y, 44, rect.height);

            s_InvertRotationToggle = EditorGUI.Toggle(popupRect, s_InvertRotationToggle);

            EditorGUI.LabelField(labelRect, "Invert: ");

            rect = EditorGUILayout.GetControlRect();

            popupRect = new Rect(rect.x + rect.width * 0.75f, rect.y, rect.width * 0.25f, rect.height);
            labelRect = new Rect(rect.xMax - popupRect.width - 44, rect.y, 44, rect.height);

            s_AlignRotationToggle = EditorGUI.Toggle(popupRect, s_AlignRotationToggle);

            EditorGUI.LabelField(labelRect, "Align: ");

            GUI.color = s_WhiteColor;
        }

        private void MirrorSocket(Vector3 mirrorAxis)
        {
            Vector3 mirrorScaler = new Vector3(-Mathf.Clamp01(Mathf.Abs(mirrorAxis.x)), -Mathf.Clamp01(Mathf.Abs(mirrorAxis.y)), -Mathf.Clamp01(Mathf.Abs(mirrorAxis.z)));

            if (mirrorScaler.x == 0f)
                mirrorScaler.x = 1f;

            if (mirrorScaler.y == 0f)
                mirrorScaler.y = 1f;

            if (mirrorScaler.z == 0f)
                mirrorScaler.z = 1f;

            Vector3 originalPosition = m_Socket.transform.localPosition;
            Vector3 mirrorPosition = Vector3.Scale(originalPosition, mirrorScaler);

            GameObject mirrorSocket = Instantiate(m_Socket.gameObject, m_Socket.transform.parent);
            mirrorSocket.transform.localPosition = mirrorPosition;

            mirrorSocket.name = "Socket";

            var offsets = mirrorSocket.GetComponent<Socket>().Offsets;

            foreach (var offset in offsets)
                offset.SetPositionOffset(Vector3.Scale(offset.PositionOffset, mirrorScaler));

            Undo.RegisterCreatedObjectUndo(mirrorSocket, "Create Mirror Socket");

            Selection.activeGameObject = mirrorSocket;
        }

        private void CreatePerpendicularSocket()
        {
            Quaternion rotator = Quaternion.Euler(0f, 90f * (s_InvertRotationToggle ? -1 : 1), 0f);

            Vector3 rotatedSocketPosition = RoundVector3(rotator * m_Socket.transform.localPosition);

            GameObject mirrorSocket = Instantiate(m_Socket.gameObject, m_Socket.transform.parent);
            Undo.RegisterCreatedObjectUndo(mirrorSocket, "Crate Perpendicular Socket");

            mirrorSocket.transform.localPosition = rotatedSocketPosition;

            mirrorSocket.name = "Socket";

            var offsets = mirrorSocket.GetComponent<Socket>().Offsets;

            foreach (var offset in offsets)
            {
                offset.SetPositionOffset(RoundVector3(rotator * offset.PositionOffset));

                if (s_AlignRotationToggle)
                    offset.SetRotationOffset(RoundVector3((rotator * offset.RotationOffset).eulerAngles));
            }

            Selection.activeGameObject = mirrorSocket;
        }
        #endregion

        #region Scene View
        private void OnSceneGUI()
		{
            if (CannotDisplayMesh() || m_Buildable == null)
            {
                SceneView.RepaintAll();
                return;
            }

			if (HasValidPiece())
			{
                RefreshPreviewPosition();

                Color prevHandlesColor = Handles.color;
                Handles.color = Color.white;

                var labelStyle = new GUIStyle(EditorStyles.whiteBoldLabel) { alignment = TextAnchor.MiddleCenter };
                Handles.Label(m_CurrentPreview.transform.position, m_CurrentPreview.name, labelStyle);

                Handles.color = prevHandlesColor;

                // Draw the piece tools (move & rotate).
                DoPieceOffsetTools();
            }

            SceneView.RepaintAll();

            // Draw the inspector for the piece offset for the selected socket, so you can modify the position and rotation precisely.
            DoPieceOffsetInspectorWindow();
		}

        private void RefreshPreviewPosition()
        {
            if (m_Socket.Offsets.Length <= s_SelectedOffsetIndex)
                return;

            Vector3 position = m_Socket.transform.position + m_Buildable.transform.TransformVector(m_Socket.Offsets[s_SelectedOffsetIndex].PositionOffset);
            Quaternion rotation = m_Buildable.transform.rotation * m_Socket.Offsets[s_SelectedOffsetIndex].RotationOffset;

            m_CurrentPreview.transform.SetPositionAndRotation(position, rotation);
        }

		private void DoPieceOffsetTools()
		{
			Vector3 pieceWorldPos = m_Socket.transform.position + m_Socket.transform.TransformVector(m_SelectedOffset.PositionOffset);

			EditorGUI.BeginChangeCheck();
			var handlePos = Handles.PositionHandle(pieceWorldPos, m_Socket.transform.rotation * m_SelectedOffset.RotationOffset);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Socket");

				handlePos = RoundVector3(m_Socket.transform.InverseTransformPoint(handlePos));
                m_SelectedOffset.SetPositionOffset(handlePos);
			}
		}

		private void DoPieceOffsetInspectorWindow()
		{
			Color color = Color.white;
			GUI.backgroundColor = color;

			var windowRect = new Rect(16f, 32f, 256f, 112f);
			Rect totalRect = new Rect(windowRect.x, windowRect.y - 16f, windowRect.width, windowRect.height);

			GUI.backgroundColor = Color.white;
			GUI.Window(1, windowRect, DrawPieceOffsetInspector, "Position & Rotation");

			Event e = Event.current;

			if (totalRect.Contains(e.mousePosition))
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

				if (e.type != EventType.Layout && e.type != EventType.Repaint)
					e.Use();
			}
		}

		private void DrawPieceOffsetInspector(int windowID)
		{
			if (!HasValidPiece())
			{
				EditorGUI.HelpBox(new Rect(0f, 32f, 512f, 32f), "No valid piece selected!", MessageType.Warning);
				return;
			}
				
			var pieceOffset = m_SelectedOffset;

			EditorGUI.BeginChangeCheck();

			// Position field.
			var positionOffset = EditorGUI.Vector3Field(new Rect(6f, 32f, 240f, 16f), "Position", pieceOffset.PositionOffset);

			// Rotation field.
			var rotationOffset = EditorGUI.Vector3Field(new Rect(6f, 64f, 240f, 16f), "Rotation", pieceOffset.RotationOffsetEuler);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Socket");

				positionOffset = RoundVector3(positionOffset);
				rotationOffset = RoundVector3(rotationOffset);

                pieceOffset.SetPositionOffset(positionOffset);
                pieceOffset.SetRotationOffset(rotationOffset);
			}
		}

        private bool HasValidPiece() => m_CurrentPreview != null && m_OffsetsList.count != 0 && s_SelectedOffsetIndex >= 0 && m_SelectedOffset != null && m_SelectedOffset.Category != 0;
        private bool CannotDisplayMesh() => (!s_PreviewEnabled || Selection.activeGameObject == null || Selection.activeGameObject != m_Socket.gameObject);

        private Vector3 RoundVector3(Vector3 source, int digits = 3)
		{
			source.x = (float)System.Math.Round(source.x, digits);
			if (Mathf.Approximately(source.x, 0f))
				source.x = 0f;

			source.y = (float)System.Math.Round(source.y, digits);
			if (Mathf.Approximately(source.y, 0f))
				source.y = 0f;

			source.z = (float)System.Math.Round(source.z, digits);
			if (Mathf.Approximately(source.y, 0f))
				source.y = 0f;

			return source;
		}
        #endregion
    }
}
