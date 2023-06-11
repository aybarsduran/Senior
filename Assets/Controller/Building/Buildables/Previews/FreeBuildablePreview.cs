using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    public class FreeBuildablePreview : BuildablePreview
    {
        private FreeBuildable m_Buildable;


        public override void CancelPreview() => Destroy(gameObject);
        private void Awake() => m_Buildable = GetComponent<FreeBuildable>();

        protected override void OnBuildableBuilt(Buildable buildable) => buildable.OnBuilt();
        protected override void OnAllBuildablesBuilt() => Destroy(this);
        protected override Vector3 GetPreviewCenter() => m_Buildable.transform.position;

        protected override bool CanAddBuildMaterial()
        {
            // Check if there's any characters inside this preview.
            int colliderCount = Physics.OverlapBoxNonAlloc(
                m_Buildable.Bounds.center,
                m_Buildable.Bounds.extents,
                s_CollisionResults,
                m_Buildable.transform.rotation,
                BuildingManager.CharacterLayers,
                QueryTriggerInteraction.Ignore);

            return colliderCount == 0;
        }

        #region Save & Load
        public override void LoadMembers(object[] members)
        {
            buildableRequirements = new()
            {
                [m_Buildable] = (BuildRequirement[])members[0]
            };

            m_Buildable.OnCreated(false);
            m_Buildable.OnPlaced(false);

            EnablePreview();
            CalculateCenter();
        }

        public override object[] SaveMembers()
        {
            var buildReq = buildableRequirements[m_Buildable];

            object[] members = new object[]
            {
                buildReq
            };

            return members;
        }
        #endregion
    }
}