using IdenticalStudios.ProceduralMotion;
using IdenticalStudios.WorldManagement;
using UnityEngine;

namespace IdenticalStudios
{
    public class CharacterSleepEffects : CharacterBehaviour
    {
        //Sleep Stats Change

        [SerializeField, Range(0, 100f)]
        private float m_HealthIncreasePerHour;

        [SerializeField, Range(0, 100f)]
        private float m_EnergyIncreasePerHour;

        //Camera Settings

        [SerializeField]
        private CameraEffectSettings m_SleepEffects;

        [SerializeField]
        private BobMotionData m_CameraBob;

        //Audio Settings

        [SerializeField, ReorderableList(Foldable = true)]
        private DelayedSound[] m_SleepingSounds;

        [SerializeField, ReorderableList(Foldable = true)]
        private DelayedSound[] m_GetUpSounds;

        private ICameraMotionHandler m_CameraMotion;
        private ICameraEffectsHandler m_CameraEffects;
        private ISleepHandler m_SleepHandler;
        private IEnergyManager m_EnergyManager;


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_CameraMotion);
            GetModule(out m_CameraEffects);
            GetModule(out m_EnergyManager);
            GetModule(out m_SleepHandler);

            m_SleepHandler.SleepStart += OnSleepStart;
            m_SleepHandler.SleepEnd += OnSleepEnd;
        }

        private void OnSleepStart(GameTime timeToSleep) 
        {
            m_CameraEffects.DoAnimationEffect(m_SleepEffects);
            m_CameraMotion.DataHandler.AddDataOverride(m_CameraBob);
            Character.AudioPlayer.PlaySounds(m_SleepingSounds);
        }

        private void OnSleepEnd(GameTime timeSlept)
        {
            int hoursSlept = timeSlept.Hours;

            if (hoursSlept > 0)
            {
                Character.HealthManager.RestoreHealth(hoursSlept * m_HealthIncreasePerHour);
                m_EnergyManager.Energy += hoursSlept * m_EnergyIncreasePerHour;
            }

            m_CameraMotion.DataHandler.RemoveDataOverride(m_CameraBob);
            Character.AudioPlayer.PlaySounds(m_GetUpSounds);
        }
    }
}