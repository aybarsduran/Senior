using System.Collections.Generic;

namespace IdenticalStudios.MovementSystem
{
    public sealed class MovementModifier
    {
        public delegate float ModifierDelegate();

        private readonly List<ModifierDelegate> m_Modifiers = new();


        public float Get()
        {
            float mod = 1f;

            for (int i = 0; i < m_Modifiers.Count; i++)
                mod *= m_Modifiers[i].Invoke();

            return mod;
        }

        public void Add(ModifierDelegate modifier)
        {
            if (modifier == null)
                return;

            if (!m_Modifiers.Contains(modifier))
                m_Modifiers.Add(modifier);
        }

        public void Remove(ModifierDelegate modifier)
        {
            if (modifier == null)
                return;

            m_Modifiers.Remove(modifier);
        }
    }
}
