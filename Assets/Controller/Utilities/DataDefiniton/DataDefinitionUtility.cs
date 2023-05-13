using System.Collections.Generic;
using UnityEngine;
using System;
using IdenticalStudios;

namespace IdenticalStudios
{
    public static class DataDefinitionUtility
    {
        /// <returns> The icons of all of the definitions.</returns>
        public static Sprite[] GetAllIcons<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            Sprite[] icons = new Sprite[definitions.Length];

            for (int i = 0; i < icons.Length; i++)
                icons[i] = definitions[i].Icon;

            return icons;
        }

        /// <returns> The names of all of the definitions.</returns>
        public static string[] GetAllNames<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            string[] names = new string[definitions.Length];

            for (int i = 0; i < names.Length; i++)
                names[i] = definitions[i].FullName;

            return names;
        }

        /// <returns> The descriptions of all of the definitions.</returns>
        public static string[] GetAllDescriptions<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            string[] descriptions = new string[definitions.Length];

            for (int i = 0; i < descriptions.Length; i++)
                descriptions[i] = definitions[i].Description;

            return descriptions;
        }

        /// <returns> All the ids of all of the definitions.</returns>
        public static List<int> GetAllIds<T>() where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            var ids = new List<int>();

            for (int i = 0; i < definitions.Length; i++)
                ids.Add(definitions[i].Id);

            return ids;
        }

        /// <returns> Index of given definition in the internal array.</returns>
        public static int GetIndexOfId<T>(int id) where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;

            for (int i = 0; i < definitions.Length; i++)
            {
                if (id == definitions[i].Id)
                    return i;
            }

            return -1;
        }

        /// <returns> The definition id at the given index.</returns>
        public static int GetIdAtIndex<T>(int index) where T : DataDefinition<T>
        {
            var definitions = DataDefinition<T>.Definitions;
            index = Mathf.Max(0, index);

            if (definitions.Length > 0)
                return definitions[index].Id;

            return -1;
        }

        /// <returns> Index of given definition in the given array.</returns>
        public static int GetIndexOfDefinition(this DataDefinitionBase[] defs, DataDefinitionBase def)
        {
            for (int i = 0; i < defs.Length; i++)
            {
                if (def == defs[i])
                    return i;
            }

            return -1;
        }
    }
}