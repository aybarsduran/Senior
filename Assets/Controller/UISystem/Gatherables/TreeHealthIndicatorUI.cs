using IdenticalStudios.ResourceGathering;
using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.UISystem
{
    public class TreeHealthIndicatorUI : PlayerUIBehaviour
    {
        [Title("References")]

        [SerializeField]
        private Renderer m_Renderer;

        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private SpriteRenderer m_Sprite;

        [Title("Settings")]

        [SerializeField, Range(0f, 30f)]
        private float m_MinShowDistance = 5f;

        [SerializeField, Range(0f, 30f)]
        private float m_MaxShowDistance = 5f;

        [SerializeField, Range(0f, 120f)]
        private float m_ShowAngle = 60f;

        [SerializeField, Range(0f, 30f)]
        private float m_OpacityLerpSpeed = 5f;

        [SpaceArea]

        [SerializeField]
        private bool m_RotateWithPlayer;

        [SerializeField]
        private Gradient m_HealthColorGradient;

        private Gatherable m_ClosestTree;
        private List<Gatherable> m_AllTreesInScene;

        private float m_Opacity;
        private float m_LastTreeHealth;
        private float m_DistanceFromTree;
        private float m_AngleFromTree;

        private int m_HashedTreeDamagedTrigger;
        private int m_HashedShowIndicator;

        private int m_HealthAmountShaderId;
        private int m_HealthColorShaderId;
        private int m_AlphaShaderId;

        private bool m_IsActive = false;

        private static TreeHealthIndicatorUI s_Instance;


        protected override void OnAttachment()
        {
            m_HealthAmountShaderId = Shader.PropertyToID("_HealthAmount");
            m_HealthColorShaderId = Shader.PropertyToID("_HealthColor");
            m_AlphaShaderId = Shader.PropertyToID("_Alpha");

            m_HashedShowIndicator = Animator.StringToHash("Show");
            m_HashedTreeDamagedTrigger = Animator.StringToHash("Damage");

            m_Renderer.material.SetFloat(m_HealthAmountShaderId, 1f);
            m_Renderer.material.SetColor(m_HealthColorShaderId, m_HealthColorGradient.Evaluate(1));

            s_Instance = this;

            SetOpacity(0f);
            SetHealth(0f);
        }

        protected override void OnDetachment()
        {
            if (s_Instance == this)
                HideIndicator();
        }

        public static void ShowIndicator(GatherableDefinition[] definitions)
        {
            if (s_Instance == null)
                return;

            foreach (var def in definitions)
                Gatherable.TryGetAllGatherablesWithDefinition(def, out s_Instance.m_AllTreesInScene);

            if (!s_Instance.m_IsActive)
            {
                s_Instance.m_IsActive = true;
                UpdateManager.AddUpdate(s_Instance.UpdateIndicator);
            }
        }

        public static void HideIndicator()
        {
            if (s_Instance == null)
                return;

            s_Instance.m_AllTreesInScene = null;
            s_Instance.m_ClosestTree = null;
            s_Instance.SetOpacity(0f);

            if (s_Instance.m_IsActive)
            {
                s_Instance.m_IsActive = false;
                UpdateManager.RemoveUpdate(s_Instance.UpdateIndicator);
            }
        }

        private void UpdateIndicator()
        {
            if (Player == null)
            {
                HideIndicator();
                return;
            }

            var prevTree = m_ClosestTree;

            m_ClosestTree = null;
            m_DistanceFromTree = 1000f;

            if (m_AllTreesInScene.Count > 0)
            {
                Vector3 playerPosition = Player.transform.position;

                foreach (var tree in m_AllTreesInScene)
                {
                    if (tree == null)
                        continue;

                    Vector3 previewPosition = tree.transform.position + tree.transform.TransformVector(tree.GatherOffset);
                    Vector3 dirFromPlayerToTree = previewPosition - playerPosition;

                    m_DistanceFromTree = Vector3.Distance(playerPosition, previewPosition);
                    m_AngleFromTree = Vector3.Angle(dirFromPlayerToTree, Player.transform.forward);

                    if (m_DistanceFromTree < m_MaxShowDistance && m_AngleFromTree < m_ShowAngle && tree.Health > Mathf.Epsilon)
                    {
                        m_ClosestTree = tree;
                        transform.position = previewPosition;

                        m_Sprite.sprite = m_ClosestTree.Definition.Icon;

                        UpdateHealth();

                        if (prevTree != m_ClosestTree)
                            m_Animator.SetTrigger(m_HashedShowIndicator);

                        break;
                    }
                }
            }

            UpdateOpacity(m_DistanceFromTree);

            if (m_RotateWithPlayer)
                RotateWithPlayer();
        }

        private void RotateWithPlayer()
        {
            Quaternion rot = Quaternion.LookRotation(transform.position - Player.transform.position, Vector3.up);
            rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, 0);

            transform.rotation = rot;
        }

        private void UpdateOpacity(float distanceFromTree)
        {
            float opacity;

            if (m_ClosestTree != null && m_ClosestTree.Health > 0.1f)
                opacity = (m_MaxShowDistance - distanceFromTree) / (m_MaxShowDistance - m_MinShowDistance);
            else
                opacity = Mathf.Lerp(m_Opacity, 0f, Time.deltaTime * m_OpacityLerpSpeed);

            SetOpacity(opacity);
        }

        private void SetOpacity(float opacity)
        {
            m_Opacity = opacity;
            m_Sprite.color = m_Sprite.color.SetAlpha(m_Opacity);
            m_Renderer.material.SetFloat(m_AlphaShaderId, m_Opacity);
        }

        private void UpdateHealth()
        {
            float health = m_ClosestTree.Health / m_ClosestTree.MaxHealth;

            if (Mathf.Approximately(health, m_LastTreeHealth) || health <= Mathf.Epsilon)
                return;

            SetHealth(health);

            m_Animator.SetTrigger(m_HashedTreeDamagedTrigger);

            m_LastTreeHealth = health;
        }

        private void SetHealth(float health)
        {
            Color healthColor = m_HealthColorGradient.Evaluate(health);

            m_Renderer.material.SetFloat(m_HealthAmountShaderId, health);
            m_Renderer.material.SetColor(m_HealthColorShaderId, healthColor);
        }
     }
}
