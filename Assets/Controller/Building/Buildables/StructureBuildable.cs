using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    public class StructureBuildable : Buildable
    {
        public StructureManager ParentStructure { get; set; }

        public DataIdReference<BuildableCategoryDefinition> RequiredSpace => m_RequiredSpace;
        public DataIdReference<BuildableCategoryDefinition>[] SpacesToOccupy => m_SpacesToOccupy;

		public bool RequiresSockets => m_RequiresSockets;
		public Socket[] Sockets => m_Sockets;
        public Vector3 OccupiedSocketPosition 
		{
			get => m_OccupiedSocketPosition;
			set => m_OccupiedSocketPosition = value;
		}

		[Title("Settings (Sockets)")]

		[SerializeField]
		private bool m_RequiresSockets = true;

		[SerializeField, DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<BuildableCategoryDefinition> m_RequiredSpace;

		[SpaceArea]

		[SerializeField]
		[DataReferenceDetails(HasNullElement = false)]
        [ReorderableList(HasLabels = false, Foldable = true)]
        private DataIdReference<BuildableCategoryDefinition>[] m_SpacesToOccupy;

		private Vector3 m_OccupiedSocketPosition;
        private Socket[] m_Sockets;


		public override void OnCreated(bool playEffects = true)
		{
			EnableColliders(false);
			EnableSockets(false);

			gameObject.SetLayerRecursively(BuildingManager.BuildablePreviewLayer);
			MaterialEffect.EnableCustomEffect(BuildingManager.PlacementAllowedMaterialEffect);
		}

		public override void OnPlaced(bool playEffects = true)
		{
			EnableSockets(true);

			if (playEffects)
				DoPlacementEffects();

			EnableColliders(true);
		}

		public override void OnBuilt(bool playEffects = true)
        {
			gameObject.SetLayerRecursively(BuildingManager.BuildableLayer);
			MaterialEffect.DisableActiveEffect();

			EnableSockets(true);

			if (playEffects)
				DoBuildEffects();
		}

		protected void EnableSockets(bool enable)
		{
			foreach (var socket in m_Sockets)
				socket.gameObject.SetActive(enable);
		}

		protected override void Awake()
		{
			base.Awake();

			m_Sockets = gameObject.GetComponentsInFirstChildren<Socket>().ToArray();
		}
	}
}
