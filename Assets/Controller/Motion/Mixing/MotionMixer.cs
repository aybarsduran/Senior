using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    public class MotionMixer : MonoBehaviour, IMotionMixer
    {
        #region Internal
        private enum MixMode
        {
            None = 0,
            Update = 1,
            LateUpdate = 2,
            FixedUpdate = 3,
            FixedLerpUpdate = 4,
            FixedLerpLateUpdate = 5
        }
        #endregion

        public Transform TargetTransform => m_TargetTransform;
        public Vector3 PivotPosition => m_TargetTransform.localPosition;
        public Quaternion PivotRotation => m_TargetTransform.localRotation;

        [SerializeField, InLineEditor]
        private Transform m_TargetTransform;

        [SerializeField]
        private MixMode m_MixMode = MixMode.FixedLerpUpdate;

        [Title("Offset")]

        [SerializeField]
        private Vector3 m_PivotPosition;

        [SerializeField]
        private Vector3 m_PositionOffset;

        [SerializeField]
        private Vector3 m_RotationOffset;

        private Vector3 m_StartPosition = Vector3.zero;
        private Vector3 m_TargetPosition = Vector3.zero;
        private Quaternion m_StartRotation = Quaternion.identity;
        private Quaternion m_TargetRotation = Quaternion.identity;

        private MixMode m_ActiveMixMode = MixMode.None;
        protected List<IMixedMotion> m_Motions = s_EmptyList;
        protected Dictionary<Type, IMixedMotion> m_MotionsDict;
        protected static readonly List<IMixedMotion> s_EmptyList = new();


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

            if (m_Motions == null)
                m_Motions = new();

            if (m_MotionsDict == null)
                m_MotionsDict = new();

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

        private void OnEnable() => SetMixMode(m_MixMode);
        private void OnDisable() => SetMixMode(MixMode.None);

        private void SetMixMode(MixMode mixMode)
        {
            if (m_ActiveMixMode == mixMode)
                return;

            _RegisterMode(m_ActiveMixMode, false);
            m_ActiveMixMode = mixMode;
            _RegisterMode(m_ActiveMixMode, true);

            void _RegisterMode(MixMode mixMode, bool register)
            {
                switch (mixMode)
                {
                    case MixMode.None:
                        break;
                    case MixMode.Update:
                        if (register) UpdateManager.AddUpdate(UpdateMotionsNonLerp);
                        else UpdateManager.RemoveUpdate(UpdateMotionsNonLerp);
                        break;
                    case MixMode.LateUpdate:
                        if (register) UpdateManager.AddLateUpdate(UpdateMotionsNonLerp);
                        else UpdateManager.RemoveLateUpdate(UpdateMotionsNonLerp);
                        break;
                    case MixMode.FixedUpdate:
                        if (register) UpdateManager.AddFixedUpdate(UpdateMotionsNonLerp);
                        else UpdateManager.RemoveFixedUpdate(UpdateMotionsNonLerp);
                        break;
                    case MixMode.FixedLerpUpdate:
                        if (register)
                        {
                            UpdateManager.AddFixedUpdate(UpdateMotionsLerp);
                            UpdateManager.AddUpdate(UpdateInterpolation);
                        }
                        else
                        {
                            UpdateManager.RemoveFixedUpdate(UpdateMotionsLerp);
                            UpdateManager.RemoveUpdate(UpdateInterpolation);
                        }
                        break;
                    case MixMode.FixedLerpLateUpdate:
                        if (register)
                        {
                            UpdateManager.AddFixedUpdate(UpdateMotionsLerp);
                            UpdateManager.AddLateUpdate(UpdateInterpolation);
                        }
                        else
                        {
                            UpdateManager.RemoveFixedUpdate(UpdateMotionsLerp);
                            UpdateManager.RemoveLateUpdate(UpdateInterpolation);
                        }
                        break;
                }
            }

            void UpdateMotionsNonLerp(float deltaTime) => UpdateMotions(false, deltaTime);
            void UpdateMotionsLerp(float deltaTime) => UpdateMotions(true, deltaTime);
        }

        private void UpdateInterpolation()
        {
            float delta = Time.unscaledTime - Time.fixedUnscaledTime;
            if (delta < Time.fixedUnscaledDeltaTime)
            {
                float t = delta / Time.fixedUnscaledDeltaTime;
                Vector3 targetPosition = Vector3.Lerp(m_StartPosition, m_TargetPosition, t);
                Quaternion targetRotation = Quaternion.Lerp(m_StartRotation, m_TargetRotation, t);
                m_TargetTransform.SetLocalPositionAndRotation(targetPosition, targetRotation);
            }
            else
                m_TargetTransform.SetLocalPositionAndRotation(m_TargetPosition, m_TargetRotation);
        }

        private void UpdateMotions(bool lerp, float deltaTime)
        {
            Vector3 targetPos = m_PivotPosition;
            Quaternion targetRot = Quaternion.identity;

            for (int i = 0; i < m_Motions.Count; i++)
            {
                var motion = m_Motions[i];
                motion.TickMotionLogic(deltaTime);

                targetPos += targetRot * motion.GetPosition(deltaTime);
                targetRot *= motion.GetRotation(deltaTime);
            }

            targetPos = targetPos - (targetRot * m_PivotPosition) + m_PositionOffset;
            targetRot *= Quaternion.Euler(m_RotationOffset);

            if (lerp)
            {
                m_StartPosition = m_TargetPosition;
                m_StartRotation = m_TargetRotation;
                m_TargetPosition = targetPos;
                m_TargetRotation = targetRot;
            }
            else
                m_TargetTransform.SetLocalPositionAndRotation(targetPos, targetRot);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && GetInstanceID() < 0)
                SetMixMode(m_MixMode);
            else if (!Application.isPlaying && m_TargetTransform != null)
                m_TargetTransform.SetLocalPositionAndRotation(m_PositionOffset, Quaternion.Euler(m_RotationOffset));
        }

        private void Reset() => m_TargetTransform = transform;

        private void OnDrawGizmos()
        {
            Color pivotColor = new(0.1f, 1f, 0.1f, 0.5f);
            const float pivotRadius = 0.08f;

            Color prevColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = pivotColor;
            UnityEditor.Handles.SphereHandleCap(0, transform.TransformPoint(m_PivotPosition), Quaternion.identity, pivotRadius, EventType.Repaint);
            UnityEditor.Handles.color = prevColor;
        }
#endif
    }
}