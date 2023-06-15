using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IdenticalStudios
{
    public static class AssetDatabaseHelper
    {
        /// <summary>
        /// Finds and returns the most similar asset name wise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameToCompare"></param>
        /// <returns></returns>
        public static T FindClosestMatchingObjectWithName<T>(string nameToCompare) where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            string[] fullPaths = new string[guids.Length];

            if (fullPaths.Length == 0)
                return null;

            for (int i = 0; i < guids.Length; i++)
                fullPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

            string[] names = fullPaths.ToArray();

            for (int i = 0; i < names.Length; i++)
            {
                int indexOfLast = -1;

                for (int j = 0; j < names[i].Length; j++)
                {
                    if (names[i][j] == '/')
                        indexOfLast = j;
                }

                if (indexOfLast != -1)
                    names[i] = names[i].Remove(0, indexOfLast);
            }

            int mostSimilarIndex = -1;
            int similarityValue = int.MaxValue;

            for (int i = 0; i < names.Length; i++)
            {
                int similarity = names[i].DamerauLevenshteinDistanceTo(nameToCompare);

                if (similarity < similarityValue)
                {
                    similarityValue = similarity;
                    mostSimilarIndex = i;
                }
            }

            if (mostSimilarIndex != -1)
                return AssetDatabase.LoadAssetAtPath<T>(fullPaths[mostSimilarIndex]);

            return null;
        }

        /// <summary>
        /// Finds and returns the most similar asset name wise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameToCompare"></param>
        /// <returns></returns>
        public static T FindClosestMatchingPrefab<T>(string nameToCompare, string ignore) where T : UnityEngine.Object
        {
            var guids = AssetDatabase.FindAssets($"t:Prefab");
            string[] fullPaths = new string[guids.Length];

            if (fullPaths.Length == 0)
                return null;

            for (int i = 0; i < guids.Length; i++)
                fullPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

            string[] names = fullPaths.ToArray();

            for (int i = 0; i < names.Length; i++)
            {
                int indexOfLast = -1;

                for (int j = 0; j < names[i].Length; j++)
                {
                    if (names[i][j] == '/')
                        indexOfLast = j;
                }

                if (indexOfLast != -1)
                    names[i] = names[i].Remove(0, indexOfLast);
            }

            int mostSimilarIndex = -1;
            int similarityValue = int.MaxValue;

            for (int i = 0; i < names.Length; i++)
            {
                string prefabName = names[i].Contains(ignore) ? names[i].Substring(ignore.Length -1) : names[i];
                int similarity = prefabName.DamerauLevenshteinDistanceTo(nameToCompare);

                if (AssetDatabase.LoadAssetAtPath<GameObject>(fullPaths[i]).GetComponent<T>() == null)
                    continue;

                if (similarity < similarityValue)
                {
                    similarityValue = similarity;
                    mostSimilarIndex = i;
                }
            }

            if (mostSimilarIndex != -1)
                return AssetDatabase.LoadAssetAtPath<T>(fullPaths[mostSimilarIndex]);

            return null;
        }
    }
}