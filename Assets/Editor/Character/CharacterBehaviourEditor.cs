using UnityEditor;

namespace IdenticalStudios
{
    [CustomEditor(typeof(CharacterBehaviour), true)]
    public class CharacterBehaviourEditor : InterfaceModuleRequirementEditor<ICharacterModule>
    {
        protected override string GetRequiredString() => "Required Modules";
        protected override string GetRequiredTypeName() => "module";
    }
}