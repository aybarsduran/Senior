using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class CursorLocker : MonoBehaviour
    {
        private static readonly List<Object> s_CursorUnlockers = new List<Object>();


        public static void RemoveCursorUnlocker(Object obj)
        {
            if (s_CursorUnlockers.Remove(obj))
                RemoveAllNullUnlockers();

            // Lock cursor if there's no "unlockers" left.
            if (s_CursorUnlockers.Count == 0)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public static void AddCursorUnlocker(Object obj)
        {
            if (s_CursorUnlockers.Contains(obj))
                return;

            s_CursorUnlockers.Add(obj);
            RemoveAllNullUnlockers();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private static void RemoveAllNullUnlockers()
        {
            int i = 0;
            while (i < s_CursorUnlockers.Count)
            {
                if (s_CursorUnlockers[i] == null)
                    s_CursorUnlockers.RemoveAt(i);
                else
                    i++;
            }
        }

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}