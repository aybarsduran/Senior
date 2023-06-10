using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [System.Serializable]
    public sealed class DynamicEffectCollection
    {
        [SerializeReference]
        [ReorderableList(ListStyle.Lined, childLabel: "Effect")]
        private IWieldableEffect[] m_StaticEffects;


        [SerializeReference]
        [ReorderableList(ListStyle.Lined, childLabel: "Effect")]
        private IWieldableEffect[] m_DynamicEffects;

#if !UNITY_EDITOR
        private bool m_IsInitialized;
#endif


        public DynamicEffectCollection(IWieldableEffect[] staticEffects, IWieldableEffect[] dynamicEffects)
        {
            m_StaticEffects = staticEffects;
            m_DynamicEffects = dynamicEffects;
        }

        public void PlayEffects(IWieldable wieldable, float value)
        {
#if !UNITY_EDITOR
            // Initialize.
            if (!m_IsInitialized)
            {
                m_IsInitialized = true;
                for (int i = 0; i < m_StaticEffects.Length; i++)
                {
                    IWieldableEffect effect = m_StaticEffects[i];
                    effect.OnInitialized(wieldable);
                }

                for (int i = 0; i < m_DynamicEffects.Length; i++)
                {
                    IWieldableEffect effect = m_DynamicEffects[i];
                    effect.OnInitialized(wieldable);
                }
            }

            for (int i = 0; i < m_DynamicEffects.Length; i++)
                m_DynamicEffects[i].PlayEffectDynamically(value);
            
            for (int i = 0; i < m_StaticEffects.Length; i++)
                m_StaticEffects[i].PlayEffect();
#else
            if (wieldable == null)
            {
                Debug.LogError($"The passed wieldable is null.");
                return;
            }

            for (int i = 0; i < m_StaticEffects.Length; i++)
            {
                var effect = m_StaticEffects[i];

                if (!effect.IsInitialized)
                    effect.OnInitialized(wieldable);

                effect.PlayEffect();
            }

            for (int i = 0; i < m_DynamicEffects.Length; i++)
            {
                var effect = m_DynamicEffects[i];

                if (!effect.IsInitialized)
                    effect.OnInitialized(wieldable);

                effect.PlayEffect();
            }
#endif
        }
    }
}
