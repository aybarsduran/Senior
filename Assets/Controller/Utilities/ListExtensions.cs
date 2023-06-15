using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;

namespace IdenticalStudios
{
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    public static class ListExtensions
    {
        public static bool TryGetElementOfType<T>(this IList list, out T element, bool allowInheritance = false) where T : class
        {
            if (allowInheritance)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var obj = list[i];
                    if (obj as T != null)
                    {
                        element = (T)obj;
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var obj = list[i];
                    if (obj.GetType() != typeof(T))
                        continue;

                    element = (T)obj;
                    return true;
                }
            }

            element = null;
            return false;
        }

        public static T SelectRandom<T>(this T[] array)
        {
            int last = 0;

            return Select(array, ref last, SelectionType.Random);
        }

        public static bool IsIndexValid(this IList list, int index)
        {
            if (list == null)
                return false;

            return index >= 0 && list.Count > index;
        }

        public static int IndexOf<T>(this IList<T> list, T obj) where T : class
        {
            if (list == null)
                return -1;

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (list[i] == obj)
                    return i;
            }

            return -1;
        }

        public static T Select<T>(this T[] array, ref int last, SelectionType selectionMethod = SelectionType.Random)
        {
            if (array == null || array.Length == 0)
                return default;

            int next = 0;

            switch (selectionMethod)
            {
                case SelectionType.Random:
                    next = Random.Range(0, array.Length);
                    break;
                case SelectionType.RandomExcludeLast when array.Length > 1:
                {
                    last = Mathf.Clamp(last, 0, array.Length - 1);

                    (array[0], array[last]) = (array[last], array[0]);

                    next = Random.Range(1, array.Length);
                    break;
                }
                case SelectionType.Sequence:
                    next = (int)Mathf.Repeat(last + 1, array.Length);
                    break;
            }

            last = next;

            return array[next];
        }

        public static T SelectRandom<T>(this List<T> list)
        {
            int last = 0;

            return Select(list, ref last, SelectionType.Random);
        }

        public static T Select<T>(this List<T> list, ref int last, SelectionType selectionMethod = SelectionType.Random)
        {
            if(list == null || list.Count == 0)
                return default;

            int next = 0;

            switch (selectionMethod)
            {
                case SelectionType.Random: next = Random.Range(0, list.Count); break;
                case SelectionType.RandomExcludeLast:
                {
                    if (list.Count > 1)
                    {
                        last = Mathf.Clamp(last, 0, list.Count - 1);

                        (list[0], list[last]) = (list[last], list[0]);

                        next = Random.Range(1, list.Count);
                    }
                }
                    break;
                case SelectionType.Sequence: next = (int)Mathf.Repeat(last + 1, list.Count); break;
            }

            last = next;

            return list[next];
        }

        /// <summary>
        /// Checks if the index is inside the list's bounds.
        /// </summary>
        public static bool IndexIsValid<T>(this List<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static bool IsPartOfCollection<T>(this IEnumerable<T> collection, T value) where T : IEquatable<T>
        {
            foreach (var element in collection)
            {
                if (value.Equals(element))
                    return true;
            }

            return false;
        }

        public static void DistinctPreserveNull<T>(ref List<T> list) where T : Object
        {
            if (list == null)
                return;

            int index = 0;
            while (index < list.Count)
            {
                int itemCount = 0;
                int indexOfDuplicate = -1;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[index] == list[i])
                    {
                        itemCount++;
                        indexOfDuplicate = i;
                    }
                }

                if (list[indexOfDuplicate] != null && itemCount == 2)
                {
                    list[indexOfDuplicate] = null;
                    index++;
                    continue;
                }

                if (itemCount > 1)
                {
                    list.RemoveAt(indexOfDuplicate);
                    continue;
                }

                index++;
            }
        }

        public static void DistinctRemoveNull<T>(ref T[] array) where T : Object
        {
            if (array == null)
                return;

            array = array.Distinct().ToArray();
        }

        public static void RemoveAllNull<T>(ref T[] array) where T : Object
        {
            if (array == null)
                return;

            int index = 0;
            while (index < array.Length)
            {
                if (array[index] == null)
                    RemoveAtIndex(ref array, index);
                else
                    index++;
            }
        }

        public static void RemoveAllNulls<T>(ref T[] array) where T : INullable
        {
            if (array == null)
                return;

            int index = 0;
            while (index < array.Length)
            {
                if (array[index].IsNull)
                    RemoveNullAtIndex(ref array, index);
                else
                    index++;
            }
        }

        public static void DistinctPreserveNull<T>(ref T[] array) where T : Object
        {
            if (array == null)
                return;

            int index = 0;
            while (index < array.Length)
            {
                int itemCount = 0;
                int indexOfDuplicate = -1;
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[index] == array[i])
                    {
                        itemCount++;
                        indexOfDuplicate = i;
                    }
                }

                if (array[indexOfDuplicate] != null && itemCount == 2)
                {
                    array[indexOfDuplicate] = null;
                    index++;
                    continue;
                }

                if (itemCount > 1)
                {
                    RemoveAtIndex(ref array, indexOfDuplicate);
                    continue;
                }
                
                index++;
            }
        }

        private static void RemoveAtIndex<T>(ref T[] array, int index) where T : Object
        {
            var newArray = new T[array.Length - 1];

            for (int i = 0; i < index; i++)
                newArray[i] = array[i];

            for (int j = index; j < newArray.Length; j++)
                newArray[j] = array[j + 1];

            array = newArray;
        }

        private static void RemoveNullAtIndex<T>(ref T[] array, int index) where T : INullable
        {
            var newArray = new T[array.Length - 1];

            for (int i = 0; i < index; i++)
                newArray[i] = array[i];

            for (int j = index; j < newArray.Length; j++)
                newArray[j] = array[j + 1];

            array = newArray;
        }
    }

    public enum SelectionType
    {
        /// <summary>The item will be selected randomly.</summary>
        Random,

        /// <summary>The item will be selected randomly, but will exclude the last selected.</summary>
        RandomExcludeLast,

        /// <summary>The items will be selected in sequence.</summary>
        Sequence
    }
}
