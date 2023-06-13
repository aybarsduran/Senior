using IdenticalStudios.BuildingSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public class BookBuildableUI : PlayerUIBehaviour
    {
        [SpaceArea]

        [Title("Buildables")]

        [SerializeField, DataReferenceDetails(HasNullElement = false)]
        private DataIdReference<BuildableCategoryDefinition> m_BuildableCategory;

        [Title("Template")]

        [SerializeField]
        private BuildableDefinitionSlotUI m_Template;

        [SerializeField]
        private RectTransform m_TemplateSpawnRect;

        [Title("Graphics")]

        [SerializeField, NotNull]
        private TextMeshProUGUI m_NameTxt;
        
        [SerializeField, NotNull]
        private Image m_CategoryImg;

        private BuildableDefinitionSlotUI[] m_Slots;


        protected override void OnAttachment()
        {
            var category = m_BuildableCategory.Def;
            var buildables = new List<BuildableDefinition>(category.Members.Length);
            buildables.AddRange(category.Members);

            bool wasActive = m_TemplateSpawnRect.gameObject.activeInHierarchy;
            m_TemplateSpawnRect.gameObject.SetActive(true);

            m_Slots = new BuildableDefinitionSlotUI[buildables.Count];
            for (int i = 0; i < m_Slots.Length; i++)
            {
                m_Slots[i] = Instantiate(m_Template, m_TemplateSpawnRect);
                m_Slots[i].SetBuildable(buildables[i]);
                m_Slots[i].Selectable.OnSelected += StartBuilding;
            }

            if (!wasActive)
                m_TemplateSpawnRect.gameObject.SetActive(false);
        }

        protected override void OnDetachment()
        {
            for (int i = 0; i < m_Slots.Length; i++)
                m_Slots[i].Selectable.OnSelected -= StartBuilding;
        }

        private void StartBuilding(SelectableUI selectable)
        {
            var buildable = selectable.GetComponent<BuildableDefinitionSlotUI>().BuildableDef;
            GetModule<IWieldableSurvivalBookHandler>().TryStopInspection(buildable);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityUtils.SafeOnValidate(this, () =>
            {
                if (m_CategoryImg != null)
                    m_CategoryImg.sprite = m_BuildableCategory.Icon;

                if (m_NameTxt != null)
                    m_NameTxt.text = m_BuildableCategory.Name;
            });
        }
#endif
    }
}