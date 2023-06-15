using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AdditiveForceMotionModule))]
    [AddComponentMenu("IdenticalStudios/Motion/Offset Motion")]
    public sealed class OffsetMotionModule : DataMotionModule<OffsetMotionDataBase>
    {
        #region Internal
        [System.Serializable]
        private struct OffsetTransitionData
        {
            public MovementStateType From;
            public MovementStateType To;
        }
        #endregion

        [SerializeField]
        private AdditiveForceMotionModule m_ForceModule;

        [Title("Settings")]

        [SerializeField]
        private AnimationCurve m_InterpolationCurve;

        [SpaceArea]

        [SerializeField, ReorderableList(childLabel: "From")]
        private OffsetTransitionData[] m_IgnoredTransitions;

        private OffsetMotionDataBase.OffsetData m_StartOffset;
        private OffsetMotionDataBase.OffsetData m_TargetOffset;

        private IMovementController m_Movement;

        private int m_CurrentOffsetIndex;
        private float m_InverseDuration;
        private float m_Interpolation;

        private const float k_RotationForceMod = 3f;
        private const float k_PositionOffsetMod = 0.01f;

        private MovementStateType m_PrevStateType = MovementStateType.Idle;
        private OffsetMotionDataBase m_PrevData;


        protected override void OnBehaviourEnabled()
        {
            base.OnBehaviourEnabled();
            GetModule(out m_Movement);
        }

        protected override OffsetMotionDataBase GetDataFromPreset(IMotionDataHandler dataHandler)
        {
            //var state = m_Movement.ActiveState;
            //if (state == MovementStateType.Airborne || state == MovementStateType.Jump)
            //    return m_PrevData;

            if (dataHandler.TryGetData(out SimpleOffsetMotionData data))
                return data;

            return dataHandler.GetData<OffsetMotionData>();
        }

        protected override void OnDataChanged(OffsetMotionDataBase data)
        {
            //var state = m_Movement.ActiveState;
            //if (state == MovementStateType.Airborne || state == MovementStateType.Jump)
            //    return;

            bool addTransitionForce = data != m_PrevData && CanAddTransitionForce();
            m_PrevStateType = m_Movement.ActiveState;

            if (m_PrevData != null)
            {
                if (addTransitionForce)
                    m_ForceModule.AddRotationForce(m_PrevData.ExitForce, k_RotationForceMod);
            }

            if (data != null)
            {
                positionSpring.Adjust(data.PositionSettings);
                rotationSpring.Adjust(data.RotationSettings);

                if (addTransitionForce)
                    m_ForceModule.AddRotationForce(Data.EnterForce, k_RotationForceMod);

                SetTargetOffset(0);
            }

            m_PrevData = data;
        }

        public override void TickMotionLogic(float deltaTime)
        {
            if (Data == null)
                return;

            m_Interpolation += deltaTime * m_InverseDuration;
            if (m_Interpolation >= 1f)
                SetTargetOffset(m_CurrentOffsetIndex + 1);

            float t = Mathf.Clamp01(m_InterpolationCurve.Evaluate(m_Interpolation));
            Vector3 targetPosition = Vector3.Lerp(m_StartOffset.PositionOffset, m_TargetOffset.PositionOffset, t);
            Vector3 targetRotation = Vector3.Slerp(m_StartOffset.RotationOffset, m_TargetOffset.RotationOffset, t);

            SetTargetPosition(targetPosition, k_PositionOffsetMod);
            SetTargetRotation(targetRotation);
        }

        private bool CanAddTransitionForce()
        {
            for (int i = 0; i < m_IgnoredTransitions.Length; i++)
            {
                var transition = m_IgnoredTransitions[i];

                if (m_Movement.ActiveState == transition.To &&
                    m_PrevStateType == transition.From)
                    return false;
            }

            return true;
        }

        private void SetTargetOffset(int index)
        {
            m_CurrentOffsetIndex = index;

            if (m_CurrentOffsetIndex < Data.Offsets.Length)
            {
                var offset = Data.Offsets[0];
                m_InverseDuration = 1f / Mathf.Max(offset.Duration, 0.001f);
                m_Interpolation = 0f;

                m_StartOffset = Data.Offsets[m_CurrentOffsetIndex];
                m_TargetOffset = m_CurrentOffsetIndex + 1 < Data.Offsets.Length ? Data.Offsets[m_CurrentOffsetIndex + 1] : m_StartOffset;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_ForceModule == null)
                m_ForceModule = GetComponent<AdditiveForceMotionModule>();
        }
#endif
    }
}