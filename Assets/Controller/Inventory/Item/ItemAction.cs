using IdenticalStudios;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace IdenticalStudios.InventorySystem
{
    public abstract class ItemAction : ScriptableObject
    {
        [SerializeField]
        private string m_DisplayName;

        [SerializeField]
        private string m_DisplayVerb;

        [SerializeField]
        private Sprite m_DisplayIcon;


        public abstract bool IsViableForItem(ICharacter character, ItemSlot itemSlot);
        public abstract float GetDuration(ICharacter character, ItemSlot itemSlot);

        public virtual void StartAction(ICharacter character, ItemSlot itemSlot) { }
        public abstract void PerformAction(ICharacter character, ItemSlot itemSlot);
        public virtual void CancelAction(ICharacter character, ItemSlot itemSlot) { }

        public virtual string GetDisplayName() => m_DisplayName;
        public virtual string GetDisplayVerb() => m_DisplayVerb;
        public virtual Sprite GetDisplayIcon() => m_DisplayIcon;

#if UNITY_EDITOR
        private void Reset()
        {
            m_DisplayName = name;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(m_DisplayName))
            {
                var path = UnityEditor.AssetDatabase.GetAssetPath(this);
                var index = path.LastIndexOf("/") + 10;

                if (path.Length < index)
                    return;

                m_DisplayName = path.Substring(index).Replace(".asset", "");
            }
        }
#endif
    }
}