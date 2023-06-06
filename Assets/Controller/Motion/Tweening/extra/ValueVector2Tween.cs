using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public static partial class TweenExtensions
    {
        public static Tween<Vector2> TweenValueVector2(this GameObject self, Vector2 to, float duration, Action<Vector2> onUpdate) =>
          Tween<Vector2>.Add<ValueVector2Tween>().SetOnUpdate(onUpdate).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    public sealed class ValueVector2Tween : Tween<Vector2>
    {
        private Action<Vector2> m_OnUpdate;


        protected override bool TryInitialize() => true;

        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Vector2 GetDefaultFrom() => Vector2.zero;
        protected override Vector2 GetToRelativeToFrom(Vector2 valueFrom, Vector2 valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime)
        {
            currentValue.x = InterpolateValue (m_ValueFrom.x, valueTo.x, easedTime);
            currentValue.y = InterpolateValue (m_ValueFrom.y, valueTo.y, easedTime);
            m_OnUpdate?.Invoke(currentValue);
        }

        public Tween<Vector2> SetOnUpdate (Action<Vector2> onUpdate)
        {
            m_OnUpdate = onUpdate;
            return this;
        }
    }
}