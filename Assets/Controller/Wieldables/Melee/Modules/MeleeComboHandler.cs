using System;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class MeleeComboHandler : MonoBehaviour
    {
        #region Internal
        [Serializable]
        private class SwingCombo
        {
            [NonSerialized]
            public int LastSelected = -1;

            public SelectionType SelectionType = SelectionType.Sequence;

            [ReorderableList(ListStyle.Lined, HasLabels = false)]
            public MeleeSwingBehaviour[] Swings;
        }
        #endregion

        public IMeleeSwing LastMeleeSwing { get; private set; }

        [SerializeField, ReorderableList()]
        private SwingCombo[] m_Combos;

        private float m_NextPossibleSwingTime;


        /// <summary>
        /// Tries to do a melee attack using its assigned swings.
        /// </summary>
        /// <returns> True if successful </returns>
        public bool TryMelee(float accuracy)
        {
            if (Time.time < m_NextPossibleSwingTime || m_Combos.Length == 0)
                return false;

            int comboIndex = GetSwingWithHighestPriority(accuracy);
            SwingCombo swingCombo = m_Combos[comboIndex];

            LastMeleeSwing = swingCombo.Swings.Select(ref swingCombo.LastSelected, swingCombo.SelectionType);
            LastMeleeSwing.DoSwing(accuracy);
            m_NextPossibleSwingTime = Time.time + LastMeleeSwing.SwingDuration;

            return true;
        }

        public void CancelAllSwings()
        {
            foreach (var combo in m_Combos)
            {
                foreach (var swing in combo.Swings)
                    swing.CancelSwing();
            }
        }

        private int GetSwingWithHighestPriority(float accuracy)
        {
            int swingComboIndex = 0;

            for (int i = 0; i < m_Combos.Length; i++)
            {
                var firstSwing = m_Combos[i].Swings[0];

                if (firstSwing != null && firstSwing.GetSwingValidity(accuracy))
                {
                    swingComboIndex = i;
                    break;
                }
            }

            return Mathf.Clamp(swingComboIndex, 0, m_Combos.Length - 1);
        }
    }
}
