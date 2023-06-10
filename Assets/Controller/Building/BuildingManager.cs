﻿using IdenticalStudios.BuildingSystem;
using UnityEngine;

namespace IdenticalStudios
{
    public sealed class BuildingManager : Manager<BuildingManager>
	{
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() => SetInstance();

        public static StructureManager StructurePrefab => Instance.m_CustomStructure;
        public static MaterialEffectInfo PlacementAllowedMaterialEffect => Instance.m_PlacementAllowedMaterial;
        public static MaterialEffectInfo PlacementDeniedMaterialEffect => Instance.m_PlacementDeniedMaterial;
        public static LayerMask FreePlacementMask => Instance.m_FreePlacementMask;
        public static LayerMask OverlapCheckMask => Instance.m_OverlapCheckMask;
        public static LayerMask CharacterLayers => Instance.m_CharacterMask;
        public static int BuildableLayer => Instance.m_BuildableLayer;
        public static int BuildablePreviewLayer => Instance.m_BuildablePreviewLayer;
        public static int DefaultBuildableIndex => Instance.m_DefaultCustomBuildableIndex;

        private static BuildableDefinition[] m_SocketBasedDefinitions;
        public static BuildableDefinition[] CustomBuildingDefinitions
        {
            get
            {
                if (m_SocketBasedDefinitions == null)
                    m_SocketBasedDefinitions = BuildableDefinition.GetAllBuildablesOfType<StructureBuildable>();

                return m_SocketBasedDefinitions;
            }
        }

        //Custom Building"

        [SerializeField]
        private StructureManager m_CustomStructure;

        [SerializeField, Range(0, 1000)]
        private int m_DefaultCustomBuildableIndex = 3;

        //Masks & Layers

        [SerializeField]
        [Tooltip("Tells the controller on what layers can buildables be placed.")]
        private LayerMask m_FreePlacementMask;

        [SerializeField]
        [Tooltip("Tells the controller what layers to use when checking for collisions.")]
        private LayerMask m_OverlapCheckMask;

        [SerializeField]
        [Tooltip("Layer mask to detect entities (AI or players).")]
        private LayerMask m_CharacterMask;

        [SerializeField]
        private int m_BuildableLayer;

        [SerializeField]
        private int m_BuildablePreviewLayer;

        //Materials

        [SerializeField]
        private MaterialEffectInfo m_PlacementAllowedMaterial;

        [SerializeField]
        private MaterialEffectInfo m_PlacementDeniedMaterial;
    }
}