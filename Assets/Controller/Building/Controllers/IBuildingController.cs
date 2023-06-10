using IdenticalStudios.BuildingSystem;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public enum BuildingMode { Free, Socket }

    public interface IBuildingController : ICharacterModule
    {
        bool IsBuildingActive { get; }
        BuildingMode Mode { get; }
        BuildableDefinition Buildable { get; }
        float RotationOffset { get; set; }

        event UnityAction BuildingStarted;
        event UnityAction BuildingStopped;
        event UnityAction<BuildableDefinition> BuildableChanged;
        event UnityAction ObjectPlaced;

        void SetBuildable(BuildableDefinition buildable);
        void SelectNextBuildable(bool next);

        void PlaceBuildable();
    }
}