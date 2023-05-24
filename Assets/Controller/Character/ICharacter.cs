using UnityEngine; 
using UnityEngine.Events; 

namespace IdenticalStudios 
{
    public interface ICharacter 
    {
        // Define a boolean property that indicates whether the character has been initialized or not
        bool IsInitialized { get; }

        // Define a Transform property that represents the character's view transform in the game world
        Transform ViewTransform { get; }

        // Define a Collider[] property that represents an array of colliders that the character has
        Collider[] Colliders { get; }

        IAudioPlayer AudioPlayer { get; }

        // The character's health manager module
        IHealthManager HealthManager { get; }


        // Define an event that is triggered when the character is initialized
        event UnityAction Initialized;

        bool TryGetModule<T>(out T module) where T : class, ICharacterModule;
        void GetModule<T>(out T module) where T : class, ICharacterModule;
        T GetModule<T>() where T : class, ICharacterModule;

        // Define a method that checks if the character has a specific collider
        bool HasCollider(Collider collider);

        
        // Define the gameObject property, which is part of the MonoBehaviour class
        GameObject gameObject { get; }

        // Define the transform property, which is part of the MonoBehaviour class
        Transform transform { get; }
       
    }
}
