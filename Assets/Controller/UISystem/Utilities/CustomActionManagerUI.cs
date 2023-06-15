using IdenticalStudios.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IdenticalStudios.UISystem
{
    public sealed class CustomActionManagerUI : MonoBehaviour
    {
        #region Internal
        public struct AParams
        {
            public float StartTime { get; }
            public float EndTime { get; }
            public string Name { get; }
            public string Description { get; }
            public bool CanCancel { get; }
            public UnityAction CompletedCallback { get; }
            public UnityAction CancelledCallback { get; }


            public AParams(string name, string description, float duration, bool canCancel, UnityAction completedCallback, UnityAction cancelledCallback)
            {
                this.StartTime = Time.time;
                this.EndTime = Time.time + duration;
                this.Name = name;
                this.Description = description;
                this.CanCancel = canCancel;
                this.CompletedCallback = completedCallback;
                this.CancelledCallback = cancelledCallback;
            }

            public float GetProgress() => 1f - ((EndTime - Time.time) / (EndTime - StartTime));
        }
        #endregion
        
        public bool CustomActionActive { get; private set; }
        
        public event UnityAction<AParams> ActionStart;
        public event UnityAction ActionEnd;

        [SerializeField]
        private InputContextGroup m_ActionContext;

        [SpaceArea]

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private Image m_FillImg;

        [SerializeField]
        private TextMeshProUGUI m_LoadTxt;

        [SerializeField, Range(1f, 20f)]
        private float m_AlphaLerpSpeed = 10f;

        private AParams m_CustomAction;
        private float m_ActionEndTime;
        
        private static CustomActionManagerUI s_Instance;


        private void OnEnable()
        {
            m_CanvasGroup.alpha = 0f;
            s_Instance = this;
        }

        private void OnDisable()
        {
            if (s_Instance == this)
                s_Instance = null;
        }

        public static bool TryStartAction(AParams aParams)
        {
            if (s_Instance == null)
                return false;

            s_Instance.StartAction(aParams);
            return true;
        }

        public static bool TryCancelAction()
        {
            if (s_Instance.CustomActionActive || !s_Instance.m_CustomAction.CanCancel)
                return false;
            
            s_Instance.StopAction();
            return true;
        }

        private void StartAction(AParams aParams)
        {
            if (CustomActionActive)
                StopAction();

            InputManager.PushContext(m_ActionContext);
            InputManager.PushEscapeCallback(StopAction);

            CustomActionActive = true;
            m_CustomAction = aParams;
            m_ActionEndTime = aParams.EndTime;
            ActionStart?.Invoke(aParams);

            m_CustomAction = aParams;
            m_CanvasGroup.blocksRaycasts = true;
            m_LoadTxt.text = m_CustomAction.Description;
            
            StartCoroutine(C_UpdateCurrentAction());
        }

        private void StopAction()
        {
            if (!CustomActionActive)
                return;

            InputManager.PopEscapeCallback(StopAction);
            InputManager.PopContext(m_ActionContext);

            if (Time.time > m_ActionEndTime)
                m_CustomAction.CompletedCallback?.Invoke();
            else
                m_CustomAction.CancelledCallback?.Invoke();

            ActionEnd?.Invoke();
            CustomActionActive = false;
            
            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.alpha = 0f;
        }

        private IEnumerator C_UpdateCurrentAction() 
        {
            while (CustomActionActive)
            {
                m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 1f, Time.deltaTime * m_AlphaLerpSpeed);
                m_FillImg.fillAmount = m_CustomAction.GetProgress();
                
                if (Time.time > m_ActionEndTime)
                    StopAction();

                yield return null;
            }
        }
    }
}