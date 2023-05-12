

using UnityEngine; // Import the UnityEngine namespace
using UnityEngine.Events; // Import the UnityEvent class

namespace IdenticalStudios // Define the namespace for the interface
{
    public interface ICharacter // Define the ICharacter interface
    {
        // Define a boolean property that indicates whether the character has been initialized or not
        bool IsInitialized { get; }

        // Define a Transform property that represents the character's view transform in the game world
        Transform ViewTransform { get; }

        // Define a Collider[] property that represents an array of colliders that the character has
        Collider[] Colliders { get; }


       // Define an event that is triggered when the character is initialized
        event UnityAction Initialized;

        

        // Define a method that checks if the character has a specific collider
        bool HasCollider(Collider collider);

        
        // Define the gameObject property, which is part of the MonoBehaviour class
        GameObject gameObject { get; }

        // Define the transform property, which is part of the MonoBehaviour class
        Transform transform { get; }
       
    }
}
