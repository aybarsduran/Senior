using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public static partial class TweenExtensions
    {
        public static Tween<Color> TweenValueColor(this GameObject self, Color to, float duration, Action<Color> onUpdate) =>
          Tween<Color>.Add<ValueColorTween>().SetOnUpdate(onUpdate).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    public sealed class ValueColorTween : Tween<Color>
    {
        private Action<Color> m_OnUpdate;
        
        
        protected override bool TryInitialize() => true;

        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Color GetDefaultFrom() => Color.black;
        protected override Color GetToRelativeToFrom(Color valueFrom, Color valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate(float easedTime)
        {
            currentValue.r = InterpolateValue (m_ValueFrom.r, valueTo.r, easedTime);
            currentValue.g = InterpolateValue (m_ValueFrom.g, valueTo.g, easedTime);
            currentValue.b = InterpolateValue (m_ValueFrom.b, valueTo.b, easedTime);
            currentValue.a = InterpolateValue (m_ValueFrom.a, valueTo.a, easedTime);
            m_OnUpdate?.Invoke (currentValue);
        }

        public Tween<Color> SetOnUpdate(Action<Color> onUpdate)
        {
            m_OnUpdate = onUpdate;
            return this;
        }
    }
}