using IdenticalStudios.UISystem;
using PolymindGames.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class DeathUI : PlayerUIBehaviour
    {
        [SerializeField]
        private InputContextGroup m_DeathContext;


        [SerializeField, Range(0f, 10f)]
        private float m_FadeDeathDelay = 2f;

        [SerializeField, Range(0f, 10f)]
        private float m_FadeSpawnDelay = 3f;

        [SerializeField, Range(0f, 50f)]
        private float m_ShowRespawnButtonDelay = 5f;


        [SerializeField]
        private TextMeshProUGUI m_RespawnTimeText;

        [SerializeField]
        private Button m_RespawnButton;


        [SerializeField]
        private AudioMixerSnapshot m_NotAliveSnapshot;

        [SerializeField]
        private AudioMixerSnapshot m_DefaultSnapshot;


        /// <summary>
        /// Respawn the player by restoring the health to the max amount
        /// </summary>
        public void RespawnPlayer()
        {
            if (!m_RespawnTimeText.enabled)
                return;

            Player.HealthManager.RestoreHealth(Player.HealthManager.MaxHealth);

            // Audio
            m_DefaultSnapshot.TransitionTo(m_FadeSpawnDelay * 2f);

            m_RespawnButton.gameObject.SetActive(false);
            m_RespawnTimeText.enabled = false;

            InputManager.PopContext(m_DeathContext);
            BlurBackgroundUI.DisableBlur();
        }

        protected override void OnAttachment()
        {
            m_RespawnButton.onClick.AddListener(RespawnPlayer);

            m_RespawnButton.gameObject.SetActive(false);
            m_RespawnTimeText.enabled = false;

            Player.HealthManager.Death += OnPlayerDeath;
        }

        protected override void OnDetachment()
        {
            m_RespawnButton.onClick.RemoveListener(RespawnPlayer);
            Player.HealthManager.Death -= OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            InputManager.PushContext(m_DeathContext);
            BlurBackgroundUI.EnableBlur();
            m_NotAliveSnapshot.TransitionTo(m_FadeDeathDelay * 2f);

            StartCoroutine(C_ShowRespawnPanel());
        }

        private IEnumerator C_ShowRespawnPanel()
        {
            m_RespawnButton.gameObject.SetActive(true);
            m_RespawnButton.interactable = false;
            m_RespawnTimeText.enabled = true;

            float currentTimeLeft = m_ShowRespawnButtonDelay;

            while (currentTimeLeft > 0.01f)
            {
                m_RespawnTimeText.text = currentTimeLeft.ToString("0.0");

                currentTimeLeft -= Time.deltaTime;

                yield return null;
            }

            m_RespawnTimeText.text = "Respawn";
            m_RespawnButton.interactable = true;
        }
    }
}