namespace IdenticalStudios.WieldableSystem
{
    public sealed class WieldableParticleVFX : WieldableVFXBehaviour<ParticleSystemCollection>
    {
        protected override ParticleSystemCollection SpawnEffect()
        {
			var effect = base.SpawnEffect();
			effect.Play();
			return effect;
        }
    }
}
