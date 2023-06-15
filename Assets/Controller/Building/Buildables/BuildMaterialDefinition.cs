using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Building/Build Material Definition", fileName = "(BuildMaterial) ")]
    public sealed class BuildMaterialDefinition : DataDefinition<BuildMaterialDefinition>
    {
        public override string Name
        {
            get => m_BuildMaterialName;
            protected set => m_BuildMaterialName = value;
        }

        public override Sprite Icon => m_Icon;
        public SoundPlayer UseSound => m_UseAudio;

        [SerializeField, NewLabel("Name")]
        private string m_BuildMaterialName;

        [SerializeField]
        private Sprite m_Icon;

        [SerializeField, NewLabel("Sound")]
        private SoundPlayer m_UseAudio;
    }
}
