using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios
{
    public static class UnityUtils
    {
        private static Camera m_CachedMainCamera;
        public static Camera CachedMainCamera
        {
            get
            {
                if (m_CachedMainCamera == null)
                    m_CachedMainCamera = Camera.main;

                return m_CachedMainCamera;
            }
        }

        #region Editor Problem Utils
        /// <summary>
        /// Sometimes, when you use Unity's built-in OnValidate, it will spam you with a very annoying warning message,
        /// even though nothing has gone wrong. To avoid this, you can run your OnValidate code through this utility.
        /// Runs <paramref name="onValidateAction"/> once, after all inspectors have been updated.
        /// </summary>
        public static void SafeOnValidate(Object obj, UnityAction onValidateAction)
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += _OnValidate;


            void _OnValidate()
            {
                EditorApplication.delayCall -= _OnValidate;

                // Important: this function could be called after the object has been destroyed
                // (ex: during reinitialization when entering Play Mode, when saving it in a prefab...),
                // so to prevent a potential ArgumentNullException, we check if "this" is null.
                // Note: the components we want to modify could also be in this "destroyed" state
                // and trigger an ArgumentNullException.

                // We also check if "this" is dirty, this is to prevent the scene to be marked
                // as dirty as soon as we load it (because we will dirty some components in this function).

                if (obj == null || !EditorUtility.IsDirty(obj)) return;

                onValidateAction();
            }
#endif
        }

        public static bool IsQuittingPlayMode { get; private set; } = false;

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void EnterPlayMode(EnterPlayModeOptions options)
        {
            IsQuittingPlayMode = false;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            IsQuittingPlayMode = false;
            Application.quitting += Quit;
        }

        private static void Quit()
        {
            IsQuittingPlayMode = true;
        }
#endif
        #endregion
    }
}
