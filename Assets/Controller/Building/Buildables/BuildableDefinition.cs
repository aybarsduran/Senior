using System.Linq;
using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Building/Buildable Definition", fileName = "(Buildable) ")]
    public sealed class BuildableDefinition : GroupMemberDefinition<BuildableDefinition, BuildableCategoryDefinition>
    {
        public override string Name
        {
            get => m_BuildableName;
            protected set => m_BuildableName = value;
        }

        public override Sprite Icon => m_Icon;
        public override string Description => m_Description;
        public Buildable Prefab => m_Prefab;

        public BuildRequirementInfo[] BuildRequirements => m_BuildRequirements;

        [SerializeField]
        private string m_BuildableName;

        [SerializeField, SpritePreview]
        [Tooltip("Item Icon.")]
        private Sprite m_Icon;

        [SerializeField]
        [Tooltip("Corresponding pickup for this item, so you can actually drop it, or pick it up from the ground.")]
        private Buildable m_Prefab;

        [SerializeField, Multiline]
        [Tooltip("Item description to display in the UI.")]
        private string m_Description;


        [SerializeField, ReorderableList]
        private BuildRequirementInfo[] m_BuildRequirements;


        public BuildRequirement[] GetBuildRequirements()
        {
            var buildReq = new BuildRequirement[m_BuildRequirements.Length];

            for (int i = 0; i < m_BuildRequirements.Length; i++)
            {
                var buildableReq = m_BuildRequirements[i];
                buildReq[i] = new BuildRequirement(buildableReq);
            }

            return buildReq;
        }

        public static BuildableDefinition[] GetAllBuildablesOfType<T>() where T : Buildable
        {
            return Definitions.Where((BuildableDefinition def) => def.Prefab.GetType() == typeof(T)).ToArray();
        }

#if UNITY_EDITOR
        public override void Reset()
        {
            base.Reset();

            m_Prefab = null;
        }
#endif
    }
}
