using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface IInventoryInspectManager : ICharacterModule
    {
        bool IsInspecting { get; }
        IWorkstation Workstation { get; }

        event UnityAction BeforeInspectionStarted;
        event UnityAction AfterInspectionStarted;
        event UnityAction BeforeInspectionEnded;
        event UnityAction AfterInspectionEnded;


        /// <summary>
        /// Workstation: Null for default inspection.
        /// </summary>
        void StartInspection(IWorkstation workstation, UnityAction inspectionStartCallback = null, UnityAction inspectionEndCallback = null);
        void StopInspection();
    }
}