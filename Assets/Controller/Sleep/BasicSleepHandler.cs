using IdenticalStudios.InputSystem;
using IdenticalStudios.ProceduralMotion;
using IdenticalStudios.UISystem;
using IdenticalStudios.WorldManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios
{
    public sealed class BasicSleepHandler : CharacterBehaviour, ISleepHandler, ISaveableComponent
    {
        #region
        [System.Serializable]
        private class SleepEvent : UnityEvent<GameTime> { }
        #endregion

        public Vector3 LastSleepPosition { get; private set; } = Vector3.zero;
        public Quaternion LastSleepRotation { get; private set; } = Quaternion.identity;

        public bool SleepActive { get; private set; }

        public event UnityAction<GameTime> SleepStart
        {
            add => m_OnSleepStart.AddListener(value);
            remove => m_OnSleepStart.RemoveListener(value);
        }

        public event UnityAction<GameTime> SleepEnd
        {
            add => m_OnSleepEnd.AddListener(value);
            remove => m_OnSleepEnd.RemoveListener(value);
        }

        [SerializeField]
        private InputContextGroup m_SleepContext;


        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time it takes to transition to sleeping (e.g. moving to bed).")]
        private float m_GoToSleepDuration = 2f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time it takes to fall asleep.")]
        private float m_SleepDelay = 1.5f;

        [SerializeField, Range(0, 60)]
        [Tooltip("Sleep duration in seconds")]
        private float m_SleepDuration;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time to wait after the sleep is done, before getting up.")]
        private float m_WakeUpDelay = 1.5f;

        [SerializeField, Range(0f, 10f)]
        [Tooltip("How much time it takes to transition from sleeping to standing up.")]
        private float m_WakeUpDuration = 2f;


        [SerializeField, Range(0, 24)]
        [Tooltip("Max hours that can pass while sleeping")]
        private int m_HoursToSleep = 8;

        [SerializeField, Range(0, 24)]
        [Tooltip("Max hour this character can wake up at, we don't want to be lazy :)")]
        private int m_MaxGetUpHour = 8;

        //Conditions

        [SerializeField]
        [Tooltip("If Enabled, this character will not be allowed to sleep during the day.")]
        private bool m_OnlySleepAtNight = true;


        [SerializeField]
        [Tooltip("Check for enemies before sleeping, if any of the are found, this character will be unable to sleep.")]
        private bool m_CheckForEnemies;

        [SerializeField]
        [Tooltip("The enemy check radius.")]
        private float m_CheckForEnemiesRadius;

        [SerializeField]
        [Tooltip("The enemy layer, any object with this layer will be considered an enemy.")]
        private LayerMask m_EnemiesLayerMask;

        //Audio

        [SerializeField]
        [Tooltip("Sound that will be played when attempting to sleep unsuccessfully.")]
        private StandardSound m_CantSleepSound;

        //Animation
        
        [SerializeField]
        private Transform m_Camera;

        [SerializeField]
        private EaseType m_EaseType;
        
        //Events

        [SerializeField]
        private SleepEvent m_OnSleepStart;

        [SerializeField]
        private SleepEvent m_OnSleepEnd;


        public void Sleep(ISleepingPlace sleepingPlace)
        {
            if (SleepActive) return;

            if (m_OnlySleepAtNight && WorldManagerBase.Instance.GetTimeOfDay() == TimeOfDay.Day)
            {
                MessageDisplayerUI.PushMessage("You can only sleep at night!", Color.red);
                Character.AudioPlayer.PlaySound(m_CantSleepSound);
                return;
            }

            if (m_CheckForEnemies && HasEnemiesNearby(sleepingPlace.gameObject.transform))
            {
                MessageDisplayerUI.PushMessage("Can't sleep with enemies nearby!", Color.red);
                Character.AudioPlayer.PlaySound(m_CantSleepSound);
                return;
            }

            StartCoroutine(C_Sleep(sleepingPlace));
        }

        private bool HasEnemiesNearby(Transform checkPoint)
        {
            int size = PhysicsUtils.OverlapSphereNonAlloc(checkPoint.position, m_CheckForEnemiesRadius, out var cols, m_EnemiesLayerMask);

            for (int i = 0; i < size; i++)
            {
                if (Character.HasCollider(cols[i]))
                    continue;
                
                if (cols[i].GetComponent<CharacterHitbox>() != null)
                    return true;
            }
            
            return false;
        }

        private IEnumerator C_Sleep(ISleepingPlace sleepingPlace)
        {
            // Holster active wiedable
            GetModule<IWieldablesController>().TryEquipWieldable(null, 1.3f);

            var originalRotation = m_Camera.eulerAngles;
            var targetPosition = m_Camera.InverseTransformPoint(sleepingPlace.SleepPosition);
            var targetRotation = sleepingPlace.SleepRotation;

            // Play the sleep animation
            PlayCameraAnimation(targetPosition, targetRotation, m_GoToSleepDuration);

            InputManager.PushContext(m_SleepContext);

            yield return new WaitForSeconds(m_GoToSleepDuration + m_SleepDelay);

            int hoursToSleep = GetHoursToSleep();
            GameTime timeToSleep = new(hoursToSleep, 0f, 0f);
            WorldManagerBase.Instance.PassTime(hoursToSleep / 24f, m_SleepDuration);

            SleepActive = true;
            m_OnSleepStart.Invoke(timeToSleep);

            float passedTime = 0f;
            while (passedTime <= m_SleepDuration)
            {
                passedTime += Time.deltaTime;
                yield return null;
            }

            LastSleepPosition = Character.transform.position;

            yield return new WaitForSeconds(m_WakeUpDelay);

            SleepActive = false;
            m_OnSleepEnd?.Invoke(timeToSleep);

            // Play the wake up animation
            PlayCameraAnimation(Vector3.zero, originalRotation, m_WakeUpDuration);

            yield return new WaitForSeconds(m_WakeUpDuration);

            InputManager.PopContext(m_SleepContext);
        }

        private int GetHoursToSleep() 
        {
            int hoursToSleep;
            int currentHour = (int)(WorldManagerBase.Instance.GetNormalizedTime() * 24);

            if (currentHour <= 24 && currentHour > 12)
                hoursToSleep = 24 - currentHour + m_MaxGetUpHour;
            else if (currentHour < 12 && currentHour <= m_MaxGetUpHour)
                hoursToSleep = m_MaxGetUpHour - currentHour;
            else
                hoursToSleep = m_HoursToSleep;

            hoursToSleep = Mathf.Clamp(hoursToSleep, 0, m_HoursToSleep);

            return hoursToSleep;
        }
        
        private void PlayCameraAnimation(Vector3 targetPosition, Vector3 targetRotation, float duration)
        {
            m_Camera.TweenLocalPosition(targetPosition, duration).SetFrom(m_Camera.localPosition).SetEase(m_EaseType).Play();
            m_Camera.TweenRotation(targetRotation, duration).SetEase(m_EaseType).Play();
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            LastSleepPosition = (Vector3)members[0];
            LastSleepRotation = (Quaternion)members[1];
        }

        public object[] SaveMembers()
        {
            return new object[]
            {
                LastSleepPosition,
                LastSleepRotation
            };
        }
        #endregion
    }
}