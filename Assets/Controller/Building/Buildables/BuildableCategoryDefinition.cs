using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Building/Buildable Category", fileName = "(BuildableCategory) ")]
    public sealed class BuildableCategoryDefinition : GroupDefinition<BuildableCategoryDefinition, BuildableDefinition>
    {
        public override Sprite Icon => m_Icon;

        [SerializeField]
        private Sprite m_Icon;
    }
}
