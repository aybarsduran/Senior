using System;
using UnityEngine;

namespace IdenticalStudios
{
    [Serializable]   
    public sealed class AnimatorParameterTrigger
    {
        [SerializeField]
        private AnimatorControllerParameterType m_Type;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private float m_Value;

        [NonSerialized]
        private int m_Hash;


        public AnimatorParameterTrigger(AnimatorControllerParameterType type, string name, float value)
        {
            this.m_Type = type;
            this.m_Name = name;
            this.m_Value = value;
        }

        public void HashParameter()
        {
            m_Hash = Animator.StringToHash(m_Name);

#if !UNITY_EDITOR
            m_Name = string.Empty;
#endif
        }

        public void TriggerParameter(Animator animator)
        {
            switch (m_Type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(m_Name, m_Value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(m_Name);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(m_Name, m_Value > 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(m_Name, (int)m_Value);
                    break;
            }
        }

        public void TriggerParameter(Animator animator, float valueMod)
        {
            float value = m_Value * valueMod;

            switch (m_Type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(m_Name, value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(m_Name);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(m_Name, value > 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(m_Name, (int)value);
                    break;
            }
        }

        public void TriggerHashedParameter(Animator animator)
        {
            switch (m_Type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(m_Hash, m_Value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(m_Hash);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(m_Hash, m_Value > 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(m_Hash, (int)m_Value);
                    break;
            }
        }

        public void TriggerHashedParameter(Animator animator, float valueMod = 1f)
        {
            float value = m_Value * valueMod;

            switch (m_Type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(m_Hash, value);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(m_Hash);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(m_Hash, value > 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(m_Hash, (int)value);
                    break;
            }
        }
    }
}