using IdenticalStudios.WieldableSystem.Effects;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public class Wieldable : MonoBehaviour, IWieldable, ICrosshairHandler, IWeightHandler
    {
        private ICharacter m_Character;
        public ICharacter Character
        {
            get
            {
                if (m_Character == null)
                    m_Character = gameObject.GetComponentInRoot<ICharacter>();

                return m_Character;
            }
            set
            {
                if (value == null || m_Character == value)
                    return;

                m_Character = value;
                OnCharacterChanged(value);
            }
        }

        private IAudioPlayer m_AudioPlayer;
        public IAudioPlayer AudioPlayer
        {
            get => m_AudioPlayer;
            set
            {
                if (value != null)
                    m_AudioPlayer = value;
            }
        }

        private IAccuracyHandler m_AccuracyHandler;
        public IAccuracyHandler AccuracyHandler
        {
            get => m_AccuracyHandler;
            set
            {
                if (value != null)
                    m_AccuracyHandler = value;
            }
        }

        public float EquipDuration => m_EquipDuration;
        public float HolsterDuration => m_HolsterDuration;

        public bool IsVisible { get; private set; }
        public float EquipOrHolsterTime { get; protected set; }

        public event UnityAction EquippingStarted;
        public event UnityAction HolsteringEnded;
        

        [SerializeField, Range(-1, 100)]
        [Tooltip("Crosshair index for this wieldable, -1 or lower will result in no crosshair")]
        private int m_BaseCrosshair;


        [SerializeField, Range(0f, 5f)]
        private float m_EquipDuration = 0.5f;

        [SerializeField]
        private EffectCollection m_EquipEffects = new(new[]
        {
            new AnimationWieldableEffect(
                new AnimatorParameterTrigger(AnimatorControllerParameterType.Float, "EquipSpeed", 1f),
                new AnimatorParameterTrigger(AnimatorControllerParameterType.Trigger, "Equip", 1f))
        });



        [SerializeField, Range(0f, 5f)]
        private float m_HolsterDuration = 0.5f;

        [SerializeField]
        private DynamicEffectCollection m_HolsterEffects = new(null, new[]
        {
            new AnimationWieldableEffect(
                new AnimatorParameterTrigger(AnimatorControllerParameterType.Float, "HolsterSpeed", 1f),
                new AnimatorParameterTrigger(AnimatorControllerParameterType.Trigger, "Holster", 1f))
        });



        public virtual void SetVisibility(bool visible)
        {
            bool wasVisible = IsVisible;
            IsVisible = visible;
            gameObject.SetActive(visible);

            if (wasVisible != visible)
            {
                if (!visible)
                    HolsteringEnded?.Invoke(); 
            }
        }

        public virtual void OnEquip()
        {
            EquippingStarted?.Invoke();

            EquipOrHolsterTime = Time.time + m_EquipDuration;
            m_EquipEffects.PlayEffects(this);
        }

        public virtual void OnHolster(float holsterSpeed)
        {
            EquipOrHolsterTime = Time.time + m_HolsterDuration * 2f;
            AudioPlayer.ClearAllQueuedSounds();
            m_HolsterEffects.PlayEffects(this, holsterSpeed);
        }

        protected virtual void OnCharacterChanged(ICharacter character) { }
        
        protected virtual void FixedUpdate() => Accuracy = GetAccuracy();
        protected virtual void Awake()  => m_CurrentCrosshairIndex = m_BaseCrosshair;

        protected bool IsEquipping() => EquipOrHolsterTime > Time.time;

        #region Crosshair Handling
        private float m_Accuracy = 1f;
        public virtual float Accuracy
        {
            get => m_Accuracy;
            protected set
            {
                float val = Mathf.Clamp01(value);
                if (Math.Abs(m_Accuracy - val) > 0.001f)
                {
                    m_Accuracy = val;
                    AccuracyChanged?.Invoke(m_Accuracy);
                }
            }
        }

        private int m_CurrentCrosshairIndex;
        public int CrosshairIndex
        {
            get => m_CurrentCrosshairIndex;
            set
            {
                if (m_CurrentCrosshairIndex == value)
                    return;
                
                m_CurrentCrosshairIndex = value;
                CrosshairChanged?.Invoke(value);
            }
        }

        public event UnityAction<int> CrosshairChanged;
        public event UnityAction<float> AccuracyChanged;

        
        public void ResetCrosshair() => CrosshairIndex = m_BaseCrosshair;
        protected virtual float GetAccuracy() => AccuracyHandler != null ? AccuracyHandler.GetAccuracyMod() : 1f;
        #endregion

        #region Weight Handling
        private float m_CurrentWeight;
        public float Weight
        {
            get => m_CurrentWeight;
            protected set
            {
                if (Math.Abs(m_CurrentWeight - value) < 0.001f)
                    return;
                
                m_CurrentWeight = value;
            }
        }   
        #endregion
    }
}
