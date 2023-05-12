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

        event UnityAction PostViewUpdate;

        void SetAdditiveLook(Vector2 look);
        void MergeAdditiveLook();

        
        // A method that will be used when the look handler needs input. 
        void SetLookInput(LookHandlerInputDelegate input);
    }

   
    // A delegate that will be used when the look handler needs input. 
    public delegate Vector2 LookHandlerInputDelegate();
}