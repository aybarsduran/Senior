using UnityEngine;

namespace IdenticalStudios
{
    [DisallowMultipleComponent]
    public class MaterialEffect : MonoBehaviour
    {
        [SerializeField]
        private MaterialEffectInfo m_DefaultEffect;

        [SpaceArea]

        [SerializeField, ReorderableList(ListStyle.Boxed, HasLabels = false)]
#if UNITY_EDITOR
        [EditorButton(nameof(Reset), "Reset", ButtonActivityType.OnEditMode)]
#endif
        private Renderer[] m_Renderers;

        private MaterialEffectInfo m_ActiveEffect;
        private Material[][] m_BaseMaterials;


        public void EnableDefaultEffect()
        {
            // No point in throwing an error for this, if there's no default effect found we'll try to disable the active effect..
            if (m_DefaultEffect == null)
            {
                DisableActiveEffect();
                return;
            }

            EnableEffect(m_DefaultEffect);
        }

        public void EnableCustomEffect(MaterialEffectInfo effect)
        {
            if (effect == null)
            {
                Debug.LogError($"Cannot enable a null custom effect on ''{gameObject}''.");
                return;
            }

            EnableEffect(effect);
        }

        public void DisableActiveEffect()
        {
            if (m_ActiveEffect == null)
                return;

            for (int i = 0; i < m_Renderers.Length; i++)
                m_Renderers[i].sharedMaterials = m_BaseMaterials[i];

            m_ActiveEffect = null;
        }

        private void EnableEffect(MaterialEffectInfo effect)
        {
            if (m_ActiveEffect == effect)
                return;

            if (m_BaseMaterials == null)
                m_BaseMaterials = GetBaseMaterials();

            if (effect.EffectMode == MaterialEffectInfo.EffectType.StackWithBaseMaterials)
            {
                for (int i = 0; i < m_Renderers.Length; i++)
                {
                    var baseMaterials = m_BaseMaterials[i];
                    var newMaterials = new Material[baseMaterials.Length + 1];

                    newMaterials[baseMaterials.Length] = effect.Material;
                    for (int j = 0; j < baseMaterials.Length; j++)
                        newMaterials[j] = baseMaterials[j];

                    m_Renderers[i].sharedMaterials = newMaterials;
                }
            }
            else
            {
                for (int i = 0; i < m_Renderers.Length; i++)
                {
                    var effectMaterials = new Material[m_Renderers[i].sharedMaterials.Length];

                    for (int j = 0; j < effectMaterials.Length; j++)
                        effectMaterials[j] = effect.Material;

                    m_Renderers[i].sharedMaterials = effectMaterials;
                }
            }

            m_ActiveEffect = effect;
        }

        private Material[][] GetBaseMaterials()
        {
            var allMaterials = new Material[m_Renderers.Length][];

            for (int i = 0; i < allMaterials.Length; i++)
            {
                var sharedMaterials = m_Renderers[i].sharedMaterials;
                var materials = new Material[sharedMaterials.Length];

                for (int j = 0; j < sharedMaterials.Length; j++)
                    materials[j] = sharedMaterials[j];

                allMaterials[i] = materials;
            }

            return allMaterials;
        }

#if UNITY_EDITOR
        protected void Reset()
        {
            m_Renderers = GetComponentsInChildren<Renderer>(true);

            var meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
            var skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);

            m_Renderers = new Renderer[meshRenderers.Length + skinnedRenderers.Length];

            meshRenderers.CopyTo(m_Renderers, 0);
            skinnedRenderers.CopyTo(m_Renderers, meshRenderers.Length);

            m_DefaultEffect = GetDefaultInfo();
        }

        private MaterialEffectInfo GetDefaultInfo()
        {
            var infos = UnityExtensions.FindAndGetAssetOfType<MaterialEffectInfo>();

            if (infos == null || infos.Count == 0)
                return null;

            foreach (var info in infos)
            {
                if (info.name.Contains("Outline"))
                    return info;
            }

            return infos[0];
        }
#endif
    }
}
