using UnityEngine;

namespace IdenticalStudios.ProceduralMotion
{
    [DisallowMultipleComponent]
    [AddComponentMenu("IdenticalStudios/Motion/Camera Recoil Motion")]
    public sealed class CameraRecoilMotionModule : CharacterBehaviour, IMixedMotion
    {
        public IMotionMixer MotionMixer { get; private set; }

        [Title("Settings")]

        [SerializeField]
        private SpringSettings m_RecoilSpring = SpringSettings.Default;

        private ILookHandler m_LookHandler;

        private float m_XControlled;
        private float m_YControlled;
        
        private Vector2 m_TargetRotation;
        private Vector2 m_StartRotation;
        private Vector2 m_CurrentRotation;
        private float m_Interpolation = 1f;
        private float m_InverseDuration;
        private bool m_RecoilActive;
        private bool m_RecoveryActive;

        private SpringSettings m_RecoverySpring;
        private readonly Spring2D m_Spring = new();
        
        
        protected override void OnBehaviourEnabled()
        {
            m_Spring.Adjust(SpringSettings.Default);

            MotionMixer = GetComponent<IMotionMixer>();
            MotionMixer.AddMixedMotion(this);

            GetModule(out m_LookHandler);
            m_LookHandler.PostViewUpdate += OnPostViewUpdate;
        }

        protected override void OnBehaviourDisabled()
        {
            MotionMixer.RemoveMixedMotion(this);
            m_LookHandler.PostViewUpdate -= OnPostViewUpdate;
        }

        public void AddRecoil(Vector2 recoilToAdd, float duration, SpringSettings recoverySpring)
        {
            if (!m_RecoilActive)
            {
                m_RecoverySpring = recoverySpring;
                m_Spring.Adjust(m_RecoilSpring);
            }

            m_StartRotation = m_Spring.Value;
            m_CurrentRotation = m_Spring.Value;

            if (!m_RecoveryActive)
                m_TargetRotation += recoilToAdd;
            else
                m_TargetRotation = m_StartRotation + recoilToAdd;

            m_Interpolation = 0f;
            m_InverseDuration = 1f / Mathf.Max(duration, 0.001f);

            m_RecoveryActive = false;
            m_RecoilActive = true;
        }
        
        private void OnPostViewUpdate()
        {
            if (!m_RecoilActive)
                return;
            
            float deltaTime = Time.deltaTime;
            
            if (!m_RecoveryActive)
            {
                var internalMoveDelta = m_LookHandler.LookDelta;
                if (internalMoveDelta.x > 0f)
                    m_XControlled += internalMoveDelta.x;

                m_YControlled += internalMoveDelta.y;

                if (m_Interpolation >= 1.35f && m_TargetRotation.x < 0f)
                {
                    if (m_XControlled > Mathf.Abs(m_TargetRotation.x))
                        OnRecoilStop();

                    m_RecoveryActive = true;
                    float targetX = Mathf.Clamp(-m_XControlled, m_TargetRotation.x, 0f);
                    float targetY = Mathf.Clamp(-m_YControlled, -Mathf.Abs(m_TargetRotation.y / 2f), Mathf.Abs(m_TargetRotation.y / 2f));

                    m_Spring.Adjust(m_RecoverySpring);
                    m_Spring.SetTargetValue(new Vector2(targetX, targetY));
                }
                else
                {
                    m_Interpolation = Mathf.Min(m_Interpolation + (deltaTime * m_InverseDuration), 2f);
                    float interpolation = Mathf.Clamp01(m_Interpolation);
                    m_CurrentRotation = Vector2.Lerp(m_StartRotation, m_TargetRotation, interpolation);
                    m_Spring.SetTargetValue(m_CurrentRotation);
                }
            }
            
            m_LookHandler.SetAdditiveLook(m_Spring.Evaluate(deltaTime));

            if (m_RecoveryActive && m_Spring.IsIdle)
                OnRecoilStop();
        }

        private void OnRecoilStop()
        {
            m_CurrentRotation = Vector2.zero;
            m_TargetRotation = Vector2.zero;
            m_RecoilActive = false;
            m_RecoveryActive = false;
            m_LookHandler.MergeAdditiveLook();
            m_Spring.Reset();
            
            m_XControlled = 0f;
            m_YControlled = 0f;
        }

        #region Ignore
        public float WeightMod { get; set; } = 1f;
        public void TickMotionLogic(float deltaTime) { }
        public Vector3 GetPosition(float deltaTime) => Vector3.zero;
        public Quaternion GetRotation(float deltaTime) => Quaternion.identity;
        #endregion
    }
}
