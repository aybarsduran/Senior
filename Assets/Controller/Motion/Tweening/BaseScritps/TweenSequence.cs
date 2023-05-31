using IdenticalStudios.ProceduralMotion;
using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [Serializable]
    public sealed class TweenSequence
    {
        public ITween[] m_Tweens;


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

        }
    }
}