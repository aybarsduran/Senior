using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace IdenticalStudios
{
    public class GroupMemberDefinition<T, Group> : DataDefinition<T> where T : GroupMemberDefinition<T, Group> where Group : GroupDefinition<Group, T>
    {
        public Group ParentGroup => m_ParentGroup;
        public bool IsPartOfGroup => m_ParentGroup != null;
        public override string FullName
        {
            get
            {
                string categoryName = ParentGroup != null ? ParentGroup.Name : k_UnssignedGroup;
                return $"( {categoryName} ) / {Name}";
            }
        }

        [SerializeField, Disable]
        private Group m_ParentGroup;

        private const string k_UnssignedGroup = "No Group";


        #region Editor
#if UNITY_EDITOR
        /// <summary>
        /// <para> Warning: This is an editor method, don't call it at runtime.</para> 
        /// Sets the category of this item (Internal).
        /// </summary>
        public void SetGroup(GroupDefinition<Group, T> group)
        {
            m_ParentGroup = group as Group;
            EditorUtility.SetDirty(this);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_ParentGroup != null)
            {
                if (!m_ParentGroup.Members.Contains(this))
                    m_ParentGroup = null;
            }
        }
#endif
        #endregion
    }
}
