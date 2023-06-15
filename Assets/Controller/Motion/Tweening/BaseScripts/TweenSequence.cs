using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public sealed class TweenSequence
    {
        [SerializeReference, ReorderableList(ListStyle.Boxed, childLabel: "Tween", HasHeader = false)]
        [ReferencePicker]
        private ITween[] m_Tweens;

        
        public void PlayAnimation()
        {
            for (int i = 0; i < m_Tweens.Length; i++)
                m_Tweens[i].Play();
        }
        
        public void CancelAnimation()
        {
            for (int i = 0; i < m_Tweens.Length; i++)
                m_Tweens[i].Cancel();
        }

        public void OnValidate(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (m_Tweens == null || Application.isPlaying)
                return;

            foreach (var tween in m_Tweens)
                tween?.OnEditorValidate(gameObject);
#endif
        }
    }
}