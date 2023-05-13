using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IdenticalStudios
{
    public static class UnityExtensions
    {
        public static Transform FindDeepChild(this Transform parent, string childName)
        {
            var result = parent.Find(childName);

            if (result)
                return result;

            for (int i = 0; i < parent.childCount; i++)
            {
                result = parent.GetChild(i).FindDeepChild(childName);
                if (result)
                    return result;
            }

            return null;
        }

        public static void SetLocalPositionAndRotation(this Transform trs, Vector3 localPosition, Quaternion localRotation)
        {
            trs.localPosition = localPosition;
            trs.localRotation = localRotation;
        }

        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            foreach (Transform child in gameObject.transform)
                child.gameObject.SetLayerRecursively(layer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActive(this IEnumerable<GameObject> gameObjects, bool active)
        {
            foreach (var gameObject in gameObjects)
                gameObject.SetActive(active);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetRandomPoint(this Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LocalToWorld(this Vector3 vector, Transform transform)
        {
            return transform.rotation * vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LocalToWorld(this Transform transform, Vector3 localPosition)
        {
            return transform.right * localPosition.x + transform.up * localPosition.y + transform.forward * localPosition.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetHorizontal(this Vector3 vector)
        {
            return new Vector3(vector.x, 0f, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAbsoluteValue(this float value)
        {
            return value < 0f ? -value : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetAbsoluteValue(this int value)
        {
            return value < 0 ? -value : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Jitter(this float refFloat, float jitter)
        {
            return refFloat + Random.Range(-jitter, jitter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Jitter(this Vector3 refVector, float jitter)
        {
            return new Vector3(
                refVector.x + Random.Range(-jitter, jitter),
                refVector.y + Random.Range(-jitter, jitter),
                refVector.z + Random.Range(-jitter, jitter));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Jitter(this Vector3 refVector, float xJit, float yJit, float zJit)
        {
            refVector.x -= Mathf.Abs(refVector.x * Random.Range(0, xJit)) * 2f;
            refVector.y -= Mathf.Abs(refVector.y * Random.Range(0, yJit)) * 2f;
            refVector.z -= Mathf.Abs(refVector.z * Random.Range(0, zJit)) * 2f;

            return refVector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetRandomFloat(this Vector2 vector)
        {
            return Random.Range(vector.x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetRandomInt(this Vector2Int vector)
        {
            return Random.Range(vector.x, vector.y + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReverseVector(this ref Vector2 vector)
        {
            float xValue = vector.x;
            vector.x = vector.y;
            vector.y = xValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponentInRoot<T>(this GameObject gameObj)
        {
            return gameObj.transform.root.GetComponentInChildren<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrAddComponent<T>(this GameObject gameObj) where T : Component
        {
            return gameObj.TryGetComponent(out T comp) ? comp : gameObj.AddComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent(this GameObject gameObject, System.Type type)
        {
            return gameObject.GetComponent(type) != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static T GetComponentInFirstChildren<T>(this GameObject gameObj) where T : class
        {
            var transform = gameObj.transform;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<T>(out var comp))
                    return comp;
            }

            return null;
        }

        public static List<T> GetComponentsInFirstChildren<T>(this GameObject gameObj) where T : class
        {
            var components = new List<T>();

            var transform = gameObj.transform;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<T>(out var comp))
                    components.Add(comp);
            }

            return components;
        }

    }
}