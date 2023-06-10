using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    [AddComponentMenu(k_AddMenuPath + "Events Info")]
    public class EventsInfoUI : GenericDataInfoUI
    {
        public event UnityAction OnInfoChanged
        {
            add => m_OnInfoChanged.AddListener(value);
            remove => m_OnInfoChanged.RemoveListener(value);
        }

        public event UnityAction OnInfoDisabled
        {
            add => m_OnInfoDisabled.AddListener(value);
            remove => m_OnInfoDisabled.RemoveListener(value);
        }

        [SerializeField]
        private UnityEvent m_OnInfoChanged;

        [SerializeField]
        private UnityEvent m_OnInfoDisabled;


        public override void OnDataChanged() => m_OnInfoChanged.Invoke();
        public override void OnDataDisabled() => m_OnInfoDisabled.Invoke();
    }
}
