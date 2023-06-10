using UnityEngine;

namespace IdenticalStudios.InputSystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Input/Input Contexts Asset", fileName = "(InputContexts) ")]
    public class InputContexts : ScriptableObject
    {
        [SerializeField]
        private InputContextGroup m_Groups;

        [SerializeField]
        private InputContext[] m_Contexts;
    }
}
