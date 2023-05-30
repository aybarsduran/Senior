using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace IdenticalStudios.ProceduralMotion
{
    internal interface ITween
    {
        bool IsPlaying { get; }

        void Play();
        void Update();
        void Cancel();

        Task Await();
        IEnumerator Yield();

    }

    /// <summary>
    /// Base class for all Tweens with a value type for the driver and a component.
    /// </summary>
    /// <typeparam name="DriverValueType">The type of the Tweened value.</typeparam>
    /// <typeparam name="ComponentType">The type of the target Component.</typeparam>
    [Serializable]
    public abstract class Tween<DriverValueType, ComponentType> : Tween<DriverValueType> where ComponentType : Component
    {
        /// <summary>
        /// A reference to the target componented fetched during the on initialize.
        /// </summary>
        [SerializeField]
        protected ComponentType component;


        /// <summary>
        /// During the on initialize cycle the target component will be fetched.
        /// </summary>
        /// <returns>Whether the component was available.</returns>
        protected override bool TryInitialize() => component != null;

        /// <summary>
        /// Adds a Tween Driver and directly set the component reference.
        /// </summary>
        /// <returns>The instanciated Driver.</returns>
        internal static Driver Add<Driver>(GameObject gameObject) where Driver : Tween<DriverValueType, ComponentType>, new()
        {
            var driver = new Driver();
            driver.component = gameObject.GetComponent<ComponentType>();
            return driver;
        }

        /// <summary>
        /// Adds a Tween Driver and directly set the component reference.
        /// </summary>
        /// <returns>The instanciated Driver.</returns>
        internal static Driver Add<Driver>(ComponentType componentType) where Driver : Tween<DriverValueType, ComponentType>, new()
        {
            var driver = new Driver();
            driver.component = componentType;
            return driver;
        }

    }

    /// <summary>
    /// Base class for all Tweens with a value type for the driver.
    /// </summary>
    [Serializable]
    public abstract class Tween<DriverValueType> : ITween
    {
        /// <summary>
        /// Defines whether the Tween is decommissioned, when set to true the Tween
        /// will be stopped and destroyed as soon as possible. All playback will stop.
        /// </summary>
        public bool IsPlaying { get; private set; } = false;
        public bool IsInitialized { get; private set; } = false;

        public DriverValueType Value => currentValue;

        [SerializeField, FormerlySerializedAs("m_DidOverwriteFrom")]
        [Tooltip("Defines whether the from value has been overwritten. " +
            "If not, the initial value will be used. Recommended: leave as false.")]
        protected bool m_IsFromLocked = false;

        [SerializeField, FormerlySerializedAs("valueFrom")]
        [Tooltip("The value the driver should Tween from.")]

        protected DriverValueType m_ValueFrom = default;

        [Space(3f)]

        [SerializeField, FormerlySerializedAs("m_IsRelative")]
        private bool m_IsToRelative = false;

        [SerializeField, FormerlySerializedAs("valueTo")]
        [Tooltip("The default value the driver should Tween to.")]
        private DriverValueType m_ValueTo = default;

        [Space(3f)]

        [SerializeField, Range(0f, 100f)]
        [Tooltip("The delay of the Tween, when defines will hold the Tween for its first play. " +
                 "Any play after that when looping or ping-ponging will not be delayed.")]
        private float m_Delay = 0f;

        [SerializeField, Range(0f, 100f)]
        [Tooltip("The duration of the Tween, it's value is used to calculate the progression of the time each frame.")]
        private float m_Duration = 1f;

        [SerializeField]
        [Tooltip("The Ease type of the Tween defining the style of animation.")]
        private EaseType m_EaseType = 0;

        [SerializeField]
        [Tooltip("The value of the loop count. When the loop count is used the Tween will keep re-playing until the loop count hits zero. " +
                 "When ping pong is enabled, a play forth and back counts as a single loop.")]
        private int m_LoopCount = 0;

        [SerializeField]
        [Tooltip("Defines whether the Tween uses ping pong during it's animation. When set to true, the twee will play back and forth. " +
                 "Going once forth and once back counts as one cycle.")]
        private bool m_HasPingPong = false;

        [SerializeField, HideInInspector]
        protected DriverValueType m_DefaultFrom = default;

        /// <summary>
        /// The state value the driving is currently tweening at.
        /// </summary>
        protected DriverValueType currentValue;

        /// <summary>
        /// The value the driver should Tween to.
        /// </summary>
        protected DriverValueType valueTo = default;

        /// <summary>
        /// The current play-time of the Tween. When actually playing, value
        /// is increased every frame based on a number of factors such as the
        /// duration and the use of unscaled timing.
        /// </summary>
        private float m_Time;

        /// <summary>
        /// The duration of the Tween, it's value is used to calculate the
        /// progression of the time each frame.
        /// </summary>
        private int m_CurrentLoopCount;

        /// <summary>
        /// Defines whether the Tween is playing forward, is only used for the
        /// ping pong mechanic.
        /// </summary>
        private bool m_IsPlayingForward;

        /// <summary>
        /// Defines whether the Tween did reach the end time. If true, and no other
        /// values such as ping pong or infinite will overwrite it, the Tween will
        /// be decommissioned.
        /// </summary>
        private bool m_DidTimeReachEnd;

        /// <summary>
        /// The start callback will be invoked when the Tween completes. This
        /// will only happen when the Tween truely start and will not invoke before
        /// the delay.
        /// </summary>
        private UnityAction m_Started;

        /// <summary>
        /// The completion callback will be invoked when the Tween completes. This
        /// will only happen when the Tween truely ends and will not invoke when
        /// the Tween is canceld or destroyed.
        /// </summary>
        private UnityAction m_Completed;

        /// <summary>
        /// The cancelation callback will be invoked when the Tween is canceled.
        /// This will only happen when the Tween is manually canceled and will not
        /// invoke when the Tween is decommissioned due to a destroy or missing
        /// component reference.
        /// </summary>
        private UnityAction m_Cancelled;


        /// <summary>
        /// On Initialize will be invoked when the Tween will initialize. During 
        /// cycle the Tween has yet to be started, but expect all chainable options 
        /// to be set already. This method should return a boolean informing the 
        /// Tween if the initialization was successfull. When it was not, the Tween 
        /// will be decommissioned.
        /// </summary>
        /// <returns>Whether the Tween is initialized succesfully.</returns>
        protected abstract bool TryInitialize();

        /// <summary>
        /// On Get From will be invoked when the Tween wants to fetch its from value.
        /// When the Tween was initialized with a SetFrom chainable option, will
        /// not be invoked.
        /// </summary>
        /// <returns>The new From value.</returns>
        protected abstract DriverValueType GetDefaultFrom();

        /// <summary>
        /// Returns the to value relative to from.
        /// </summary>
        /// <param name="valueFrom"></param>
        /// <returns></returns>
        protected abstract DriverValueType GetToRelativeToFrom(DriverValueType valueFrom, DriverValueType valueTo);

        /// <summary>
        /// On Update will be invoked every frame and passes the eased time. During
        /// cycle all animation calculations can be performed.
        /// </summary>
        /// <param name="easedTime">The current time of the ease.</param>
        protected abstract void OnUpdate(float easedTime);

        private void RecalculateValueTo() => valueTo = m_IsToRelative ? GetToRelativeToFrom(m_DefaultFrom, m_ValueTo) : m_ValueTo;

        ~Tween()
        {
            Stop();
        }

        /// <summary>
        /// Start playing this tween.
        /// </summary>
        public void Play()
        {
            if (IsPlaying)
                return;

            if (!IsInitialized)
            {
                // Invokes the OnInitialize on the Tween driver. If returned false, the
                // Tween is not ready to play or is incompatible. Then we'll decommission.
                if (TryInitialize())
                    IsInitialized = true;
                else
                    return;
            }

            m_Time = 0f;
            m_DidTimeReachEnd = false;
            m_IsPlayingForward = true;
            m_CurrentLoopCount = m_LoopCount;

            if (!m_IsFromLocked)
                m_ValueFrom = GetDefaultFrom();

            RecalculateValueTo();

            IsPlaying = true;
            TweenManager.AddTweenDelayed(this, m_Delay, OnStart);

            void OnStart()
            {
                // When the tween has no duration, the timing will not be done and the
                // animation will be set to its last frame, the Tween will be 
                // decomissioned right away.
                if (m_Duration <= 0)
                {
                    OnUpdate(Easer.Apply(m_EaseType, 1));

                    // the onstart and oncomplete callbacks will be invoked anyway.
                    m_Started?.Invoke();
                    m_Completed?.Invoke();

                    Stop();
                }
                else
                {
                    OnUpdate(Easer.Apply(m_EaseType, 0));

                    m_Started?.Invoke();
                }
            }
        }

        /// <summary>
        /// During the monobehaviour update cycle, the logic will be performed.
        /// </summary>
        public void Update()
        {
            // Increase or decrease the time of the tween based on the direction.
            var delta = Time.deltaTime / m_Duration;

            m_Time += m_IsPlayingForward ? delta : -delta;

            // The time will be capped to 1, when pingpong is enabled the tween will
            // play backwards, otherwise when the tween is not infinite, didReachEnd
            // will be set to true.
            if (m_Time > 1)
            {
                m_Time = 1;

                if (m_HasPingPong)
                    m_IsPlayingForward = false;
                else
                    m_DidTimeReachEnd = true;
            }
            else if (m_HasPingPong && m_Time < 0)
            {
                m_Time = 0;
                m_IsPlayingForward = true;

                m_DidTimeReachEnd = true;
            }

            // The time will be updated on the inheriter.
            OnUpdate(Easer.Apply(m_EaseType, m_Time));

            // When the end is reached either the loop count will be decreased, or
            // the tween will be marked as completed and will be decommissioned, 
            // the oncomplete may be invoked.
            if (m_DidTimeReachEnd)
            {
                if (m_CurrentLoopCount > 1)
                {
                    m_DidTimeReachEnd = false;
                    m_Time = 0f;
                    m_CurrentLoopCount--;
                }
                else
                {
                    m_Completed?.Invoke();
                    Stop();
                }
            }
        }

        /// <summary>
        /// Cancel the tween and decommision the component.
        /// </summary>
        public void Cancel()
        {
            if (!IsPlaying)
                return;

            m_Cancelled?.Invoke();
            Stop();
        }

        /// <summary>
        /// Disables/Destroys the component.
        /// </summary>
        private void Stop()
        {
            if (IsPlaying)
                TweenManager.RemoveTween(this);

            IsPlaying = false;
        }

        /// <summary>
        /// Returns the interpolated value of time between from and to.
        /// </summary>
        /// <param name="from">The value to interpolate from.</param>
        /// <param name="to">The value to interpolate to.</param>
        /// <param name="time">The time of the interpolation.</param>
        /// <returns>The interpolated value.</returns>
        protected float InterpolateValue(float from, float to, float time) => from * (1 - time) + to * time;

        /// <summary>
        /// Sets the target value of this tween and its duration.
        /// </summary>
        /// <param name="valTo">The target value.</param>
        /// <param name="duration">The duration of the tween.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetTargetValue(DriverValueType valTo, float duration)
        {
            m_Duration = duration;
            return SetTargetValue(valTo);
        }

        /// <summary>
        /// Sets the duration of this Tween.
        /// </summary>
        /// <param name="duration">The duration of the tween.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetDuration(float duration)
        {
            m_Duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the target value of this tween.
        /// </summary>
        /// <param name="valTo">The target value.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetTargetValue(DriverValueType valTo)
        {
            m_ValueTo = valTo;

            if (m_Time > 0.001f)
                m_ValueFrom = currentValue;

            RecalculateValueTo();
            m_Time = 0f;

            return this;
        }

        /// <summary>
        /// Sets the value tween should animate from instead of its current.
        /// </summary>
        /// <param name="valFrom">The from value.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetFrom(DriverValueType valFrom)
        {
            m_IsFromLocked = true;
            m_ValueFrom = valFrom;

            return this;
        }

        /// <summary>
        /// Binds an onStart event which will be invoked when the tween starts.
        /// </summary>
        /// <param name="onStart">The start callback.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetStartCallback(UnityAction onStart)
        {
            m_Started = onStart;
            return this;
        }

        /// <summary>
        /// Binds an onComplete event which will be invoked when the tween ends.
        /// </summary>
        /// <param name="onComplete">The completion callback.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetCompleteCallback(UnityAction onComplete)
        {
            m_Completed = onComplete;
            return this;
        }

        /// <summary>
        /// Binds an onCancel event which will be invoked when the tween is canceled.
        /// </summary>
        /// <param name="onCancel">The cancelation callback.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetCancelCallback(UnityAction onCancel)
        {
            m_Cancelled = onCancel;
            return this;
        }

        /// <summary>
        /// Sets the loop type to ping pong, will bounce the animation back
        /// and forth endlessly. When a loop count is set, the tween has play forward
        /// and backwards to count as one cycle.
        /// </summary>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetPingPong()
        {
            m_HasPingPong = true;
            return this;
        }

        /// <summary>
        /// Sets the loop count of tween until it can be decomissioned.
        /// </summary>
        /// <param name="loopCount">The target number of loops.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetLoopCount(int loopCount)
        {
            m_CurrentLoopCount = loopCount;
            return this;
        }

        /// <summary>
        /// Sets tween to infinite.
        /// </summary>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetInfinite()
        {
            m_CurrentLoopCount = int.MaxValue;
            return this;
        }

        /// <summary>
        /// Sets the delay of this tween. The tween will not play anything until
        /// the requested delay time is reached. You can change behaviour by
        /// enabeling 'goToFirstFrameImmediately' to set the animation to the first
        /// frame immediately.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="goToFirstFrameImmediately">Go to the first frame immediately</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetDelay(float delay)
        {
            m_Delay = delay;
            return this;
        }

        /// <summary>
        /// Sets the time of the tween (0-1).
        /// </summary>
        /// <param name="time"></param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetTime(float time)
        {
            m_Time = Mathf.Clamp01(time);
            return this;
        }

        /// <summary>
        /// Sets the ease for tween.
        /// </summary>
        /// <param name="ease">The target ease type.</param>
        /// <returns>The current Tween.</returns>
        public Tween<DriverValueType> SetEase(EaseType ease)
        {
            m_EaseType = ease;
            return this;
        }

        /// <summary>
        /// Gets the total duration of the tween including the loop count and
        /// ping pong settings, and the delay optionally.
        /// </summary>
        /// <param name="includeDelay">Include delay in calculation?</param>
        /// <returns>The total duration of the Tween.</returns>
        public float GetTotalDuration(bool includeDelay = false)
        {
            var duration = m_Duration;

            if (m_CurrentLoopCount > 0)
                duration *= m_CurrentLoopCount;

            if (m_HasPingPong)
                duration *= 2f;

            if (includeDelay && m_Delay > 0f)
                duration += m_Delay;

            return duration;
        }

        /// <summary>
        /// Returns a Task which awaits the Tween's completion or decommissioning.
        /// </summary>
        /// <returns>An awaitable Task.</returns>
        public async Task Await()
        {
            while (IsPlaying)
            {
#if UNITY_EDITOR
                // It is possible for the Application to stop playing in the middle of a
                // Tween. Usually is no problem, since the application is stopped
                // anyway. But when in the editor, the runtime is still running, so to
                // avoid any issues, we stop when the application is no longer playing.
                if (Application.isPlaying == false)
                    return;
#endif
                await Task.Yield();
            }
        }

        /// <summary>
        /// Returns an enumerator which yields the Tween's completion or decommissioning.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator Yield()
        {
            while (IsPlaying)
                yield return 0;
        }

        /// <summary>
        /// Adds a Tween Driver.
        /// </summary>
        /// <typeparam name="Driver">The type of the Driver.</typeparam>
        /// <param name="gameObject">The target Game Object.</param>
        /// <returns>The instantiated Driver.</returns>
        internal static Driver Add<Driver>() where Driver : Tween<DriverValueType>, new()
        {
            Driver driver = new Driver();
            return driver;
        }
    }
}
