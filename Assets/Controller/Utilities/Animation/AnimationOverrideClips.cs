using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios
{
    [Serializable]
    public sealed class AnimationOverrideClips
    {
        [Serializable]
        public struct AnimationClipPair
        {
            public AnimationClip Original;
            public AnimationClip Override;
        }

        public RuntimeAnimatorController Controller { get => m_Controller; set => m_Controller = value; }
        public AnimationClipPair[] Clips => m_Clips;

        [SerializeField]
        private RuntimeAnimatorController m_Controller;

        [SerializeField]
        private AnimationClipPair[] m_Clips;
    }

    public static class AnimationOverrideClipsExtensions
    {
        public static void SetClips(this Animator animator, AnimationOverrideClips overrideClips)
        {
            if (overrideClips == null)
                return;

            var overrideController = new AnimatorOverrideController(overrideClips.Controller);
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            var clips = overrideClips.Clips;
            for (int i = 0; i < clips.Length; i++)
                overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clips[i].Original, clips[i].Override));

            overrideController.ApplyOverrides(overrides);
            animator.runtimeAnimatorController = overrideController;
        }
    }
}