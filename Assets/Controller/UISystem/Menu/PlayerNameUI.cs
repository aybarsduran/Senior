using TMPro;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class PlayerNameUI : MonoBehaviour
    {
        [SerializeField]
        private PanelUI m_Panel;

        [SerializeField]
        private TextMeshProUGUI m_NameText;

        [SerializeField]
        private TMP_InputField m_NameInputField;

        private const string k_PlayerNamePrefName = "IDENTICAL_PLAYER_NAME";
        private const string k_UnnamedPlayerName = "Unnamed";


        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        public void SavePlayerNameFromField()
        {
            if (string.IsNullOrEmpty(m_NameInputField.text))
                return;

            SavePlayerName(m_NameInputField.text);
        }

        public void SavePlayerName(string name)
        {
            PlayerPrefs.SetString(k_PlayerNamePrefName, name);
            m_NameText.text = name;
        }

        public void ResetPlayerNameField()
        {
            m_NameInputField.text = PlayerPrefs.GetString(k_PlayerNamePrefName);
        }

        private void Start()
        {
            if (!PlayerPrefs.HasKey(k_PlayerNamePrefName) || PlayerPrefs.GetString(k_PlayerNamePrefName) == string.Empty)
            {
                SavePlayerName(k_UnnamedPlayerName);
                m_Panel.Show(true);
            }
            else
                m_NameInputField.text = m_NameText.text = PlayerPrefs.GetString(k_PlayerNamePrefName);
        }
    }
}
