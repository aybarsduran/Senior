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

        [SerializeField, NewLabel("Name")]
		protected string m_GroupName;

		[SpaceArea, SerializeField, ReorderableList(HasLabels = false)]
		protected Member[] m_Members;


		#region Editor
#if UNITY_EDITOR
		/// <summary>
		/// Warning: This is an editor property, don't interact with it at runtime.
		/// </summary>
		public void MergeWith(T group)
		{
			if (group == null)
				return;

			ArrayUtility.AddRange(ref m_Members, group.Members);
			EditorUtility.SetDirty(this);

			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(group));

			ReloadDefinitions();
			RefreshAllGroups();
		}

		/// <summary>
		/// Warning: This is an editor method, don't call it at runtime.
		/// </summary>
		public virtual void AddDefaultDataToDefinition(Member def) { }

		/// <summary>
		/// Warning: This is an editor method, don't call it at runtime.
		/// </summary>
		public void RemoveAllNullMembers()
		{
			if (this == null)
			{
				ReloadDefinitions();
				return;
			}

			EditorUtility.SetDirty(this);
			ListExtensions.RemoveAllNull(ref m_Members);
		}

		/// <summary>
		/// Warning: This is an editor method, don't call it at runtime.
		/// </summary>
		public void AddMember(Member def)
		{
			if (def == null)
				return;

			if (!ArrayUtility.Contains(m_Members, def))
			{
				def.SetGroup(this);
				ArrayUtility.Add(ref m_Members, def);

				EditorUtility.SetDirty(this);
				EditorUtility.SetDirty(def);
			}
		}

		/// <summary>
		/// Warning: This is an editor method, don't call it at runtime.
		/// </summary>
		public void RemoveMember(Member def)
		{
			if (def == null)
				return;

			if (ArrayUtility.Contains(m_Members, def))
			{
				ArrayUtility.Remove(ref m_Members, def);
				def.SetGroup(null);

				EditorUtility.SetDirty(this);
				EditorUtility.SetDirty(def);
			}
		}

		/// <summary>
		/// Warning: This is an editor method, don't call it at runtime.
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			m_Members = new Member[] { };

			RefreshAllGroups();
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			ListExtensions.DistinctPreserveNull(ref m_Members);
			RefreshAllGroups();
		}

		private void RefreshAllGroups()
		{
			if (m_Members == null || m_Members.Length == 0)
				return;

			int index = 0;
			while (index < m_Members.Length)
			{
				var item = m_Members[index];
				if (item != null)
				{
					if (item.ParentGroup != null && item.ParentGroup != this)
					{
						ArrayUtility.RemoveAt(ref m_Members, index);
						continue;
					}
					else
						index++;

					item.SetGroup(this);
					EditorUtility.SetDirty(item);
				}
				else
					index++;
			}
		}
#endif
		#endregion
	}
}
