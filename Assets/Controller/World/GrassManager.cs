using UnityEngine;

namespace IdenticalStudios
{
    [ExecuteInEditMode]
	public sealed class GrassManager : MonoBehaviour 
	{
		[SerializeField, Range(0f, 1f)]
		private float m_Shininess = 0.5f;


		private void OnEnable() => Shader.SetGlobalFloat("_GrassShininess", m_Shininess);
		private void OnValidate() => Shader.SetGlobalFloat("_GrassShininess", m_Shininess);
	}
}
