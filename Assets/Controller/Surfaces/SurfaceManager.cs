using IdenticalStudios.PoolingSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IdenticalStudios.Surfaces
{
    /// <summary>
    /// Global surface effects system
    /// </summary>
    [CreateAssetMenu(menuName = "Identical Studios/Managers/Surfaces", fileName = "SurfaceManager")]
    public sealed class SurfaceManager : Manager<SurfaceManager>
	{
		#region Internal
		private class TerrainInfo
		{
			public Vector3 Position { get; }
			public TerrainData Data { get; }

			private readonly TerrainLayer[] m_Layers;


			public TerrainInfo(Terrain terrain)
			{
				Position = terrain.GetPosition();

				Data = terrain.terrainData;
				m_Layers = Data.terrainLayers;
			}

			public Texture GetSplatmapPrototypeId(int i)
			{
				if (m_Layers != null && m_Layers.Length > i)
					return m_Layers[i].diffuseTexture;

				return null;
			}
		}
		#endregion
		
		#region Initialization
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Init() => SetInstance();
		
		protected override void OnInitialized()
		{
			CacheSurfacesByTexture();
			CacheSurfaceEffects();

			SceneManager.sceneUnloaded += OnSceneUnloaded;
			void OnSceneUnloaded(Scene scene) => m_CachedTerrains.Clear();
		}

		private void CacheSurfacesByTexture()
		{
			var surfacesByTexture = Resources.LoadAll<SurfaceTexturesSet>("Definitions/Surfaces/");

			if (surfacesByTexture == null || surfacesByTexture.Length == 0)
				return;

            foreach (var surfByTexture in surfacesByTexture)
            {
				foreach (var texture in surfByTexture.Textures)
					m_SurfacesByTexture.Add(texture, surfByTexture.Surface);
			}
		}

		private void CacheSurfaceEffects()
		{
			var surfaces = SurfaceDefinition.Definitions;
			foreach (var surface in surfaces)
			{
				int surfaceID = surface.Id;

				CreatePoolForEffect($"{surface.name}_{SurfaceEffects.SoftFootstep}", surface.SoftFootstepEffect, surfaceID + (int)SurfaceEffects.SoftFootstep, m_EffectReleaseDelay);
				CreatePoolForEffect($"{surface.name}_{SurfaceEffects.HardFootstep}", surface.HardFootstepEffect, surfaceID + (int)SurfaceEffects.HardFootstep, m_EffectReleaseDelay);
				CreatePoolForEffect($"{surface.name}_{SurfaceEffects.FallImpact}", surface.FallImpactEffect, surfaceID + (int)SurfaceEffects.FallImpact, m_EffectReleaseDelay);

				CreatePoolForEffect($"{surface.name}_{SurfaceEffects.BulletHit}", surface.BulletHitEffect, surfaceID + (int)SurfaceEffects.BulletHit, m_DecalReleaseDelay);
				CreatePoolForEffect($"{surface.name}_{SurfaceEffects.Slash}", surface.SlashEffect, surfaceID + (int)SurfaceEffects.Slash, m_DecalReleaseDelay);
				CreatePoolForEffect($"{surface.name}_{SurfaceEffects.Stab}", surface.StabEffect, surfaceID + (int)SurfaceEffects.Stab, m_DecalReleaseDelay);
			}
		}

		private void CreatePoolForEffect(string effectName, SurfaceDefinition.EffectPair effectPair, int surfaceID, float releaseDelay)
		{
			var effectTemplate = new GameObject(effectName);
			var effectComponent = effectTemplate.AddComponent<SurfaceEffect>();
			effectComponent.Init(effectPair.AudioEffect, effectPair.VisualEffect, m_SpatializeAudio);

			var pool = PoolingManager.CreatePool(effectTemplate, m_InitialPoolSize, m_PoolCapacity, releaseDelay);
			m_SurfaceEffects.Add(surfaceID, pool);

			Destroy(effectTemplate);
		}
		#endregion

		[SerializeField, InLineEditor]
		private SurfaceDefinition m_DefaultSurface;

		[SerializeField]
		private bool m_SpatializeAudio = false;

		[Title("Pooling")]

		[SerializeField, Range(0, 128)]
		private int m_InitialPoolSize = 4;

		[SerializeField, Range(4, 128)]
		private int m_PoolCapacity = 16;

		[SerializeField, Range(1f, 1000f)]
		private float m_DecalReleaseDelay = 30f;

		[SerializeField, Range(1f, 1000f)]
		private float m_EffectReleaseDelay = 3f;

		private readonly Dictionary<TerrainCollider, TerrainInfo> m_CachedTerrains = new(4);
		private readonly Dictionary<Texture, SurfaceDefinition> m_SurfacesByTexture = new(32);
		private readonly Dictionary<int, ObjectPool> m_SurfaceEffects = new(16);


		public static SurfaceDefinition SpawnEffect(RaycastHit hitInfo, SurfaceEffects effectType, float audioVolume, bool parentEffect = false)
		{
			return SpawnEffect(hitInfo, effectType, audioVolume, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), parentEffect);
		}

		public static SurfaceDefinition SpawnEffect(RaycastHit hitInfo, SurfaceEffects effectType, float audioVolume, Vector3 position, Quaternion rotation, bool parentEffect = false)
		{
			if (Instance.TryGetSurfaceDefinition(hitInfo, out var surfInfo))
			{
				if (Instance.m_SurfaceEffects.TryGetValue(surfInfo.Id + (int)effectType, out var pool))
				{
					var effect = pool.GetInstance();
					effect.transform.SetPositionAndRotation(position, rotation);

					if (parentEffect)
						effect.transform.SetParent(hitInfo.transform, true);

					effect.GetComponent<SurfaceEffect>().Play(audioVolume);
				}
			}

			return surfInfo;
		}

		public static void SpawnEffect(int surfaceId, SurfaceEffects effectType, float audioVolume, Vector3 position, Quaternion rotation)
		{
			SurfaceDefinition surfInfo = SurfaceDefinition.GetWithId(surfaceId);
			if (Instance.m_SurfaceEffects.TryGetValue(surfInfo.GetHashCode() + (int)effectType * 100, out var pool))
			{
				var effect = pool.GetInstance();

				effect.transform.SetPositionAndRotation(position, rotation);
				effect.GetComponent<SurfaceEffect>().Play(audioVolume);
			}
		}

		public static SurfaceDefinition GetSurfaceInfo(RaycastHit hitInfo)
		{
			Instance.TryGetSurfaceDefinition(hitInfo, out var surfDef);
			return surfDef;
		}

		public static SurfaceDefinition GetSurfaceInfo(Material material)
		{
			Instance.m_SurfacesByTexture.TryGetValue(material.mainTexture, out var surfDef);
			return surfDef;
		}

		public static SurfaceDefinition GetSurfaceInfo(Texture texture)
		{
			Instance.m_SurfacesByTexture.TryGetValue(texture, out var surfDef);
			return surfDef;
		}

		private bool TryGetSurfaceDefinition(RaycastHit hitInfo, out SurfaceDefinition surfaceDef)
		{
			Collider col = hitInfo.collider;

			// Check if the collider has a surface identity component beforehand, if not get the texture from the renderer.
			if (col.TryGetComponent(out SurfaceIdentity surfId))
			{
				if (surfId.Surface == null)
				{
					surfaceDef = m_DefaultSurface;
					return true;
				}

				surfaceDef = surfId.Surface;
				return true;
			}

			// Get the surface from the terrain.
			if (col is TerrainCollider terrainCollider)
			{
				TerrainInfo terrainInfo = m_CachedTerrains.GetValueOrDefault(terrainCollider);

				if (terrainInfo == null)
				{
					terrainInfo = new TerrainInfo(terrainCollider.GetComponent<Terrain>());
					m_CachedTerrains.Add(terrainCollider, terrainInfo);
				}

				float[] textureMix = GetTerrainTextureMix(hitInfo.point, terrainInfo.Data, terrainInfo.Position);
				int textureIndex = GetTerrainLayerIndex(textureMix);

				var terrainTexture = terrainInfo.GetSplatmapPrototypeId(textureIndex);

				return m_SurfacesByTexture.TryGetValue(terrainTexture, out surfaceDef);
			}

			// Get the surface from the texture.
			var materialTexture = GetTextureFromTriangle(col, hitInfo.triangleIndex);

			if (materialTexture != null && m_SurfacesByTexture.TryGetValue(materialTexture, out surfaceDef))
				return true;

			surfaceDef = m_DefaultSurface;
			return true;
		}

		private Texture GetTextureFromTriangle(Collider collider, int triangleIndex)
		{
			Renderer renderer = collider.GetComponent<Renderer>();
			if (renderer == null || renderer.sharedMaterials.Length == 0)
				return null;

			if (renderer.sharedMaterials.Length == 1)
				return renderer.sharedMaterials[0].mainTexture;

			MeshCollider meshCollider = collider as MeshCollider;
			if (meshCollider == null || meshCollider.convex)
				return renderer.sharedMaterials[0].mainTexture;

			Mesh mesh = meshCollider.sharedMesh;
			if (mesh.isReadable == false)
				return renderer.sharedMaterials[0].mainTexture;

			int materialIndex = -1;
			int lookupIndex1 = mesh.triangles[triangleIndex * 3];
			int lookupIndex2 = mesh.triangles[triangleIndex * 3 + 1];
			int lookupIndex3 = mesh.triangles[triangleIndex * 3 + 2];

			for (int i = 0;i < mesh.subMeshCount;i++)
			{
				int[] triangles = mesh.GetTriangles(i);

				for (int j = 0;j < triangles.Length;j += 3)
				{
					if (triangles[j] == lookupIndex1 && triangles[j + 1] == lookupIndex2 && triangles[j + 2] == lookupIndex3)
					{
						materialIndex = i;
						break;
					}
				}

				if (materialIndex != -1)
					break;
			}

			return renderer.sharedMaterials[materialIndex].mainTexture;
		}

		private static float[] GetTerrainTextureMix(Vector3 worldPos, TerrainData terrainData, Vector3 terrainPos)
		{
			// Returns an array containing the relative mix of textures
			// on the terrain at this world position.

			// The number of values in the array will equal the number
			// of textures added to the terrain.

			// Calculate which splat map cell the worldPos falls within (ignoring y)
			int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
			int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);

			// Get the splat data for this cell as a 1x1xN 3D array (where N = number of textures)
			float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

			// Extract the 3D array data to a 1D array:
			float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];

			for(int n = 0;n < cellMix.Length;n++)
				cellMix[n] = splatmapData[0, 0, n];

			return cellMix;
		}

		private static int GetTerrainLayerIndex(float[] textureMix)
		{
			// Returns the zero-based index of the most dominant texture
			float maxMix = 0;
			int maxIndex = 0;

			// Loop through each mix value and find the maximum.
			for (int n = 0;n < textureMix.Length;n++)
			{
				if (textureMix[n] > maxMix)
				{
					maxIndex = n;
					maxMix = textureMix[n];
				}
			}

			return maxIndex;
		}
	}
}