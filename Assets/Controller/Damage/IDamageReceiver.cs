
namespace IdenticalStudios
{
    public interface IDamageReceiver
    {
        DamageResult HandleDamage(float damage, DamageContext context = default);
    }

    public enum DamageResult
    {
        Default,
        Critical,
        Blocked,
        Reflected,
        Ignored
    }
}