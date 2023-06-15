using UnityEditor;

namespace IdenticalStudios
{
    [CustomEditor(typeof(Interactable), true)]
    public class InteractableEditor : FoldoutBaseTypeEditor<Interactable> 
    {
        protected override bool DrawScriptProperty => false;
    }
}
