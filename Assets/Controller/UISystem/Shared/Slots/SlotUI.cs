using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios.UISystem
{
    [DisallowMultipleComponent]
    public abstract class SlotUI<T> : MonoBehaviour where T : class
    {
        public SelectableUI Selectable
        {
            get
            {
                if (m_Selectable == null)
                    m_Selectable = GetComponent<SelectableUI>();

                return m_Selectable;
            }
        }

        protected T Data { get; private set; }

        [SerializeField]
        [Help("Specific Info Components")]
        [FormerlySerializedAs("m_InfoComponents")]
        [Disable, ReorderableList(HasLabels = false, HasHeader = false)]
        private DataInfoUI<T>[] m_SpecificInfo;

        [SerializeField]
        [Help("Generic Info Components")]
        [Disable, ReorderableList(HasLabels = false, HasHeader = false)]
#if UNITY_EDITOR
        [EditorButton(nameof(RefreshInfoComponents), "Refresh")]
#endif
        private GenericDataInfoUI[] m_GenericInfo;

        private SelectableUI m_Selectable;


        public bool TryGetInfoOfType<U>(out U info) where U : DataInfoBaseUI
        {
            if (m_SpecificInfo.TryGetElementOfType(out info, true))
                return true;

            return m_GenericInfo.TryGetElementOfType(out info, true);
        }

        protected void SetData(T data)
        {
//#if UNITY_EDITOR
//            if (UnityUtils.IsQuittingPlayMode)
//                return;
//#endif

            Data = data;

            for (int i = 0; i < m_SpecificInfo.Length; i++)
                m_SpecificInfo[i].SetData(data);

            if (data != null)
            {
                for (int i = 0; i < m_GenericInfo.Length; i++)
                    m_GenericInfo[i].OnDataChanged();
            }
            else
            {
                for (int i = 0; i < m_GenericInfo.Length; i++)
                    m_GenericInfo[i].OnDataDisabled();
            }
        }

        protected virtual void Awake() => SetData(null);

#if UNITY_EDITOR
        protected void RefreshInfoComponents()
        {
            m_SpecificInfo = GetComponents<DataInfoUI<T>>();
            m_GenericInfo = GetComponents<GenericDataInfoUI>();
        }

        protected virtual void Reset() => gameObject.GetOrAddComponent<SelectableUI>();
        protected virtual void OnValidate() => RefreshInfoComponents();
#endif
    }
}