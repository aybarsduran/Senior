using IdenticalStudios.MovementSystem;
using IdenticalStudios;
using UnityEngine;

namespace PolymindGames.MovementSystem
{
    public interface ICharacterMotionState
    {
        MovementStateType StateType { get; }
        float StepCycleLength { get; }
        bool ApplyGravity { get; }
        bool SnapToGround { get; }
        bool Enabled { get; set; }


        
        // Initializes/enables this state.
        void InitializeState(IMovementController movement, IMovementInputProvider input, ICharacterMotor motor, ICharacter character);

    
        // Decommisions/disables this state.
        void DecommisionState();

       
        // Can this state be transitioned to.
        bool IsStateValid();

        /// Called when entering this state.
        void OnStateEnter(MovementStateType prevStateType);

        
        // Updates this state's logic, it also handles transitions.
        void UpdateLogic();

       
        // Passes the current velocity and returns a new one that will be used to move the parent character.
        // returns New velocity </returns>
        Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime);

        
        // Called when exiting this state.
        void OnStateExit();
    }
}