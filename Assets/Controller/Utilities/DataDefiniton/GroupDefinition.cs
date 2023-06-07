using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios
{
	public class GroupDefinition<T, Member> : DataDefinition<T> where T : GroupDefinition<T, Member> where Member : GroupMemberDefinition<Member, T>
	{
        public Member[] Members
		{
			get => m_Members;
			protected set => m_Members = value;
		}

		public bool HasMembers => m_Members != null && m_Members.Length > 0;

		public override string Name
		{
			get => m_GroupName;
			protected set => m_GroupName = value;
		}

        [SerializeField]
		protected string m_GroupName;

		[SerializeField, ReorderableList(HasLabels = false)]
		protected Member[] m_Members;
	}
}
