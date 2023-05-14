using IdenticalStudios;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public interface ILookHandler : ICharacterModule
    {
        Vector2 ViewAngle { get; }
        Vector2 LookInput { get; }
        Vector2 LookDelta { get; }

        // This event is triggered after the character's view has been updated
        event UnityAction PostViewUpdate;

        // This method adds an additional look input to the character's view
        void SetAdditiveLook(Vector2 look);
        // This method merges all accumulated look input into the final view
        void MergeAdditiveLook();

        // A method that will be used when the look handler needs input. 
        void SetLookInput(LookHandlerInputDelegate input);
    }

    // A delegate that will be used when the look handler needs input. 
    public delegate Vector2 LookHandlerInputDelegate();
}