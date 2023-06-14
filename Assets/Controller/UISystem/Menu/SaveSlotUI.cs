using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class SaveSlotUI : MonoBehaviour
    {
        public Button Button => m_Button;

        [SerializeField]
        private Button m_Button;

        [SpaceArea]

        [SerializeField]
        private RawImage m_Screenshot;

        [SpaceArea]

        [SerializeField]
        private TextMeshProUGUI m_SaveName;

        [SerializeField]
        private TextMeshProUGUI m_SaveTime;

        [SerializeField]
        private TextMeshProUGUI m_MapName;

        [SpaceArea]

        [SerializeField]
        private GameObject m_NoSaveObject;


        public void ShowSave(Texture screenshot, string saveName, string saveTime, string mapName)
        {
            m_Screenshot.gameObject.SetActive(true);
            m_Screenshot.texture = screenshot;

            m_SaveName.text = saveName;
            m_SaveTime.text = saveTime;
            m_MapName.text = mapName;

            m_NoSaveObject.SetActive(false);
        }

        public void ShowNoSave()
        {
            m_Screenshot.gameObject.SetActive(false);

            m_SaveName.text = string.Empty;
            m_SaveTime.text = string.Empty;
            m_MapName.text = string.Empty;

            m_NoSaveObject.SetActive(true);
        }
    }
}