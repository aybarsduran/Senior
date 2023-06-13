using IdenticalStudios.BuildingSystem;

namespace IdenticalStudios.UISystem
{
    public class BuildableDefinitionSlotUI : SlotUI<BuildableDefinition>
    {
        public BuildableDefinition BuildableDef => Data;


        public void SetNull() => SetData(null);
        public void SetBuildable(BuildableDefinition buildableDef) => SetData(buildableDef);

        public void SetBuildable(Buildable buildable)
        {
            if (buildable != null)
                SetData(buildable.Definition);
            else
                SetData(null);
        }
    }
}
