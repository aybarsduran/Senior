using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    
    public sealed class ButtonAudioUI : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField]
        private SoundPlayer m_OnClickAudio;

        [SerializeField]
        private SoundPlayer m_PointerEnterAudio;


        public void OnPointerEnter(PointerEventData eventData) => m_PointerEnterAudio.Play2D();
        private void Awake() => GetComponent<Button>().onClick.AddListener(OnButtonClick);
        private void OnButtonClick() => m_OnClickAudio.Play2D();
    }
}