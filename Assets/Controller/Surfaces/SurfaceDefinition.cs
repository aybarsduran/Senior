using System;
using UnityEngine;
using IdenticalStudios;
using System.Xml.Linq;

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

        public float VelocityModifier = 1f;

        public float SurfaceFriction = 1f;

        public float PenetrationResistence = 0.3f;

        public EffectPair SoftFootstepEffect;

        public EffectPair HardFootstepEffect;

        public EffectPair FallImpactEffect;

        public EffectPair BulletHitEffect;

        public EffectPair SlashEffect;

        public EffectPair StabEffect;
    }
}