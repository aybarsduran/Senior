using IdenticalStudios.MovementSystem;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    // Handles and controls motion states that are used in moving a character motor.
    public interface IMovementController : ICharacterModule
    {
        MovementStateType ActiveState { get; }

        MovementModifier VelocityModifier { get; }
        MovementModifier AccelerationModifier { get; }
        MovementModifier DecelerationModifier { get; }

        float StepCycle { get; }

        event UnityAction StepCycleEnded;
        event UnityAction StateChanged;
       // event UnityAction<MovementPreset, MovementPreset> PresetChanged;


        void ResetController();
        //void SetMovementPreset(MovementPreset preset);

        void AddStateEnterListener(MovementStateType stateType, UnityAction callback);
        void AddStateExitListener(MovementStateType stateType, UnityAction callback);

        void RemoveStateEnterListener(MovementStateType stateType, UnityAction callback);
        void RemoveStateExitListener(MovementStateType stateType, UnityAction callback);

        void AddStateLocker(Object locker, MovementStateType stateType);
        void RemoveStateLocker(Object locker, MovementStateType stateType);

        // Transition to the given state.
        bool TrySetState(ICharacterMotionState state);

        // Transition to a state of the given type.
        bool TrySetState(MovementStateType stateType);

        ICharacterMotionState GetStateOfMotionType(MovementStateType stateType);
        T GetStateOfType<T>() where T : ICharacterMotionState;
    }
}