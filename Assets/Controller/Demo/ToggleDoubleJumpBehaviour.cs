using IdenticalStudios.MovementSystem;
using UnityEngine;

namespace IdenticalStudios.Demo
{
    public class ToggleDoubleJumpBehaviour : MonoBehaviour
    {
        public void ToggleDoubleJump(ICharacter character) 
        {
            if (character.TryGetModule(out IMovementController movement))
            {
                var jumpState = movement.GetStateOfType<CharacterJumpState>();

                if (jumpState != null)
                    jumpState.MaxJumpsCount = jumpState.MaxJumpsCount == jumpState.DefaultJumpsCount ? 2 : jumpState.DefaultJumpsCount;
            }
        }
    }
}