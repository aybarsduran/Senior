using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
  public static partial class TweenExtensions
  {
    public static Tween<Vector2> TweenAnchoredPosition(this RectTransform self, Vector2 to, float duration) =>
      Tween<Vector2, RectTransform>.Add<AnchoredPositionTween>(self).SetTargetValue(to, duration);

    public static Tween<Vector2> TweenAnchoredPosition(this GameObject self, Vector2 to, float duration) =>
      Tween<Vector2, RectTransform>.Add<AnchoredPositionTween>(self).SetTargetValue(to, duration);
  }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "IdenticalStudios.ProceduralMotion.Tweening")]
    public sealed class AnchoredPositionTween : Tween<Vector2, RectTransform>
    {
        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Vector2 GetDefaultFrom() => component.anchoredPosition;
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
            component.anchoredPosition = currentValue;
        }
    }
}