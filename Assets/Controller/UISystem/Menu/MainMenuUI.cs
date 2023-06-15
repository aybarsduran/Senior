using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public sealed class MainMenuUI : MonoBehaviour
    {
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        private void OnEnable() => CursorLocker.AddCursorUnlocker(this);
        private void OnDisable() => CursorLocker.RemoveCursorUnlocker(this);
    }
}