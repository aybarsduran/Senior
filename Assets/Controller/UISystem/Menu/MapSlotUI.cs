using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class MapSlotUI : MonoBehaviour
    {
        public TextMeshProUGUI Text => m_MapNameTxt;
        public Button Button => m_MapSelectBtn;
        public Image Image => m_MapIconImg;

        [SerializeField]
        private TextMeshProUGUI m_MapNameTxt;

        [SerializeField]
        private Button m_MapSelectBtn;

        [SerializeField]
        private Image m_MapIconImg;
    }
}
