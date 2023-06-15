using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.ProceduralMotion
{
  public static partial class TweenExtensions
  {
    public static Tween<Color> TweenGraphicColor(this Graphic self, Color to, float duration) =>
      Tween<Color, Graphic>.Add<GraphicColorTween>(self).SetTargetValue(to, duration);

    public static Tween<Color> TweenGraphicColor(this GameObject self, Color to, float duration) =>
      Tween<Color, Graphic>.Add<GraphicColorTween>(self).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "IdenticalStudios.ProceduralMotion.Tweening")]
    public sealed class GraphicColorTween : Tween<Color, Graphic>
    {
        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Color GetDefaultFrom() => component.color;
        protected override Color GetToRelativeToFrom(Color valueFrom, Color valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime)
        {
            currentValue.r = InterpolateValue (m_ValueFrom.r, valueTo.r, easedTime);
            currentValue.g = InterpolateValue (m_ValueFrom.g, valueTo.g, easedTime);
            currentValue.b = InterpolateValue (m_ValueFrom.b, valueTo.b, easedTime);
            currentValue.a = InterpolateValue (m_ValueFrom.a, valueTo.a, easedTime);
            component.color = currentValue;
        }
    }
}