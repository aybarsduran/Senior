using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdenticalStudios
{
    public static class ReflectionExtensions
    {
		/// <summary>
		/// Returns the FieldInfo matching 'name' from either type 't' itself or its most-derived 
		/// base type (unlike 'System.Type.GetField'). Returns null if no match is found.
		/// </summary>
		public static FieldInfo GetPrivateField(this Type t, string name)
		{
			const BindingFlags bf = BindingFlags.Instance |
									BindingFlags.NonPublic |
									BindingFlags.DeclaredOnly;

			int iterrations = 0;

			FieldInfo fi;
			while ((fi = t.GetField(name, bf)) == null && (t = t.BaseType) != null && iterrations < 12)
				iterrations++;
			return fi;
		}

		public static T GetPrivateFieldValue<T>(this object obj, string fieldName)
		{
			Type type = obj.GetType();
			return (T)type.GetPrivateField(fieldName).GetValue(obj);
		}

		public static void SetFieldValue(this object source, string fieldName, object value)
		{
			Type sourceType = source.GetType();
			FieldInfo field = sourceType.GetPrivateField(fieldName);
			field.SetValue(source, value);
		}

        public static FieldInfo[] GetFieldInfosIncludingBaseClasses(this Type type, BindingFlags bindingFlags)
        {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

            // If this class doesn't have a base, don't waste any time
            if (type.BaseType == typeof(object))
            {
                return fieldInfos;
            }
            else
            { // Otherwise, collect all types up to the furthest base class
                var fieldInfoList = new List<FieldInfo>(fieldInfos);
                while (type.BaseType != typeof(object))
                {
                    type = type.BaseType;
                    fieldInfos = type.GetFields(bindingFlags);

                    // Look for fields we do not have listed yet and merge them into the main list
                    for (int index = 0; index < fieldInfos.Length; ++index)
                    {
                        bool found = false;

                        for (int searchIndex = 0; searchIndex < fieldInfoList.Count; ++searchIndex)
                        {
                            bool match =
                                (fieldInfoList[searchIndex].DeclaringType == fieldInfos[index].DeclaringType) &&
                                (fieldInfoList[searchIndex].Name == fieldInfos[index].Name);

                            if (match)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            fieldInfoList.Add(fieldInfos[index]);
                        }
                    }
                }

                return fieldInfoList.ToArray();
            }
        }
    }
}
