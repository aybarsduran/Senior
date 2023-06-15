using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public class PlayerLookHandler : CharacterBehaviour, ILookHandler, ISaveableComponent
    {
        public Vector2 ViewAngle => m_ViewAngle;
        public Vector2 LookInput { get; private set; }
        public Vector2 LookDelta { get; private set; }

        public event UnityAction PostViewUpdate;
        
        [Title("Transforms")]

        [SerializeField]
        [Tooltip("Transform to rotate Up & Down.")]
        private Transform m_XTransform;

        [SerializeField]
        [Tooltip("Transform to rotate Left & Right.")]
        private Transform m_YTransform;

        [Title("Settings")]

        [SerializeField]
        [Tooltip("The up & down rotation will be inverted, if checked.")]
        private bool m_Invert;

        [SerializeField]
        [Tooltip("Vertical look limits (in angles).")]
        private Vector2 m_LookLimits = new Vector2(-60f, 90f);

        [Title("Feel")]

        [SerializeField, Range(0.1f, 10f)]
        [Tooltip("Rotation Speed.")]
        private float m_Sensitivity = 1.5f;

        [SpaceArea]

        [SerializeField]
        [Help("Used in lowering/increasing the current sensitivity based on the FOV", UnityMessageType.None)]
        private Camera m_FOVCamera;

        private Vector2 m_ViewAngle;

        private float m_CurrentSensitivity;
        private Vector2 m_AdditiveLook;

        private LookHandlerInputDelegate m_InputDelegate;
        private bool m_HasFOVCamera;


        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_ViewAngle = (Vector2)members[0];
        }

        public object[] SaveMembers()
        {
            return new object[] { m_ViewAngle };
        }
        #endregion

        public void SetAdditiveLook(Vector2 look) => m_AdditiveLook = look;
        
        public void MergeAdditiveLook()
        {
            m_ViewAngle += m_AdditiveLook;
            m_AdditiveLook = Vector2.zero;
        }

        public void SetLookInput(LookHandlerInputDelegate input) => m_InputDelegate = input;

        protected override void OnBehaviourEnabled()
        {
            if (!m_XTransform)
            {
                Debug.LogErrorFormat(this, "Assign the X Transform in the inspector!", name);
                enabled = false;
            }
            else if (!m_YTransform)
            {
                Debug.LogErrorFormat(this, "Assign the Y Transform in the inspector!", name);
                enabled = false;
            }

            m_HasFOVCamera = m_FOVCamera != null;
            
            GetModule<ICharacterMotor>().Teleported += OnTeleport;
        }

        protected override void OnBehaviourDisabled()
        {
            GetModule<ICharacterMotor>().Teleported -= OnTeleport;
        }

        private void OnTeleport() => m_ViewAngle = new(0f, m_YTransform.localEulerAngles.y);

        private void LateUpdate()
        {
            LookInput = Vector2.zero;

            if (m_InputDelegate != null)
                LookInput = m_InputDelegate();

            m_CurrentSensitivity = GetTargetSensitivity(m_CurrentSensitivity, Time.deltaTime * 8f);

            MoveView(LookInput);

            PostViewUpdate?.Invoke();
        }

        private float GetTargetSensitivity(float currentSens, float delta)
        {
            float targetSensitivity = m_Sensitivity;
            targetSensitivity *= m_HasFOVCamera ? m_FOVCamera.fieldOfView / 90f : 1f;

            return Mathf.Lerp(currentSens, targetSensitivity, delta);
        }

        private void MoveView(Vector2 lookInput)
        {
            var prevAngle = m_ViewAngle;
            
            m_ViewAngle.x += lookInput.x * m_CurrentSensitivity * (m_Invert ? 1f : -1f);
            m_ViewAngle.y += lookInput.y * m_CurrentSensitivity;
            
            m_ViewAngle.x = ClampAngle(m_ViewAngle.x, m_LookLimits.x, m_LookLimits.y);
            LookDelta = new Vector2(m_ViewAngle.x - prevAngle.x, m_ViewAngle.y - prevAngle.y);

            var viewAngle = new Vector2()
            {
                x = ClampAngle(m_ViewAngle.x + m_AdditiveLook.x, m_LookLimits.x, m_LookLimits.y),
                y = m_ViewAngle.y + m_AdditiveLook.y
            };
            
            m_YTransform.localRotation = Quaternion.Euler(0f, viewAngle.y, 0f);
            m_XTransform.localRotation = Quaternion.Euler(viewAngle.x, 0f, 0f);
        }
        
        /// <summary>
        /// Clamps the given angle between min and max degrees.
        /// </summary>
        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle > 360f)
                angle -= 360f;
            else if (angle < -360f)
                angle += 360f;

            return Mathf.Clamp(angle, min, max);
        }
    }
}