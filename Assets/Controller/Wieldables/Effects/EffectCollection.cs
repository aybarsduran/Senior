using UnityEngine;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [System.Serializable]
    public sealed class EffectCollection
    {
        [SpaceArea(3f, 3f)]
        [BeginGroup] [EndGroup] [NewLabel("Effects")]
        [SerializeReference, ReferencePicker, ReorderableList(ListStyle.Lined, childLabel:"Effect")] 
        private IWieldableEffect[] m_StaticEffects;

#if !UNITY_EDITOR
        private bool m_IsInitialized;
#endif


        public EffectCollection(IWieldableEffect[] staticEffects)
        {
            m_StaticEffects = staticEffects;
        }

        public void PlayEffects(IWieldable wieldable)
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
            }

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
#endif
        }
    }
}
