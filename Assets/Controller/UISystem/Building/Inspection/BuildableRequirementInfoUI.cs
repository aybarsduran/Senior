using IdenticalStudios.BuildingSystem;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    [AddComponentMenu(k_AddMenuPath + "Buildable Requirement Info")]
    public class BuildableRequirementInfoUI : DataInfoUI<BuildableDefinition>
    {
        [SerializeField, ReorderableList(HasLabels = false)]
        private RequirementUI[] m_Requirements;


        protected override bool CanEnableInfo()
        {
            bool enableInfo = data != null;

            if (enableInfo)
                gameObject.SetActive(true);
            
            return enableInfo;
        }

        protected override void OnInfoUpdate()
        {
            var buildReq = data.BuildRequirements;

            for (int i = 0; i < m_Requirements.Length; i++)
            {
                if (i > buildReq.Length - 1)
                {
                    m_Requirements[i].gameObject.SetActive(false);
                    continue;
                }

                m_Requirements[i].gameObject.SetActive(true);

                if (buildReq[i].BuildMaterial.IsNull)
                    continue;
                
                m_Requirements[i].Display(buildReq[i].BuildMaterial.Icon, "x" + buildReq[i].RequiredAmount);
            }
        }

        protected override void OnInfoDisabled()
        {
            gameObject.SetActive(false);
        }
    }
}