using System;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.MovementSystem
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class CharacterControllerMotor : MonoBehaviour, ICharacterMotor, ISaveableComponent
    {
        public float LastGroundedChangeTime => m_LastGroundedChangeTime;
        public float Gravity => m_Gravity;
        public Vector3 Velocity => m_Velocity;
        public Vector3 SimulatedVelocity => m_SimulatedVelocity;
        public Vector3 GroundNormal => m_GroundNormal;
        public float TurnSpeed => m_TurnSpeed;
        public CollisionFlags CollisionFlags => m_CollisionFlags;
        public float GroundSurfaceAngle => Vector3.Angle(Vector3.up, m_GroundNormal);
        public float DefaultHeight => m_DefaultHeight;
        public float SlopeLimit => m_CController.slopeLimit;
        public float Radius => m_CController.radius;
        public bool IsGrounded => m_IsGrounded;
        public float Height
        {
            get => m_CController.height;
            private set
            {
                if (Mathf.Abs(m_CController.height - value) < 0.01f)
                    return;
                
                m_CController.height = value;
                m_CController.center = Vector3.up * (value * 0.5f);
                HeightChanged?.Invoke(value);
            }
        }

        public event UnityAction Teleported;
        public event UnityAction<bool> GroundedChanged;
        public event UnityAction<float> FallImpact;
        public event UnityAction<float> HeightChanged;

        [SerializeField, Range(0f, 100f)]
        [Tooltip("The strength of the gravity.")]
        private float m_Gravity = 20f;

        [SerializeField, Range(20f, 160f)]
        [Tooltip("The mass of this character.")]
        private float m_Mass;

        [Title("Sliding")]

        [SerializeField, Range(20f, 90f)]
        [Tooltip("The angle at which the character will start to slide.")]
        private float m_SlideThreshold = 32f;

        [SerializeField, Range(0f, 2f)]
        [Tooltip("The max sliding speed.")]
        private float m_SlideSpeed = 0.1f;

        [SerializeField]
        [Tooltip("Lowers/Increases the moving speed of the character when moving on sloped surfaces (i.e. lower speed when walking up a hill).")]
        private AnimationCurve m_SlopeSpeedMod;

        [Title("Misc")]

        [Tooltip("A force that will be applied to any rigidbody this motor will collide with.")]
        [SerializeField, Range(0f, 10f)]
        private float m_PushForce = 1f;

        [SerializeField, Range(0.5f, 10f)]
        private float m_DefaultHeight = 1.8f;
        
        [SerializeField]
        [Tooltip("Layers that are considered obstacles.")]
        private LayerMask m_CollisionMask;

        private CharacterController m_CController;
        private MotionInputCallback m_MotionInput;

        private Transform m_CachedTransform;
        private CollisionFlags m_CollisionFlags;
        private Vector3 m_GroundNormal = Vector3.up;
        private Vector3 m_SlideVelocity = Vector3.zero;
        private Vector3 m_Velocity = Vector3.zero;
        private Vector3 m_SimulatedVelocity = Vector3.zero;
        private Vector3 m_ExternalForce = Vector3.zero;
        private float m_LastGroundedChangeTime;
        private bool m_IsGrounded = true;
        private bool m_ApplyGravity;
        private bool m_SnapToGround;
        private bool m_DisableSnapToGround;
        private float m_TurnSpeed;
        private float m_LastYRotation;
        private float m_DefaultStepOffset;

        private RaycastHit m_RaycastHit;
        
        
        private void Awake()
        {
            m_CachedTransform = transform;
            m_CController = GetComponent<CharacterController>();
            m_DefaultHeight = m_CController.height;
            m_DefaultStepOffset = m_CController.stepOffset;

            m_LastYRotation = transform.localEulerAngles.y;

            SetHeight(m_DefaultHeight);
        }

        private void Update()
        {
            if (!m_CController.enabled || m_MotionInput == null)
                return;

            var deltaTime = Time.deltaTime;
            var wasGrounded = m_IsGrounded;
            var groundingForce = 0f;

            m_SimulatedVelocity = m_MotionInput.Invoke(m_SimulatedVelocity, out m_ApplyGravity, out m_SnapToGround);

            if (!m_DisableSnapToGround && wasGrounded && m_SnapToGround)
            {
                // Sliding...
                m_SimulatedVelocity += GetSlidingVelocty(deltaTime);

                // Grounding force...
                groundingForce = GetGroundingTranslation();
            }

            // Apply a gravity force...
            if (m_ApplyGravity)
                m_SimulatedVelocity.y -= m_Gravity * deltaTime;

            // Apply the external force...
            if (m_ExternalForce != Vector3.zero)
            {
                m_SimulatedVelocity += m_ExternalForce;
                m_DisableSnapToGround = false;
                m_ExternalForce = Vector3.zero;
            }

            // Move the controller...
            Vector3 translation = m_SimulatedVelocity * deltaTime;
            translation.y -= groundingForce;
            m_CollisionFlags = m_CController.Move(translation);

            m_Velocity = m_CController.velocity;
            m_IsGrounded = m_CController.isGrounded;

            if (wasGrounded != m_IsGrounded)
            {
                GroundedChanged?.Invoke(m_IsGrounded);
                m_LastGroundedChangeTime = Time.time;

                // Raise fall impact event...
                if (!wasGrounded && m_IsGrounded)
                    FallImpact?.Invoke(Mathf.Abs(m_SimulatedVelocity.y));
            }

            // Calculate turn velocity...
            float currentYRot = m_CachedTransform.localEulerAngles.y;
            m_TurnSpeed = Mathf.Abs(currentYRot - m_LastYRotation);
            m_LastYRotation = currentYRot;
        }

        private float GetGroundingTranslation()
        {
            // Predict next world position
            float distanceToGround = 0.001f;

            var ray = new Ray(m_CachedTransform.position, Vector3.down);
            if (PhysicsUtils.RaycastNonAlloc(ray, m_DefaultStepOffset, out m_RaycastHit, m_CollisionMask, null, QueryTriggerInteraction.Ignore))
                distanceToGround = m_RaycastHit.distance;
            else
                m_ApplyGravity = true;

            return distanceToGround;
        }

        private Vector3 GetSlidingVelocty(float deltaTime) 
        {
            if (GroundSurfaceAngle > m_SlideThreshold)
            {
                Vector3 slideDirection = m_GroundNormal + Vector3.down;
                m_SlideVelocity += slideDirection * (m_SlideSpeed * deltaTime);
            }
            else
                m_SlideVelocity = Vector3.Lerp(m_SlideVelocity, Vector3.zero, deltaTime * 10f);

            return m_SlideVelocity;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            m_GroundNormal = hit.normal;

            // make sure we hit a non kinematic rigidbody
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;

            // make sure we only push desired layer(s)
            var bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & m_CollisionMask.value) == 0) return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f) return;

            // Calculate push direction from move direction, horizontal motion only
            Vector3 pushDir = new(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

            // Apply the push and take strength into account
            body.AddForce(pushDir * m_PushForce, ForceMode.Impulse);
        }

        public void SetMotionInput(MotionInputCallback motionInput) => m_MotionInput = motionInput;

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            m_CController.enabled = false;

            rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
            position = new Vector3(position.x, position.y + m_CController.skinWidth, position.z);
            m_CachedTransform.SetPositionAndRotation(position, rotation);

            m_SlideVelocity = Vector3.zero;
            m_SimulatedVelocity = Vector3.zero;

            m_CController.enabled = true;

            Teleported?.Invoke();
        }

        public void AddForce(Vector3 force, ForceMode mode, bool snapToGround = false)
        {
            switch (mode)
            {
                case ForceMode.Force:
                    m_ExternalForce += force * (1f / m_Mass);
                    break;
                case ForceMode.Acceleration:
                    m_ExternalForce += force * Time.deltaTime;
                    break;
                case ForceMode.Impulse:
                    m_ExternalForce += force * (1f / m_Mass);
                    break;
                case ForceMode.VelocityChange:
                    m_ExternalForce += force;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            m_DisableSnapToGround = !snapToGround;
        }

        public float GetSlopeSpeedMultiplier()
        {
            if (!m_IsGrounded)
                return 1f;

            // Make sure to lower the speed when ascending steep surfaces.
            float surfaceAngle = GroundSurfaceAngle;
            if (surfaceAngle > 5f)
            {
                bool isAscendingSlope = Vector3.Dot(m_GroundNormal, m_SimulatedVelocity) < 0f;

                if (isAscendingSlope)
                    return m_SlopeSpeedMod.Evaluate(surfaceAngle / SlopeLimit);
            }

            return 1f;
        }


        public bool TrySetHeight(float targetHeight)
        {
            bool canSetHeight = true;

            if (Height < targetHeight)
                canSetHeight = !DoCollisionCheck(true, targetHeight - Height);

            if (canSetHeight)
            {
                Height = targetHeight;

                return true;
            }
 
            return false;
        }

        public bool CanSetHeight(float targetHeight)
        {
            if (Mathf.Abs(Height - targetHeight) < 0.01f)
                return true;

            if (Height < targetHeight)
                return !DoCollisionCheck(true, targetHeight - Height + 0.1f);

            return true;
        }

        public void SetHeight(float height) => Height = height;

        public bool Raycast(Ray ray, float distance) 
        {
            if (Physics.Raycast(ray, out m_RaycastHit, distance, m_CollisionMask, QueryTriggerInteraction.Ignore))
                return true;

            return false;
        }

        public bool Raycast(Ray ray, float distance, out RaycastHit raycastHit)
        {
            if (Physics.Raycast(ray, out raycastHit, distance, m_CollisionMask, QueryTriggerInteraction.Ignore))
                return true;

            return false;
        }

        public bool SphereCast(Ray ray, float distance, float radius)
        {
            if (Physics.SphereCast(ray, radius, distance, m_CollisionMask, QueryTriggerInteraction.Ignore))
                return true;

            return false;
        }

        private bool DoCollisionCheck(bool checkAbove, float maxDistance)
        {
            Vector3 rayOrigin = m_CachedTransform.position + (checkAbove ? Vector3.up * m_CController.height / 2 : Vector3.up * m_CController.height);
            Vector3 rayDirection = checkAbove ? Vector3.up : Vector3.down;

            return Physics.SphereCast(new Ray(rayOrigin, rayDirection), m_CController.radius, maxDistance, m_CollisionMask, QueryTriggerInteraction.Ignore);
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_SimulatedVelocity = (Vector3)members[0];
            m_CController.height = 0f;
            Height = (float)members[1];

            Teleport((Vector3)members[2], (Quaternion)members[3]);
        }

        public object[] SaveMembers()
        {
            return new object[] 
            {
                m_SimulatedVelocity,
                Height,
                transform.position,
                transform.rotation
            };
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_CController == null)
                return;

            m_DefaultHeight = m_CController.height;
            m_DefaultStepOffset = m_CController.stepOffset;
        }
#endif
        #endregion
    }
}