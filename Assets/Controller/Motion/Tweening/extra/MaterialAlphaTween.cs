using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
  public static partial class TweenExtensions
  {
    public static Tween<float> TweenMaterialAlpha(this MeshRenderer self, float to, float duration) =>
      Tween<float, MeshRenderer>.Add<MaterialAlphaTween>(self).SetTargetValue(to, duration);

    public static Tween<float> TweenMaterialAlpha(this GameObject self, float to, float duration) =>
      Tween<float, MeshRenderer>.Add<MaterialAlphaTween>(self).SetTargetValue(to, duration);
  }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [System.Serializable]
    public sealed class MaterialAlphaTween : Tween<float, MeshRenderer>
    {
        private Material material;
        private Color color;

        protected override bool TryInitialize ()
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
        protected override float GetDefaultFrom() => material.color.a;
        protected override float GetToRelativeToFrom(float valueFrom, float valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime) {
            color = material.color;
            currentValue = InterpolateValue (m_ValueFrom, valueTo, easedTime);
            color.a = currentValue;
            material.color = color;
        }
    }
}