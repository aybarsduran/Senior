using UnityEngine;

namespace IdenticalStudios.Surfaces
{
    [CreateAssetMenu(menuName = "Identical Studios/Surfaces/Surface Textures Set", fileName = "(SurfaceTexturesSet) ")]
    public class SurfaceTexturesSet : ScriptableObject
    {
        public SurfaceDefinition Surface
        {
            get
            {
                if (m_Surface.IsNull)
                {
                    Debug.LogError($"The surface field on ''{name}'' cannot be null, assign it or delete this set.");
                    return null;
                }

                return m_Surface.Def;
            }
        }

        public Texture[] Textures => m_Textures;

        [SerializeField]
        private DataNameReference<SurfaceDefinition> m_Surface;

        [SpaceArea]

        [SerializeField, ReorderableList(HasLabels = false)]
        private Texture[] m_Textures;
    }
}
