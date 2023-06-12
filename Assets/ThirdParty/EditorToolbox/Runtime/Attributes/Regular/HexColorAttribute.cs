using System;
using UnityEngine;

namespace UnityEngine
{
    /// <summary>
    /// Draws color picker and sets color hex code.
    /// 
    /// <para>Supported types: <see cref="string"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HexColorAttribute : PropertyAttribute
    { }
}