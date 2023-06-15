using System;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem.Effects
{
    [Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "Assembly-CSharp")]
    public sealed class UnityEventWieldableEffect : WieldableEffect
    {
        [Serializable]
        public class FloatEvent : UnityEvent<float> { }

        [SerializeField]
        private FloatEvent m_Event;


        public override void OnInitialized(IWieldable wieldable)
        {
        }

        public override void PlayEffect() => m_Event.Invoke(1f);
        public override void PlayEffectDynamically(float value) => m_Event.Invoke(value);
    }
}