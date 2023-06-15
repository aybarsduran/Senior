using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    public interface ICharacterMotionState
    {
        MovementStateType StateType { get; }
        float StepCycleLength { get; }
        bool ApplyGravity { get; }
        bool SnapToGround { get; }
        bool Enabled { get; set; }


        /// <summary>
        /// Initializes/enables this state.
        /// </summary>
        void InitializeState(IMovementController movement, IMovementInputProvider input, ICharacterMotor motor, ICharacter character);

        /// <summary>
        /// Decommisions/disables this state.
        /// </summary>
        void DecommisionState();

        /// <summary>
        /// Can this state be transitioned to.
        /// </summary>
        bool IsStateValid();

        /// <summary>
        /// Called when entering this state.
        /// </summary>
        void OnStateEnter(MovementStateType prevStateType);

        /// <summary>
        /// Updates this state's logic, it also handles transitions.
        /// </summary>
        void UpdateLogic();

        /// <summary>
        /// Passes the current velocity and returns a new one that will be used to move the parent character.
        /// </summary>
        /// <returns> New velocity </returns>
        Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime);

        /// <summary>
        /// Called when exiting this state.
        /// </summary>
        void OnStateExit();
    }
}