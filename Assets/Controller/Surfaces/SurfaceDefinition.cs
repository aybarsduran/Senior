using System;
using UnityEngine;

namespace IdenticalStudios.Surfaces
{
    [CreateAssetMenu(menuName = "Identical Studios/Surfaces/Surface Definition", fileName = "(Surface) ")]
    public sealed class SurfaceDefinition : DataDefinition<SurfaceDefinition>
    {
        #region Internal
        [Serializable]

        public class EffectPair
        {
            public SoundPlayer AudioEffect;
            public GameObject VisualEffect;
        }
        #endregion

        public override string Name => this != null ? name.Replace("(Surface) ", "") : string.Empty;

        [Title("Settings")]

        [Help("Velocity multiplier applied to a character that steps on this surface", UnityMessageType.None), Range(0.01f, 2f)]
        public float VelocityModifier = 1f;

        [SpaceArea]

        [Help("Determines how rough this surface is (0 - Slippery, 1 - Rough)", UnityMessageType.None), Range(0.01f, 1f)]
        public float SurfaceFriction = 1f;

        [SpaceArea]

        [Help("Determines how penetrable this surface is (0 - easily penetrable, 1 - not penetrable)", UnityMessageType.None), Range(0f, 1f)]
        public float PenetrationResistence = 0.3f;

        [Title("Effects"), SpaceArea]

        public EffectPair SoftFootstepEffect;

        [Line(1f)]
        public EffectPair HardFootstepEffect;

        [Line(1f)]
        public EffectPair FallImpactEffect;

        [Line(1f)]
        public EffectPair BulletHitEffect;

        [Line(1f)]
        public EffectPair SlashEffect;

        [Line(1f)]
        public EffectPair StabEffect;
    }
}