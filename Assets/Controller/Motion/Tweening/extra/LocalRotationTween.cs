using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public static partial class TweenExtensions
    { 
        public static Tween<Vector3> TweenLocalRotation(this Transform self, Vector3 to, float duration) =>
          Tween<Vector3, Transform>.Add<LocalRotationTween>(self).SetTargetValue(to, duration);

        public static Tween<Vector3> TweenLocalRotation(this GameObject self, Vector3 to, float duration) =>
          Tween<Vector3, Transform>.Add<LocalRotationTween>(self.transform).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [Serializable]
    public sealed class LocalRotationTween : Tween<Vector3, Transform>
    {
        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Vector3 GetDefaultFrom()
        {
            var localEulerAngles = component.localEulerAngles;
            return localEulerAngles;
        }

        protected override Vector3 GetToRelativeToFrom(Vector3 valueFrom, Vector3 valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime) 
        {
            var quatFrom = Quaternion.Euler(m_ValueFrom);
            var quatTo = Quaternion.Euler(valueTo);

            if (easedTime == 0)
                component.localRotation = quatFrom;
            else if (Math.Abs(easedTime - 1) < 0.001f)
                component.localRotation = quatTo;
            else
                component.localRotation = Quaternion.Lerp(quatFrom, quatTo, easedTime);
        }
    }
}