using System.Collections;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios.WieldableSystem
{
    using Random = UnityEngine.Random;

    [RequireComponent(typeof(AudioSource))]
    public class ParabolicStickyProjectile : ParabolicProjectileBase
    {
        #region Internal
		[Serializable]
		public struct TwangSettings
		{
			public bool Enabled;

			public Vector3 MovementPivot;

			[Range(0f, 10f)]
			public float Duration;

			[Range(0f, 500f)]
			public float Range;

			public Vector2 RandomRotation;

			public SoundPlayer Audio;
		}
		#endregion


		[SerializeField]
		private Collider m_Collider;

		[SerializeField]
		private Rigidbody m_Rigidbody;

		[SerializeField]
		private TrailRenderer m_Trail;
		
		[SerializeField]
		private AudioSource m_AudioSource;

		[SerializeField, Range(-10f, 10f)]
		private float m_PenetrationOffset = -0.35f;

		[SerializeField]
		private TwangSettings m_TwangSettings;

		private bool n_HitSurface;

		private Transform m_TwangPivot;
		private Transform m_PenetratedTransform;

		private Vector3 m_PenetrationPositionOffset;
		private Quaternion m_PenetrationRotationOffset;


		protected override void OnLaunched()
		{
			// Enable the trail.
			if (m_Trail != null)
				m_Trail.emitting = true;

			if (m_Rigidbody != null)
				m_Rigidbody.isKinematic = true;

			m_Collider.enabled = false;
			m_Collider.isTrigger = true;

			m_PenetratedTransform = null;
			
			if (m_TwangPivot != null)
				CachedTransform.SetParent(null);
			
			n_HitSurface = false;
		}

		protected override void OnHit(RaycastHit hit)
		{
			// Clean the trail.
			if (m_Trail != null)
				m_Trail.emitting = false;
			
			m_Collider.enabled = true;

			n_HitSurface = true;

			CachedTransform.position = hit.point + CachedTransform.forward * m_PenetrationOffset;

			// Stick the projectile in the object.
			m_PenetratedTransform = hit.collider.transform;
			m_PenetrationPositionOffset = m_PenetratedTransform.InverseTransformPoint(CachedTransform.position);
			m_PenetrationRotationOffset = Quaternion.Inverse(m_PenetratedTransform.rotation) * CachedTransform.rotation;

			MatchWithPenetratedSurface();
			
			// Animate the projectile.
			if (m_TwangSettings.Enabled)
				StartCoroutine(C_DoTwang());
		}

		protected override void Awake()
		{
			base.Awake();
			
			if (m_Trail != null)
				m_Trail.emitting = false;
		}

		protected override void FixedUpdate()
		{
			base.FixedUpdate();

			if (n_HitSurface)
				MatchWithPenetratedSurface();
		}

		private void MatchWithPenetratedSurface()
		{
			// While the penetrated transform is not null this projectile will stick to it.
			if (m_PenetratedTransform != null && m_PenetratedTransform.gameObject.activeSelf)
			{
				var position = m_PenetratedTransform.position + m_PenetratedTransform.TransformVector(m_PenetrationPositionOffset);
				var rotation = m_PenetratedTransform.rotation * m_PenetrationRotationOffset;

				CachedTransform.SetPositionAndRotation(position, rotation);
			}
			else
			{
				m_Collider.isTrigger = false;
				m_Rigidbody.isKinematic = false;

				Destroy(this);
			}
		}

		private IEnumerator C_DoTwang()
		{
			if (m_TwangPivot == null)
				m_TwangPivot = new GameObject("Shafted Projectile Pivot").transform;
			
			var twangPosition = CachedTransform.position + m_TwangSettings.MovementPivot.LocalToWorld(CachedTransform);
			var twangRotation = CachedTransform.rotation;

			m_TwangPivot.SetPositionAndRotation(twangPosition, twangRotation);

			var previousParent = CachedTransform.parent;
			if (previousParent != null)
				m_TwangPivot.SetParent(previousParent, true);

			CachedTransform.SetParent(m_TwangPivot, true);

			float stopTime = Time.time + m_TwangSettings.Duration;
			float range = m_TwangSettings.Range;
			float currentVelocity = 0f;

			m_TwangSettings.Audio.PlayAtPosition(transform.position);

			Quaternion localRotation = m_TwangPivot.localRotation;

			Vector2 randomRotationRange = m_TwangSettings.RandomRotation;

			Quaternion randomRotation = Quaternion.Euler(new Vector2(
				Random.Range(-randomRotationRange.x, randomRotationRange.x), 
				Random.Range(-randomRotationRange.y, randomRotationRange.y)));

			while (Time.time < stopTime)
			{
				m_TwangPivot.localRotation = localRotation * randomRotation * Quaternion.Euler(Random.Range(-range, range), Random.Range(-range, range), 0f);
				range = Mathf.SmoothDamp(range, 0f, ref currentVelocity, stopTime - Time.time);

				yield return null;
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			if (!m_TwangSettings.Enabled)
				return;
			
			Vector3 twangPivotPosition = transform.position + transform.TransformVector(m_TwangSettings.MovementPivot);

			Gizmos.color = new Color(1f, 0f, 0f, 0.85f);
			Gizmos.DrawSphere(twangPivotPosition, 0.03f);

			Vector3 sceneCamPosition = SceneView.currentDrawingSceneView.camera.transform.position;
			Vector3 sceneCamForward = SceneView.currentDrawingSceneView.camera.transform.forward;

			// Make sure we don't draw the label when not looking at it
			if (Vector3.Dot(sceneCamForward, twangPivotPosition - sceneCamPosition) >= 0f)
				Handles.Label(twangPivotPosition, "Twang Pivot");
		}

        private void OnValidate()
        {
            if (m_Collider == null)
				m_Collider = GetComponent<Collider>();
            
            if (m_Rigidbody == null)
	            m_Rigidbody = GetComponent<Rigidbody>();;

			if (m_Trail == null)
				m_Trail = GetComponentInChildren<TrailRenderer>();

			if (m_AudioSource == null)
				m_AudioSource = GetComponent<AudioSource>();
        }
#endif
    }
}
