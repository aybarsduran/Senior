using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public interface IWheelUI
    {
        bool IsInspecting { get; }

        void StartInspection();
        void EndInspectionAndSelectHighlighted();
        void EndInspection();
        void UpdateSelection(Vector2 input);
    }
}