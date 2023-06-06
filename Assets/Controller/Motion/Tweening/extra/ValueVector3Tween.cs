using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public static partial class TweenExtensions
    {
        public static Tween<Vector3> TweenValueVector3(this GameObject self, Vector3 to, float duration, Action<Vector3> onUpdate) =>
          Tween<Vector3>.Add<ValueVector3Tween>().SetOnUpdate(onUpdate).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    public sealed class ValueVector3Tween : Tween<Vector3>
    {
        private Action<Vector3> m_OnUpdate;
      
      
        protected override bool TryInitialize() => true;

        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Vector3 GetDefaultFrom() => Vector3.zero;
        protected override Vector3 GetToRelativeToFrom(Vector3 valueFrom, Vector3 valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime) 
        {
            currentValue.x = InterpolateValue (m_ValueFrom.x, valueTo.x, easedTime);
            currentValue.y = InterpolateValue (m_ValueFrom.y, valueTo.y, easedTime);
            currentValue.z = InterpolateValue (m_ValueFrom.z, valueTo.z, easedTime);
            m_OnUpdate?.Invoke (currentValue);
        }

        public Tween<Vector3> SetOnUpdate (Action<Vector3> onUpdate) 
        {
            m_OnUpdate = onUpdate;
            return this;
        }
    }
}