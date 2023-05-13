using IdenticalStudios.Surfaces;
using UnityEngine;

namespace IdenticalStudios.Surfaces
{
    public class SurfaceIdentity : MonoBehaviour
    {
        public SurfaceDefinition Surface { get => m_Surface; set => m_Surface = value; }

        [SerializeField]
        private SurfaceDefinition m_Surface;
    }
}
