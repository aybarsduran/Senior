using UnityEngine;

namespace IdenticalStudios.BuildingSystem
{
    public class StructurePreview : BuildablePreview
    {
        private StructureManager m_Structure;


        public override void CancelPreview()
        {
            var buildables = m_Structure.Buildables;

            foreach (var pair in buildableRequirements)
            {
                if (!AreRequirementsMet(pair.Value))
                {
                    m_Structure.RemovePart((StructureBuildable)pair.Key);
                    continue;
                }
            }

            buildableRequirements.Clear();

            if (buildables.Count == 0)
                Destroy(gameObject);
            else
                Destroy(this);
        }

        protected override bool CanAddBuildMaterial()
        {
            // Check if there's any characters inside this structure preview.
            foreach (var buildable in m_Structure.Buildables)
            {
                int colliderCount = Physics.OverlapBoxNonAlloc(
                    buildable.Bounds.center,
                    buildable.Bounds.extents,
                    s_CollisionResults,
                    buildable.transform.rotation,
                    BuildingManager.CharacterLayers,
                    QueryTriggerInteraction.Ignore);

                if (colliderCount > 0)
                    return false;
            }

            return true;
        }

        protected override void OnBuildableBuilt(Buildable buildable) => buildable.OnBuilt();

        protected override void OnAllBuildablesBuilt()
        {
            m_Structure.PlayBuildEffects();
            Destroy(this);
        }

        protected override Vector3 GetPreviewCenter()
        {
            var center = Vector3.zero;
            int notBuildCount = 0;

            foreach (var pair in buildableRequirements)
            {
                if (!AreRequirementsMet(pair.Value))
                {
                    center += pair.Key.transform.position;
                    notBuildCount++;
                }
            }

            center /= notBuildCount;

            return center;
        }

        private void Awake() => m_Structure = GetComponent<StructureManager>();

        #region Save & Load
        public override void LoadMembers(object[] members)
        {
            buildableRequirements = new();

            var buildRequirements = members[0] as BuildRequirement[][];
            var buildables = m_Structure.Buildables;

            for (int i = 0; i < buildables.Count; i++)
            {
                var buildable = buildables[i];
                var buildReq = buildRequirements[i];

                // Enable preview.
                if (buildReq != null)
                {
                    buildableRequirements.Add(buildable, buildReq);
                    buildable.OnCreated(false);
                    buildable.OnPlaced(false);
                }
            }

            EnablePreview();
            CalculateCenter();
        }

        public override object[] SaveMembers()
        {
            var buildRequirements = new BuildRequirement[m_Structure.Buildables.Count][];
            var buildables = m_Structure.Buildables;

            for (int i = 0; i < buildables.Count; i++)
            {
                if (buildableRequirements.TryGetValue(buildables[i], out var req))
                    buildRequirements[i] = req;
            }

            object[] members = new object[]
            {
                buildRequirements
            };

            return members;
        }
        #endregion
    }
}