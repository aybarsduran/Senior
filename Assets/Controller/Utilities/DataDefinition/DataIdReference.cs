using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IdenticalStudios
{
    [Serializable]
	public struct DataIdReference<T> : IEquatable<DataIdReference<T>> 
#if UNITY_EDITOR
		, IDataReferenceHandler
#endif
		where T : DataDefinition<T>
	{
		public T Def => DataDefinition<T>.GetWithId(m_Value);
		public bool IsNull => m_Value == 0;

		public int Id => m_Value;
		public string Description => IsNull ? string.Empty : Def.Description;
		public string Name => IsNull ? string.Empty : Def.Name;
		public Sprite Icon => IsNull ? null : Def.Icon;

		[SerializeField]
		private int m_Value;


		public static DataIdReference<T> Empty => s_Empty;
		private static DataIdReference<T> s_Empty = new(0);

		#region Constructors
		public DataIdReference(T def)
		{
			this.m_Value = def != null ? def.Id : 0;
		}

		public DataIdReference(int id)
		{
			this.m_Value = id;
		}

		public DataIdReference(string name)
		{
			this.m_Value = 0;

			if (DataDefinition<T>.TryGetWithName(name, out var def))
				this.m_Value = def.Id;
		}
		#endregion

		#region Operators
		public static bool operator ==(DataIdReference<T> x, DataIdReference<T> y) => x.m_Value == y.m_Value;
		public static bool operator ==(DataIdReference<T> x, DataNameReference<T> y) => x.m_Value == y.Id;
		public static bool operator ==(DataIdReference<T> x, T y) => y != null && x.m_Value == y.Id;
		public static bool operator ==(DataIdReference<T> x, int y) => x.m_Value == y;
		public static bool operator ==(DataIdReference<T> x, string y) => x.Name == y;

		public static bool operator !=(DataIdReference<T> x, DataIdReference<T> y) => x.m_Value != y.m_Value;
		public static bool operator !=(DataIdReference<T> x, DataNameReference<T> y) => x.m_Value != y.Id;
		public static bool operator !=(DataIdReference<T> x, T y) => y != null && x.m_Value != y.Id;
		public static bool operator !=(DataIdReference<T> x, int y) => x.m_Value != y;
		public static bool operator !=(DataIdReference<T> x, string y) => x.Name != y;

		public static implicit operator DataIdReference<T>(int value) => new(value);
		public static implicit operator DataIdReference<T>(string value) => new(value);

		public static implicit operator int(DataIdReference<T> reference) => reference.Id;
		public static implicit operator string(DataIdReference<T> reference) => reference.Name;
		#endregion

		#region IEquatable Implementation
		public bool Equals(DataIdReference<T> other) => m_Value == other.m_Value;
		public override bool Equals(object obj)
		{
			if (obj is DataIdReference<T>)
				return m_Value == ((DataIdReference<T>)obj).m_Value;
			else if (obj is int)
				return m_Value == (int)obj;

			return false;
		}

		public override int GetHashCode() => m_Value.GetHashCode();
		public override string ToString()
		{
			if (m_Value != 0)
				return Name;

			return string.Empty;
		}
		#endregion

		#region Editor
#if UNITY_EDITOR
		public readonly DataDefinitionBase GetDataAtIndex(int index) => DataDefinition<T>.GetWithIndex(index);
		public readonly int GetIdAtIndex(int index) => DataDefinitionUtility.GetIdAtIndex<T>(index); 
		public readonly int GetIndexOfId(int id) => DataDefinitionUtility.GetIndexOfId<T>(id);
		public readonly GUIContent[] GetAllGUIContents(bool name, bool tooltip, bool icon, GUIContent including)
		{
			return DataDefinitionUtility.GetAllGUIContents<T>(name, tooltip, icon, including);
		}
		public readonly int GetDataCount() => DataDefinition<T>.Definitions.Length;
#endif
		#endregion
	}

	public static class DataDefinitionIdReferenceExtensions
	{
		public static bool ContainsDef<T>(this DataIdReference<T>[] thisDataRefArray, DataIdReference<T> dataRef) where T : DataDefinition<T>
		{
			if (thisDataRefArray == null)
				return false;

			for (int i = 0; i < thisDataRefArray.Length; i++)
			{
				if (thisDataRefArray[i] == dataRef)
					return true;
			}

			return false;
		}

		public static bool ContainsAnyDef<T>(this DataIdReference<T>[] thisDataRefArray, DataIdReference<T>[] dataRefs) where T : DataDefinition<T>
		{
			if (thisDataRefArray == null)
				return true;

			for (int i = 0; i < thisDataRefArray.Length; i++)
			{
				for (int j = 0; j < dataRefs.Length; j++)
				{
					if (thisDataRefArray[i] == dataRefs[j])
						return true;
				}
			}

			return false;
		}

		public static bool ContainsAnyDef<T>(this List<DataIdReference<T>> thisDataRefList, DataIdReference<T>[] dataRefs) where T : DataDefinition<T>
		{
			if (thisDataRefList == null)
				return false;

            for (int i = 0; i < dataRefs.Length; i++)
            {
				if (thisDataRefList.Contains(dataRefs[i]))
					return true;
			}

			return false;
		}
	}
}
