using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    [Serializable]
    public struct DataNameReference<T> : IEquatable<DataNameReference<T>>

		where T : DataDefinition<T>
	{
		public T Def => DataDefinition<T>.GetWithName(m_Value);
		public bool IsNull => m_Value == string.Empty;

		public string Name => m_Value;
		public int Id => IsNull ? 0 : Def.Id;
		public string Description => IsNull ? string.Empty : Def.Description;
		public Sprite Icon => IsNull ? null : Def.Icon;

		[SerializeField]
		private string m_Value;


		#region Constructors
		public DataNameReference(T def)
		{
			this.m_Value = def != null ? def.Name : string.Empty;
		}

		public DataNameReference(string name)
		{
			this.m_Value = name;
		}

		public DataNameReference(int id)
		{
			this.m_Value = string.Empty;

			if (DataDefinition<T>.TryGetWithId(id, out var def))
				this.m_Value = def.Name;
		}
		#endregion

		#region Operators
		public static bool operator ==(DataNameReference<T> x, DataNameReference<T> y) => x.m_Value == y.m_Value;
		public static bool operator ==(DataNameReference<T> x, DataIdReference<T> y) => x.m_Value == y.Name;
		public static bool operator ==(DataNameReference<T> x, T y) => y != null && x.m_Value == y.Name;
		public static bool operator ==(DataNameReference<T> x, string y) => x.m_Value == y;
		public static bool operator ==(DataNameReference<T> x, int y) => x.Id == y;

		public static bool operator !=(DataNameReference<T> x, DataNameReference<T> y) => x.m_Value != y.m_Value;
		public static bool operator !=(DataNameReference<T> x, DataIdReference<T> y) => x.m_Value != y.Name;
		public static bool operator !=(DataNameReference<T> x, T y) => y != null && x.m_Value != y.Name;
		public static bool operator !=(DataNameReference<T> x, string y) => x.m_Value != y;
		public static bool operator !=(DataNameReference<T> x, int y) => x.Id != y;

		public static implicit operator DataNameReference<T>(int value) => new(value);
		public static implicit operator DataNameReference<T>(string value) => new(value);

		public static implicit operator int(DataNameReference<T> reference) => reference.Id;
		public static implicit operator string(DataNameReference<T> reference) => reference.Name;
		#endregion

		#region IEquatable Implementation
		public bool Equals(DataNameReference<T> other) => m_Value == other.m_Value;
		public override bool Equals(object obj)
		{
			if (obj is DataNameReference<T> nameRef)
				return m_Value == nameRef.m_Value;
			else if (obj is string str)
				return m_Value == str;

			return false;
		}

		public override int GetHashCode() => m_Value.GetHashCode();
		public override string ToString() => m_Value;
		#endregion

	}

	public static class DataDefinitionNameReferenceExtensions
	{
		public static bool ContainsDef<T>(this DataNameReference<T>[] thisDataRefArray, DataNameReference<T> dataRef) where T : DataDefinition<T>
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

		public static bool ContainsAnyDef<T>(this DataNameReference<T>[] thisDataRefArray, DataNameReference<T>[] dataRefs) where T : DataDefinition<T>
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

		public static bool ContainsAnyDef<T>(this List<DataNameReference<T>> thisDataRefList, DataNameReference<T>[] dataRefs) where T : DataDefinition<T>
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
