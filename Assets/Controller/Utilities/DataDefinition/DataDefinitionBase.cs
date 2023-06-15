using UnityEngine;

#if UNITY_EDITOR
#endif

namespace IdenticalStudios
{
    public abstract class DataDefinitionBase : ScriptableObject
    {
        public int Id
        {
            get => m_Id;
            protected set => m_Id = value;
        }

        public virtual string Name
        {
            get => string.Empty;
            protected set { }
        }

        public virtual string FullName => Name;
        public virtual string Description => string.Empty;
        public virtual Sprite Icon => null;

        [SerializeField, Disable]
        [Tooltip("Unique id (auto generated).")]
        protected int m_Id = -1;


        #region Editor
#if UNITY_EDITOR
        private bool m_IsDirty;
        public bool IsDirty() => m_IsDirty;
        public bool ClearDirty() => m_IsDirty = false;
        protected virtual void OnValidate() => m_IsDirty = true;

        public virtual void Reset() => Name = DataDefinitionUtility.GetDefaultDefinitionName(this);
#endif
        #endregion
    }
}
