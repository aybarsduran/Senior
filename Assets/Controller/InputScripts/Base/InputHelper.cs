using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IdenticalStudios.InputSystem
{
    public static class InputHelper
    {
        private static readonly Dictionary<InputActionReference, int> s_EnabledActionsDict = new(32);


        public static void RegisterStarted(this InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null)
            {
                Debug.LogWarning("The passed input action is null, you need to set it in the inspector for this input to work.");
                return;
            }

            Enable(actionRef);
            actionRef.action.started += callback;
        }

        public static void RegisterPerformed(this InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null)
                return;

            Enable(actionRef);
            actionRef.action.performed += callback;
        }

        public static void RegisterCanceled(this InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null)
                return;

            Enable(actionRef);
            actionRef.action.canceled += callback;
        }

        public static void UnregisterStarted(this InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null)
                return;

            actionRef.action.started -= callback;
            TryDisable(actionRef);
        }

        public static void UnregisterPerfomed(this InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null)
                return;

            actionRef.action.performed -= callback;
            TryDisable(actionRef);
        }

        public static void UnregisterCanceled(this InputActionReference actionRef, Action<InputAction.CallbackContext> callback)
        {
            if (actionRef == null)
                return;

            actionRef.action.canceled -= callback;
            TryDisable(actionRef);
        }

        public static void Enable(this InputActionReference actionRef)
        {
            if (actionRef == null)
                return;

            if (s_EnabledActionsDict.TryGetValue(actionRef, out var listenerCount))
                s_EnabledActionsDict[actionRef] = listenerCount + 1;
            else
            {
                s_EnabledActionsDict.Add(actionRef, 1);
                actionRef.action.Enable();
            }
        }

        public static void TryDisable(this InputActionReference actionRef)
        {
            if (actionRef == null)
                return;

            if (s_EnabledActionsDict.TryGetValue(actionRef, out var listenerCount))
            {
                listenerCount--;
                if (listenerCount == 0)
                {
                    s_EnabledActionsDict.Remove(actionRef);
                    actionRef.action.Disable();
                }
                else
                    s_EnabledActionsDict[actionRef] = listenerCount;
            }
        }
    }
}
