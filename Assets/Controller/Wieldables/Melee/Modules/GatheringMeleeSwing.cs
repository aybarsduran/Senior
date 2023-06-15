using IdenticalStudios.ResourceGathering;
using IdenticalStudios.UISystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Melee/Gathering Swing")]
	public class GatheringMeleeSwing : BasicMeleeSwing
	{
		[Title("Gathering Swing")]
		
		[SerializeField, ReorderableList(HasLabels = false)]
		private GatherableDefinition[] m_ValidGatherables;
		
		[SerializeField, Range(0f, 10f)]
		private float m_MaxGatherDistance = 0.35f;
		
		/// <summary>
		/// TODO: REMOVE
		/// </summary>
		[SerializeField]
		private bool m_ShowTreeHealthIndicator;
		
		
		public override bool GetSwingValidity(float accuracy)
		{
			var ray = GetUseRay(accuracy);
			if (PhysicsUtils.SphereCastNonAlloc(ray, m_HitRadius, m_HitDistance, out RaycastHit hitInfo, m_HitMask, Wieldable.Character.Colliders, m_HitTriggers))
			{
				IGatherable gatherable = hitInfo.collider.GetComponent<IGatherable>();

				bool canSwing = gatherable != null &&
								IsValidGatherable(gatherable) &&
								CheckDistanceWithGatherable(gatherable, ray);

				return canSwing;
			}

			return false;
		}
		
		protected override void OnHit(Ray ray)
		{
			base.OnHit(ray);
			
			IGatherable gatherable = HitInfo.collider.GetComponent<IGatherable>();

			bool isValidGatherable = gatherable != null &&
									 IsValidGatherable(gatherable) &&
									 CheckDistanceWithGatherable(gatherable, ray);

			if (isValidGatherable)
			{
				DamageContext dmgContext = new(ray.origin + ray.direction * 0.5f + Vector3.Cross(Vector3.up, ray.direction) * 0.25f, ray.direction * m_Force, m_HitInfo.normal, Wieldable.Character);
				gatherable.HandleDamage(m_Damage, dmgContext);
			}
		}

        private void OnEnable()
        {
			if (m_ShowTreeHealthIndicator)
				TreeHealthIndicatorUI.ShowIndicator(m_ValidGatherables);
		}

		private void OnDisable()
        {
			if (m_ShowTreeHealthIndicator)
				TreeHealthIndicatorUI.HideIndicator();
		}

        private bool IsValidGatherable(IGatherable gatherable)
		{
			GatherableDefinition gatherDefinition = gatherable.Definition;

			for (int i = 0; i < m_ValidGatherables.Length; i++)
			{
				if (gatherDefinition == m_ValidGatherables[i])
					return true;
			}

			return false;
		}
		
		private bool CheckDistanceWithGatherable(IGatherable gatherable, Ray ray)
		{
			if (gatherable != null)
			{
				var tree = gatherable.transform;
				return Mathf.Abs((ray.origin + ray.direction).y - (tree.position + gatherable.GatherOffset).y) < m_MaxGatherDistance;
			}

			return false;
		}
	}
}