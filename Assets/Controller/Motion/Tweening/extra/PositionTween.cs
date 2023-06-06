using UnityEngine;

namespace IdenticalStudios.ProceduralMotion 
{
  public static partial class TweenExtensions
  {
    public static Tween<Vector3> TweenPosition(this Transform self, Vector3 to, float duration) =>
      Tween<Vector3, Transform>.Add<PositionTween>(self).SetTargetValue(to, duration);

    public static Tween<Vector3> TweenPosition(this GameObject self, Vector3 to, float duration) =>
      Tween<Vector3, Transform>.Add<PositionTween>(self.transform).SetTargetValue(to, duration);
  }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [System.Serializable]
    public sealed class PositionTween : Tween<Vector3, Transform> 
    {
        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Vector3 GetDefaultFrom() => component.position;
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
            component.position = currentValue;
        }
    }
}