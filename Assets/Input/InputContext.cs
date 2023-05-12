using UnityEngine;
using System;

namespace IdenticalStudios.InputSystem
{
    [CreateAssetMenu(menuName = "Identical Studios/Input/Input Context", fileName = "(InputContext) ")]
    public sealed class InputContext : InputContextBase
    {
        internal override InputContextBase[] GetSubContexts() => Array.Empty<InputContextBase>();
    }
}
