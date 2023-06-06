using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public static partial class TweenExtensions
    {
        public static Tween<float> TweenValueFloat(this GameObject self, float to, float duration, Action<float> onUpdate) =>
          Tween<float>.Add<ValueFloatTween>().SetUpdateCallback(onUpdate).SetTargetValue(to, duration);
    }
    
    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    public sealed class ValueFloatTween : Tween<float>
    {
        private Action<float> m_OnUpdate;


        protected override bool TryInitialize() => true;

        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override float GetDefaultFrom () => 0f;
        protected override float GetToRelativeToFrom(float valueFrom, float valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime)
        {
            currentValue = InterpolateValue (m_ValueFrom, valueTo, easedTime);
            m_OnUpdate?.Invoke(currentValue);
        }

        public Tween<float> SetUpdateCallback (Action<float> callback) 
        {
            m_OnUpdate = callback;
            return this;
        }
    }
}