using UnityEngine;

namespace IdenticalStudios.ProceduralMotion 
{
  public static partial class TweenExtensions
  {
    public static Tween<Color> TweenMaterialColor(this MeshRenderer self, Color to, float duration) =>
      Tween<Color, MeshRenderer>.Add<MaterialColorTween>(self).SetTargetValue(to, duration);

    public static Tween<Color> TweenMaterialColor(this GameObject self, Color to, float duration) =>
      Tween<Color, MeshRenderer>.Add<MaterialColorTween>(self).SetTargetValue(to, duration);
  }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [System.Serializable]
    public sealed class MaterialColorTween : Tween<Color, MeshRenderer>
    {
        private Material material;
        private Color color;


        protected override bool TryInitialize() 
        {
            if (!base.TryInitialize())
            return false;

            material = component.material;
            return true;
        }

        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override Color GetDefaultFrom() => material.color;
        protected override Color GetToRelativeToFrom(Color valueFrom, Color valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime)
        {
            color = material.color;
            currentValue.r = InterpolateValue (m_ValueFrom.r, valueTo.r, easedTime);
            currentValue.g = InterpolateValue (m_ValueFrom.g, valueTo.g, easedTime);
            currentValue.b = InterpolateValue (m_ValueFrom.b, valueTo.b, easedTime);
            currentValue.a = InterpolateValue (m_ValueFrom.a, valueTo.a, easedTime);
            color = currentValue;
            material.color = color;
        }
    }
}