using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.UISystem
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class ControlUI : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference m_InputAction;

        [SerializeField]
        private string m_TextTemplate = "{}";

        [SerializeField]
        private Color m_TextInputColor = Color.white;


        private void Start()
        {
            if (m_InputAction == null)
            {
                Debug.LogError("ACTION NULL", gameObject);
                return;
            }

            SetText();

#if !UNITY_EDITOR
            Destroy(this);
#endif
        }

        private void SetText()
        {
            if (TryGetComponent<TextMeshProUGUI>(out var text))
                text.text = m_TextTemplate.Replace("{}", $"<color={ColorToHex(m_TextInputColor)}>{m_InputAction.action.bindings[0].ToDisplayString()}</color>");
        }

        private string ColorToHex(Color32 color)
        {
            string hex = "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!m_TextTemplate.Contains("{}"))
                m_TextTemplate += "{}";

            if (m_InputAction != null)
                SetText();
        }
#endif
    }
}