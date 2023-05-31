using IdenticalStudios;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public sealed class UpdateManager : Manager<UpdateManager>
    {
        #region Internal
        private class RuntimeObject : MonoBehaviour
        {
            public UnityAction UpdateCallback;
            public UnityAction FixedUpdateCallback;
            public UnityAction LateUpdateCallback;

            private void Update() => UpdateCallback();
            private void FixedUpdate() => FixedUpdateCallback();
            private void LateUpdate() => LateUpdateCallback();
        }

        private class DelayedAction
        {
            public Behaviour Parent;
            public UnityAction Action;
            public float CallTime;


            public DelayedAction(Behaviour parent, UnityAction action, float time)
            {
                Parent = parent;
                Action = action;
                CallTime = time;
            }
        }
        #endregion

        #region Initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() => CreateInstance();

        protected override void OnInitialized()
        {
            m_RuntimeObject = CreateRuntimeObject<RuntimeObject>("UpdateRuntimeObject");
            m_RuntimeObject.UpdateCallback = Update;
            m_RuntimeObject.FixedUpdateCallback = FixedUpdate;
            m_RuntimeObject.LateUpdateCallback = LateUpdate;
        }
        #endregion

        private readonly List<UnityAction> m_UpdateListeners = new();
        private readonly List<UnityAction> m_LateUpdateListeners = new();
        private readonly List<UnityAction> m_FixedUpdateListeners = new();
        private readonly List<UnityAction<float>> m_UpdateDeltaListeners = new();
        private readonly List<UnityAction<float>> m_LateUpdateDeltaListeners = new();
        private readonly List<UnityAction<float>> m_FixedUpdateDeltaListeners = new();

        private readonly List<DelayedAction> m_DelayedActions = new();
        private RuntimeObject m_RuntimeObject;

        private readonly static bool s_DEBUG_REMOVE_NULL = false;


        public static void AddUpdate(UnityAction action) => Instance.m_UpdateListeners.Add(action);
        public static void AddUpdate(UnityAction<float> action) => Instance.m_UpdateDeltaListeners.Add(action);
        public static bool RemoveUpdate(UnityAction action) => Instance.m_UpdateListeners.Remove(action) || ThrowNotRegisteredError();
        public static bool RemoveUpdate(UnityAction<float> action) => Instance.m_UpdateDeltaListeners.Remove(action) || ThrowNotRegisteredError();

        public static void AddLateUpdate(UnityAction action) => Instance.m_LateUpdateListeners.Add(action);
        public static void AddLateUpdate(UnityAction<float> action) => Instance.m_LateUpdateDeltaListeners.Add(action);
        public static bool RemoveLateUpdate(UnityAction action) => Instance.m_LateUpdateListeners.Remove(action) || ThrowNotRegisteredError();
        public static bool RemoveLateUpdate(UnityAction<float> action) => Instance.m_LateUpdateDeltaListeners.Remove(action) || ThrowNotRegisteredError();

        public static void AddFixedUpdate(UnityAction action) => Instance.m_FixedUpdateListeners.Add(action);
        public static void AddFixedUpdate(UnityAction<float> action) => Instance.m_FixedUpdateDeltaListeners.Add(action);
        public static bool RemoveFixedUpdate(UnityAction action) => Instance.m_FixedUpdateListeners.Remove(action) || ThrowNotRegisteredError();
        public static bool RemoveFixedUpdate(UnityAction<float> action) => Instance.m_FixedUpdateDeltaListeners.Remove(action) || ThrowNotRegisteredError();

        public static void InvokeDelayedAction(Behaviour parent, UnityAction action, float delay)
        {
            if (action == null || parent == null)
                return;

            if (delay < 0.001f)
            {
                action?.Invoke();
                return;
            }

            var delayedAction = new DelayedAction(parent, action, Time.unscaledTime + delay);
            Instance.m_DelayedActions.Add(delayedAction);
        }

        public static void StopDelayedAction(Behaviour parent, UnityAction action)
        {
            var delayedActions = Instance.m_DelayedActions;
            for (int i = 0; i < delayedActions.Count; i++)
            {
                var delayedAction = delayedActions[i];
                if (delayedAction.Parent == parent && delayedAction.Action == action)
                {
                    delayedActions.RemoveAt(i);
                    break;
                }
            }
        }

        public static void StopAllDelayedActionsFor(Behaviour parent)
        {
            var delayedActions = Instance.m_DelayedActions;
            for (int i = 0; i < delayedActions.Count; i++)
            {
                var delayedAction = delayedActions[i];
                if (delayedAction.Parent == parent)
                {
                    delayedActions.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < m_UpdateListeners.Count; i++)
                m_UpdateListeners[i]?.Invoke();

            for (int i = 0; i < m_UpdateDeltaListeners.Count; i++)
                m_UpdateDeltaListeners[i]?.Invoke(Time.unscaledDeltaTime);

            UpdateDelayedActions();
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < m_FixedUpdateListeners.Count; i++)
                m_FixedUpdateListeners[i]?.Invoke();

            for (int i = 0; i < m_FixedUpdateDeltaListeners.Count; i++)
                m_FixedUpdateDeltaListeners[i]?.Invoke(Time.fixedUnscaledDeltaTime);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < m_LateUpdateListeners.Count; i++)
                m_LateUpdateListeners[i]?.Invoke();

            for (int i = 0; i < m_LateUpdateDeltaListeners.Count; i++)
                m_LateUpdateDeltaListeners[i]?.Invoke(Time.unscaledDeltaTime);
        }

        private void UpdateDelayedActions()
        {
            int index = 0;
            float time = Time.unscaledTime;
            while (index < m_DelayedActions.Count)
            {
                var action = m_DelayedActions[index];
                if (action.CallTime < time)
                {
                    if (action.Parent != null && action.Parent.enabled)
                        action.Action();

                    m_DelayedActions.RemoveAt(index);
                }
                else
                    index++;
            }
        }

        private static bool ThrowNotRegisteredError()
        {
#if UNITY_EDITOR
            if (!s_DEBUG_REMOVE_NULL)
                return false;

            const string error = "You're trying to unregister an action from the Update Manager that hasn't been registered.";
            Debug.LogError(error);
#endif

            return false;
        }
    }
}
