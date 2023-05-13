using UnityEngine;

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

        [Tooltip("Unique id (auto generated).")]
        protected int m_Id = -1;
                
    }
}
