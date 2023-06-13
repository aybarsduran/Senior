using IdenticalStudios.BuildingSystem;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    public class CarriableBuildAction : CarriableActionBehaviour
    {
        public DataIdReference<BuildMaterialDefinition> BuildMaterial => m_BuildMaterial;

        [SerializeField, DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<BuildMaterialDefinition> m_BuildMaterial;

        private IStructureDetector m_StructureDetector;


        public override bool TryUseCarriable()
        {
            if (m_StructureDetector != null || Carriable.Character.TryGetModule(out m_StructureDetector))
            {
                var structureInView = m_StructureDetector.StructureInView;
                if (structureInView != null && structureInView.TryAddBuildingMaterial(m_BuildMaterial))
                    return true;
            }

            return false;
        }
    }
}
