using IdenticalStudios.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace IdenticalStudios.UISystem
{
    public sealed class PauseMenuUI : PlayerUIBehaviour
    {
        [SerializeField, FormerlySerializedAs("m_Context")]
        private InputContextGroup m_PauseContext;

        [SerializeField]
        private SerializedScene m_MainMenu;


        [SerializeField]
        private InputActionReference m_PauseInput;


        [SerializeField]
        private SoundPlayer m_PauseSound;

        [SerializeField]
        private SoundPlayer m_ResumeSound;


        [SerializeField, ReorderableList(HasLabels = false)]
        private PanelUI[] m_PausePanels;

        [SerializeField, ReorderableList(HasLabels = false)]
        private PanelUI[] m_ResumePanels;

        private float m_PauseTimer;
        private bool m_IsPaused;


        protected override void OnAttachment()
        {
            Player.HealthManager.Death += Resume;
            m_PauseInput.RegisterPerformed(OnPauseInput);
        }

        protected override void OnDetachment()
        {
            Player.HealthManager.Death -= Resume;
            m_PauseInput.UnregisterPerfomed(OnPauseInput);
        }

        public void QuitToMenu() => LevelManager.LoadScene(m_MainMenu.SceneName);
        public void QuitToDesktop() => Application.Quit();

        public void Pause()
        {
            if (m_IsPaused || Time.time < m_PauseTimer || InputManager.HasEscapeCallbacks)
                return;

            m_PauseTimer = Time.time + 0.3f;
            m_IsPaused = true;

            CursorLocker.AddCursorUnlocker(this);
            InputManager.PushContext(m_PauseContext);
            InputManager.PushEscapeCallback(Resume);
            BlurBackgroundUI.EnableBlur();

            m_PauseSound.Play2D();

            for (int i = 0; i < m_PausePanels.Length; i++)
                m_PausePanels[i].Show(true);
        }

        public void Resume()
        {
            if (!m_IsPaused)
                return;

            m_PauseTimer = Time.time + 0.1f;
            m_IsPaused = false;

            CursorLocker.RemoveCursorUnlocker(this);
            InputManager.PopEscapeCallback(Resume);
            InputManager.PopContext(m_PauseContext);
            BlurBackgroundUI.DisableBlur();

            m_ResumeSound.Play2D();

            for (int i = 0; i < m_ResumePanels.Length; i++)
                m_ResumePanels[i].Show(false);
        }

        private void OnDisable() => Resume();
        private void OnPauseInput(InputAction.CallbackContext context) => Pause();
    }
}