using UnityEngine;

namespace IdenticalStudios.ResourceGathering
{
    public interface IGatherable
    {
        GatherableDefinition Definition { get; }

        float Health { get; }
        float MaxHealth { get; }
        float GatherRadius { get; }
        Vector3 GatherOffset { get; }

        DamageResult HandleDamage(float damage, DamageContext context = default);

        #region Monobehaviour
        GameObject gameObject { get; }
        Transform transform { get; }
        #endregion
    }
}