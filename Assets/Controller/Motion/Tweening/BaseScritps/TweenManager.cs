using IdenticalStudios;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.ProceduralMotion
{
    public sealed class TweenManager : Manager<TweenManager>
    {
        private sealed class RuntimeObject : MonoBehaviour
        {
            public UnityAction OnUpdate;
            private void Update() => OnUpdate?.Invoke();
        }

        private struct DelayedTween
        {
            public ITween Tween;
            public UnityAction Action;
            public float Time;
        }

        private RuntimeObject m_UpdateSceneObject;
        private readonly List<ITween> m_Tweens = new();
        private readonly List<DelayedTween> m_DelayedTweens = new();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void Init() => CreateInstance();

        protected override void OnInitialized()
        {
            m_Tweens.Clear();
            m_DelayedTweens.Clear();

            m_UpdateSceneObject = CreateRuntimeObject<RuntimeObject>("TweeningRuntimeObject");
            m_UpdateSceneObject.OnUpdate = Update;
        }

        internal static void AddTween(ITween tween)
        {
            if (tween != null)
                Instance.m_Tweens.Add(tween);
        }

        internal static void AddTweenDelayed(ITween tween, float delay, UnityAction callback)
        {
            if (tween == null)
                return;

            if (delay < 0.001f)
            {
                Instance.m_Tweens.Add(tween);
                callback?.Invoke();
                return;
            }

            var delayedTween = new DelayedTween()
            {
                Tween = tween,
                Action = callback,
                Time = Time.unscaledTime + delay
            };

            Instance.m_DelayedTweens.Add(delayedTween);
        }

        internal static void RemoveTween(ITween tween)
        {
            if (tween != null)
            {
                if (!Instance.m_Tweens.Remove(tween))
                {
                    var delayedTweens = Instance.m_DelayedTweens;

                    int index = 0;
                    while (index < delayedTweens.Count)
                    {
                        if (delayedTweens[index].Tween == tween)
                        {
                            delayedTweens.RemoveAt(index);
                            return;
                        }
                        else
                            index++;
                    }
                }
            }
        }

        private void Update()
        {
            // Update the active tweens..
            int i = 0;
            while (i < m_Tweens.Count)
            {
                var tween = m_Tweens[i];
                if (tween.IsPlaying)
                {
                    tween.Update();
                    i++;
                }
                else
                    m_Tweens.RemoveAt(i);
            }

            // Update the delayed tweens..
            i = 0;
            float time = Time.unscaledTime;
            while (i < m_DelayedTweens.Count)
            {
                if (m_DelayedTweens[i].Time < time)
                {
                    m_Tweens.Add(m_DelayedTweens[i].Tween);
                    m_DelayedTweens[i].Action?.Invoke();
                    m_DelayedTweens.RemoveAt(i);
                }
                else
                    i++;
            }
        }
    }
}
