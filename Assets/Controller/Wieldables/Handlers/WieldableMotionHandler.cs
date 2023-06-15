using IdenticalStudios.ProceduralMotion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [RequireComponent(typeof(MotionDataHandler))]
    public sealed class WieldableMotionHandler : CharacterBehaviour, IMotionMixer, IWieldableMotionHandler
    {
        public IMotionMixer MotionMixer => this;
        public IMotionDataHandler DataHandler => m_DataHandler;

        public Transform TargetTransform => m_CurrentMixer != null ? m_CurrentMixer.TargetTransform : transform;
        public Vector3 PivotPosition => m_CurrentMixer != null ? m_CurrentMixer.PivotPosition : Vector3.zero;
        public Quaternion PivotRotation => m_CurrentMixer != null ? m_CurrentMixer.PivotRotation : Quaternion.identity;

        private FPWieldableMotionMixer m_CurrentMixer;
        private MotionDataHandler m_DataHandler;
        private IWieldable m_CurrentWieldable;

        private readonly List<IMixedMotion> m_Motions = new(16);
        private readonly Dictionary<Type, IMixedMotion> m_MotionsDict = new(32);
        private readonly Dictionary<IWieldable, FPWieldableMotionMixer> m_Mixers = new(16);


        public bool TryGetMotionOfType<T>(out T motion) where T : class, IMixedMotion
        {
            if (m_MotionsDict.TryGetValue(typeof(T), out var mixedMotion))
            {
                motion = (T)mixedMotion;
                return true;
            }

            motion = null;
            return false;
        }

        public IMixedMotion GetMotionOfType(Type type)
        {
            if (m_MotionsDict.TryGetValue(type, out var mixedMotion))
                return mixedMotion;
            else
                Debug.LogError($"No motion of type ''{type.Name}'' found, use ''{nameof(TryGetMotionOfType)}'' instead if the expected motion can be null.");

            return null;
        }

        public T GetMotionOfType<T>() where T : class, IMixedMotion
        {
            if (m_MotionsDict.TryGetValue(typeof(T), out var mixedMotion))
                return (T)mixedMotion;

            return null;
        }

        public void AddMixedMotion(IMixedMotion mixedMotion)
        {
            if (mixedMotion == null)
                return;

            var motionType = mixedMotion.GetType();
            if (!m_MotionsDict.ContainsKey(motionType))
            {
                m_MotionsDict.Add(motionType, mixedMotion);
                m_Motions.Add(mixedMotion);
            }
        }

        public void RemoveMixedMotion(IMixedMotion mixedMotion)
        {
            if (mixedMotion == null)
                return;

            if (m_MotionsDict.Remove(mixedMotion.GetType()))
                m_Motions.Remove(mixedMotion);
        }

        protected override void OnBehaviourEnabled()
        {
            m_DataHandler = GetComponent<MotionDataHandler>();

            var controller = GetModule<IWieldablesController>();
            controller.WieldableEquipStarted += OnWieldableChanged;

            if (controller.ActiveWieldable != null)
                OnWieldableChanged(controller.ActiveWieldable);

#if UNITY_EDITOR
            // Hack...
            FirearmAttachmentBehaviour.EffectCollectionsRefreshed += m_DataHandler.ForceRefreshData;
#endif
        }

        protected override void OnBehaviourDisabled()
        {
            GetModule<IWieldablesController>().WieldableEquipStarted -= OnWieldableChanged;
            OnWieldableChanged(null);

#if UNITY_EDITOR
            // Hack...
            FirearmAttachmentBehaviour.EffectCollectionsRefreshed -= m_DataHandler.ForceRefreshData;
#endif
        }

        private void OnWieldableChanged(IWieldable equippedWieldable)
        {
            if (m_CurrentWieldable == equippedWieldable)
                return;

            m_CurrentWieldable = equippedWieldable;
            m_CurrentMixer = GetMixer(equippedWieldable);

            // Initialize the new mixer (equipped wieldable).
            if (m_CurrentMixer != null)
            {
                m_CurrentMixer.DataHandler = m_DataHandler;
                m_DataHandler.SetPreset(m_CurrentMixer.MotionPreset);
            }
            else
                m_DataHandler.SetPreset(m_DataHandler.DefaultPreset);
        }

        private FPWieldableMotionMixer GetMixer(IWieldable wieldable)
        {
            if (wieldable == null)
                return null;

            if (m_Mixers.TryGetValue(wieldable, out var pair))
                return pair;
            
            var mixer = wieldable.gameObject.GetComponentInFirstChildren<FPWieldableMotionMixer>();
            mixer.SetMotions(m_Motions, m_MotionsDict);

            m_Mixers.Add(wieldable, mixer);

            return mixer;
        }
    }
}