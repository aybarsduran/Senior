using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PolymindGames.BuildingSystem
{
    [RequireComponent(typeof(IBuildingController))]
    [HelpURL("https://polymindgames.gitbook.io/welcome-to-gitbook/qgUktTCVlUDA7CAODZfe/player/modules-and-behaviours/building#character-structure-detector-module")]
    public class CharacterStructureDetector : CharacterBehaviour, IStructureDetector
    {
        public BuildablePreview StructureInView
        {
            get => m_StructureInView;
            set 
            {
                if (m_StructureInView != value)
                {
                    m_StructureInView = value;
                    StructureChanged?.Invoke(m_StructureInView);
                }
            }
        }

        public float CancelPreviewProgress
        {
            get => m_CancelPreviewProgress;
            private set
            {
                m_CancelPreviewProgress = value;
                CancelPreviewProgressChanged?.Invoke(m_CancelPreviewProgress);
            }
        }

        public bool DetectionEnabled
        {
            get => m_DetectionEnabled;
            set
            {
                if (m_DetectionEnabled != value)
                {
                    m_DetectionEnabled = value;

                    if (value)
                        UpdateManager.AddFixedUpdate(OnUpdate);
                    else
                    {
                        UpdateManager.RemoveFixedUpdate(OnUpdate);
                        StructureInView = null;
                    }
                }
            }
        }

        public event UnityAction<float> CancelPreviewProgressChanged;
        public event UnityAction<BuildablePreview> StructureChanged;

        [SerializeField, Range(0f, 5f)]
        private float m_CancelPreviewDuration = 0.3f;

        [Title("Building Previews Detection")]

        [SerializeField, Range(0f, 10f)]
        [Tooltip("The max detection building preview distance.")]
        private float m_MaxDetectionDistance = 5f;

        [SerializeField, Range(0f, 120f)]
        [Tooltip("The max angle detection building preview.")]
        private float m_MaxDetectionAngle = 60f;

        [SerializeField, Range(0f, 2f)]
        private float m_UpdateDetectionDelay = 0.3f;

        private BuildablePreview m_StructureInView;
        private BuildablePreview m_StructureToCancel;
        private float m_NextTimeCanUpdate;

        private bool m_DetectionEnabled;
        private float m_CancelPreviewProgress;


        public void StartCancellingPreview()
        {
            if (m_StructureInView == null)
                return;

            m_StructureToCancel = m_StructureInView;
            StartCoroutine(C_CancelPreview());
        }

        public void StopCancellingPreview()
        {
            if (m_StructureToCancel == null)
                return;

            StopAllCoroutines();
            CancelPreviewProgress = 0f;
        }

        private void OnUpdate()
        {
            if (m_NextTimeCanUpdate < Time.time)
            {
                UpdateDetection();
                m_NextTimeCanUpdate = Time.time + m_UpdateDetectionDelay;
            }
        }

        private void UpdateDetection()
        {
            var allPreviews = BuildablePreview.AllPreviewsInScene;

            if (allPreviews.Count > 0)
            {
                for (int i = 0; i < allPreviews.Count; i++)
                {
                    Vector3 playerPosition = Character.transform.position;
                    Vector3 previewPosition = allPreviews[i].PreviewCenter;
                    Vector3 dirFromPlayerToPreview = previewPosition - playerPosition;

                    float distanceToPreviewSqr = (playerPosition - previewPosition).sqrMagnitude;

                    if (distanceToPreviewSqr < m_MaxDetectionDistance * m_MaxDetectionDistance && Vector3.Angle(dirFromPlayerToPreview, Character.transform.forward) < m_MaxDetectionAngle)
                    {
                        StructureInView = allPreviews[i];
                        return;
                    }
                }
            }

            StructureInView = null;
        }

        private IEnumerator C_CancelPreview()
        {
            float endTime = Time.time + m_CancelPreviewDuration;

            while (Time.time < endTime)
            {
                CancelPreviewProgress = 1 - ((endTime - Time.time) / m_CancelPreviewDuration);
                yield return null;
            }

            CancelPreviewProgress = 0f;
            if (m_StructureToCancel != null)
                m_StructureToCancel.CancelPreview();
        }
    }
}