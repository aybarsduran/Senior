using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [AddComponentMenu(k_AddMenuPath + "Object Toggle Info")]
    public class ObjectToggleInfoUI : GenericDataInfoUI
    {
        [SerializeField, ReorderableList(HasLabels = false)]
        private GameObject[] m_ObjectsToToggle;

        private bool m_Enabled;


        public override void OnDataChanged()
        {
            if (m_Enabled)
                return;

            m_Enabled = true;
            for (int i = 0; i < m_ObjectsToToggle.Length; i++)
                m_ObjectsToToggle.SetActive(true);
        }

        public override void OnDataDisabled()
        {
            m_Enabled = false;
            for (int i = 0; i < m_ObjectsToToggle.Length; i++)
                m_ObjectsToToggle.SetActive(false);
        }
    }
}
