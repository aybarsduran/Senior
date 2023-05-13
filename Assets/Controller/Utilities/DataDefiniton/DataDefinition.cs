using System.Collections.Generic;
using UnityEngine;
using System;
using IdenticalStudios;
using System.Xml.Linq;

namespace IdenticalStudios
{
    public abstract class DataDefinition<T> : DataDefinitionBase where T : DataDefinition<T>
    {
        public static T[] Definitions
        {
            get
            {
                if (s_Definitions == Array.Empty<T>())
                    LoadDefinitions();

                return s_Definitions;
            }
        }

        protected static Dictionary<int, T> DefinitionsById
        {
            get
            {
                if (s_DefinitionsById == null)
                    CreateIdDefinitionsDict();

                return s_DefinitionsById;
            }
        }

        protected static Dictionary<string, T> DefinitionsByName
        {
            get
            {
                if (s_DefinitionsByName == null)
                    CreateNameDefinitionsDict();

                return s_DefinitionsByName;
            }
        }

        private static T[] s_Definitions = Array.Empty<T>();
        private static Dictionary<int, T> s_DefinitionsById;
        private static Dictionary<string, T> s_DefinitionsByName;


        #region Accessing Methods
        // Tries to return a definition with the given id.
        public static bool TryGetWithId(int id, out T def)
        {
            def = GetWithId(id);
            return def != null;
        }

        //Returns a definition with the given id.
        public static T GetWithId(int id)
        {
            if (DefinitionsById.TryGetValue(id, out T def))
                return def;

            return null;
        }

        // Returns a definition with the given id.
        public static T GetWithIndex(int index)
        {
            if (index >= 0 && index < Definitions.Length)
                return Definitions[index];

            return null;
        }

        // Tries to return a definition with the given name.
        public static bool TryGetWithName(string name, out T def)
        {
            def = GetWithName(name);
            return def != null;
        }

        // Returns a definition with the given name.
        public static T GetWithName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (DefinitionsByName.TryGetValue(name, out T def))
                return def;

            return null;
        }
        #endregion

        #region Definition Loading
        private static void LoadDefinitions()
        {
            string path = "Definitions/" + typeof(T).Name.Replace("Definition", "");

            s_Definitions = Resources.LoadAll<T>(path + "s");

            if (s_Definitions != null && s_Definitions.Length > 0)
                return;

            path = path.Remove(path.Length - 1, 1) + "ies";
            s_Definitions = Resources.LoadAll<T>(path);

            if (s_Definitions != null && s_Definitions.Length > 0)
                return;

            s_Definitions = Resources.LoadAll<T>(path);

            if (s_Definitions != null && s_Definitions.Length > 0)
                return;

            s_Definitions = Array.Empty<T>();
        }

        private static void CreateIdDefinitionsDict()
        {
            if (s_DefinitionsById != null)
                s_DefinitionsById.Clear();
            else
                s_DefinitionsById = new Dictionary<int, T>();

            var definitions = Definitions;
            for (int i = 0; i < definitions.Length; i++)
            {
                T def = definitions[i];


                try
                {
                    s_DefinitionsById.Add(def.Id, def);
                }
                catch
                {
                    Debug.LogError($"Multiple '{typeof(T).Name.ToUnityLikeNameFormat()}' of the same id are found. Restarting Unity should fix this problem.");
                }
            }
        }

        private static void CreateNameDefinitionsDict()
        {
            if (s_DefinitionsByName != null)
                s_DefinitionsByName.Clear();
            else
                s_DefinitionsByName = new Dictionary<string, T>();

            var definitions = Definitions;
            for (int i = 0; i < definitions.Length; i++)
            {
                T def = definitions[i];

                try
                {
                    s_DefinitionsByName.Add(def.Name, def);
                }
                catch
                {
                    Debug.LogError($"Multiple '{typeof(T).Name.ToUnityLikeNameFormat()}' of the same name are found. Make sure the names are unique.");
                }
            }
        }
        #endregion

    }
}