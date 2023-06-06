using UnityEngine;

namespace IdenticalStudios.ProceduralMotion 
{
    public static partial class TweenExtensions
    {
        /// <summary>
        /// Instantiates a tween which changes the <see cref="Light"/>'s range over
        /// time. The Light range is only avaiable on Spot, Point and Area Lights.
        /// </summary>
        /// <param name="self">The target Component.</param>
        /// <param name="to">The target value.</param>
        /// <param name="duration">The Tween's duration.</param>
        /// <returns>A Tween.</returns>
        public static Tween<float> TweenLightRange(this Light self, float to, float duration) =>
            Tween<float, Light>.Add<LightRangeTween>(self).SetTargetValue(to, duration);

        /// <summary>
        /// Instantiates a tween which changes the <see cref="Light"/>'s range over
        /// time. The Light range is only avaiable on Spot, Point and Area Lights.
        /// </summary>
        /// <param name="self">The target GameObject.</param>
        /// <param name="to">The target value.</param>
        /// <param name="duration">The Tween's duration.</param>
        /// <returns>A Tween.</returns>
        public static Tween<float> TweenLightRange(this GameObject self, float to, float duration) =>
            Tween<float, Light>.Add<LightRangeTween>(self).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    public sealed class LightRangeTween : Tween<float, Light>
    {
        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override float GetDefaultFrom() => component.range;
        protected override float GetToRelativeToFrom(float valueFrom, float valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime)
        {
            currentValue = InterpolateValue (m_ValueFrom, valueTo, easedTime);
            component.range = currentValue;
        }
    }
}