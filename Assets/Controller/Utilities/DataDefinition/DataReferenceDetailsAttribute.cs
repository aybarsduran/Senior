using System;
using UnityEngine;

namespace IdenticalStudios
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DataReferenceDetailsAttribute : PropertyAttribute 
    {
        public bool HasAssetReference { get; set; } = false;
        public bool HasLabel { get; set; } = true;
        public bool HasIcon { get; set; } = true;
        public bool HasNullElement { get; set; } = true;
        public string NullElementName { get; set; } = "Empty";
    }
}