using System;
using UnityEngine;

namespace IdenticalStudios.MovementSystem
{
    [Serializable]
    public abstract class CharacterMotionState : ICharacterMotionState
    {
        public bool Enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        public abstract MovementStateType StateType { get; }
        public abstract float StepCycleLength { get; }
        public abstract bool ApplyGravity { get; }
        public abstract bool SnapToGround { get; }

        protected IMovementController Controller { get; private set; }
        protected IMovementInputProvider Input { get; private set; }
        protected ICharacterMotor Motor { get; private set; }
        protected ICharacter Character { get; private set; }

        [NonSerialized]
        private bool m_Enabled;


        public void InitializeState(IMovementController controller, IMovementInputProvider input, ICharacterMotor motor, ICharacter character)
        {
            Controller = controller;
            Input = input;
            Motor = motor;
            Character = character;

            OnStateInitialized();

            Enabled = true;
        }

        public void DecommisionState()
        {
            OnStateDecommisioned();

            Controller = null;
            Input = null;
            Motor = null;
            Character = null;

            Enabled = true;
        }

        public virtual bool IsStateValid() => true;

        public virtual void OnStateEnter(MovementStateType prevStateType) { }
        public abstract void UpdateLogic();
        public virtual void OnStateBlocked() { }
        public abstract Vector3 UpdateVelocity(Vector3 currentVelocity, float deltaTime);
        public virtual void OnStateExit() { }

        protected virtual void OnStateDecommisioned() { }
        protected virtual void OnStateInitialized() { }

        public virtual void OnEditorValidate() { }
    }
}