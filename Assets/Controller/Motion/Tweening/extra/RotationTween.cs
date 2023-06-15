using System;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion 
{
    public static partial class TweenExtensions
    {
        public static Tween<Vector3> TweenRotation(this Transform self, Vector3 to, float duration) =>
            Tween<Vector3, Transform>.Add<RotationTween>(self).SetTargetValue(to, duration);

        public static Tween<Vector3> TweenRotation(this GameObject self, Vector3 to, float duration) =>
            Tween<Vector3, Transform>.Add<RotationTween>(self.transform).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "IdenticalStudios.ProceduralMotion.Tweening")]
    public sealed class RotationTween : Tween<Vector3, Transform>
    {
        private Quaternion m_QuaternionValueFrom;
        private Quaternion quaternionValueTo;
        private bool m_DidConvertValueFromToQuanternion;

        protected override bool TryInitialize ()
        {
            quaternionValueTo = Quaternion.Euler (valueTo);
            return base.TryInitialize ();
        }

        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Vector3 GetDefaultFrom()
        {
            var eulerAngles = component.eulerAngles;
            m_QuaternionValueFrom = Quaternion.Euler (eulerAngles);
            m_DidConvertValueFromToQuanternion = true;
            return eulerAngles;
        }

        protected override Vector3 GetToRelativeToFrom(Vector3 valueFrom, Vector3 valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime)
        {
            if (m_DidConvertValueFromToQuanternion == false) 
            {
                m_QuaternionValueFrom = Quaternion.Euler (m_ValueFrom);
                m_DidConvertValueFromToQuanternion = true;
            }
      
            if (easedTime == 0)
                component.rotation = m_QuaternionValueFrom;
            else if (Math.Abs(easedTime - 1) < 0.001f)
                component.rotation = quaternionValueTo;
            else
                component.rotation = Quaternion.Lerp (m_QuaternionValueFrom, quaternionValueTo, easedTime);
        }
    }
}