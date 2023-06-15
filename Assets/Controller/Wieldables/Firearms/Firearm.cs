using IdenticalStudios.InventorySystem;
using IdenticalStudios.WieldableSystem.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    [AddComponentMenu("IdenticalStudios/Wieldables/Firearms/Firearm")]
    public sealed class Firearm : Wieldable, IFirearm, IUseInputHandler, IAimInputHandler, IReloadInputHandler
    {
        public IAimer Aimer { get; private set; }
        public ITrigger Trigger { get; private set; }
        public IShooter Shooter { get; private set; }
        public IAmmo Ammo { get; private set; }
        public IReloader Reloader { get; private set; }
        public IRecoil Recoil { get; private set; }
        public IProjectileEffect ProjectileEffect { get; private set; }

        public bool IsReloading => Reloader.IsReloading;
        public bool IsAiming => Aimer.IsAiming;
        public float HeatValue => m_HeatValue;

        public event UnityAction AttachmentChanged;
        public event UnityAction<IAimer> AimerChanged;
        public event UnityAction<ITrigger> TriggerChanged;
        public event UnityAction<IShooter> ShooterChanged;
        public event UnityAction<IAmmo> AmmoChanged;
        public event UnityAction<IReloader> ReloaderChanged;
        public event UnityAction<IRecoil> RecoilChanged;
        public event UnityAction<IProjectileEffect> ProjectileEffectChanged;
        
        [SerializeField]
        [Tooltip("Ammo in magazine corresponding property.")]
        private DataIdReference<ItemPropertyDefinition> m_AmmoProperty;

        [SerializeField]
        private EffectCollection m_DryFireEffects;

        [SerializeField]
        private EffectCollection m_StopFireEffects;

        [Title("Attachments")]

        [SerializeField, NotNull("This attachment has to be assigned (Press on ''Setup Firearm'' to add a default attachment.")]
        private FirearmAimerBehaviour m_BaseAimer;

        [SerializeField, NotNull("This attachment has to be assigned (Press on ''Setup Firearm'' to add a default attachment.")]
        private FirearmTriggerBehaviour m_BaseTrigger;

        [SerializeField, NotNull("This attachment has to be assigned (Press on ''Setup Firearm'' to add a default attachment.")]
        private FirearmShooterBehaviour m_BaseShooter;

        [SerializeField, NotNull("This attachment has to be assigned (Press on ''Setup Firearm'' to add a default attachment.")]
        private FirearmAmmoBehaviour m_BaseAmmo;

        [SerializeField, NotNull("This attachment has to be assigned (Press on ''Setup Firearm'' to add a default attachment.")]
        private FirearmReloaderBehaviour m_BaseReloader;

        [SerializeField, NotNull("This attachment has to be assigned (Press on ''Setup Firearm'' to add a default attachment.")]
        private FirearmRecoilBehaviour m_BaseRecoil;

        [SerializeField, NotNull("This attachment has to be assigned (Press on ''Setup Firearm'' to add a default attachment.")]
        private FirearmProjectileEffectBehaviour m_BaseProjectileEffect;

        private float m_HeatValue;
        private bool m_PlayStopFireEffects;
        private float m_NextTimeCanUpdateHeatValue;
        private IItemProperty m_DurabilityProperty;
        private IItemProperty m_AmmoInMagazineProperty;

        
        public override void OnHolster(float holsterSpeed)
        {
            base.OnHolster(holsterSpeed);

            m_PlayStopFireEffects = false;

            // Release trigger if held
            if (Trigger.IsTriggerHeld)
                Trigger.ReleaseTrigger();

            // End aim if active
            EndAiming();

            // Cancel reload if active
            CancelReloading();
        }

        private void Shoot(float triggerValue)
        {
            if (m_DurabilityProperty != null && m_DurabilityProperty.Float < 0.001f)
                return;
            
            bool canShoot = !IsReloading && Reloader.TryUseAmmo(Shooter.AmmoPerShot);

            // If the firearm has enough ammo in the magazine, shoot
            if (canShoot)
            {
                if (Aimer.IsAiming && !GameplaySettings.Instance.CanAimWhileShooting)
                    Aimer.EndAim();

                Shooter.Shoot(Accuracy, ProjectileEffect, triggerValue);

                m_PlayStopFireEffects = true;
                m_NextTimeCanUpdateHeatValue = Time.unscaledTime + 0.1f;

                m_HeatValue = Mathf.Clamp01(m_HeatValue + (1 / (float)Mathf.Clamp(Reloader.MagazineSize, 0, 30)));
                Recoil.DoRecoil(Aimer.IsAiming, m_HeatValue, triggerValue);

                m_ShootInaccuracy += Aimer.IsAiming ? Recoil.AimAccuracyKick : Recoil.HipfireAccuracyKick;
            }
        }

        protected override void FixedUpdate()
        {
            Accuracy = GetAccuracy();

            if (m_NextTimeCanUpdateHeatValue < Time.unscaledTime)
            {
                float heatRecoverDelta = Time.fixedUnscaledDeltaTime * Recoil.RecoilHeatRecover;
                m_HeatValue = Mathf.Clamp01(m_HeatValue - (Mathf.Max(heatRecoverDelta * m_HeatValue * 5f, heatRecoverDelta)));

                if (m_PlayStopFireEffects)
                {
                    m_StopFireEffects.PlayEffects(this);
                    m_PlayStopFireEffects = false;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (TryGetComponent(out IWieldableItem wieldableItem))
                wieldableItem.AttachedItemChanged += OnItemAttached;

            AimBlocker.SetBlockCallback(EndAiming);
            ReloadBlocker.SetBlockCallback(CancelReloading);
        }

        private void OnItemAttached(IItem item)
        {
            if (item != null)
            {
                Weight = item.Definition.Weight;
                m_DurabilityProperty = item.GetPropertyWithName("Durability");
                m_AmmoInMagazineProperty = item.GetPropertyWithId(m_AmmoProperty);

                // Load the current 'ammo in magazine count' that's saved in one of the properties on the given item.
                if (m_AmmoInMagazineProperty != null)
                    Reloader.AmmoInMagazine = m_AmmoInMagazineProperty.Integer;
            }
            else
            {
                Weight = 1f;
                m_DurabilityProperty = null;
                m_AmmoInMagazineProperty = null;
            }
        }

        private void OnAmmoInMagazineChanged(int prevAmmo, int ammo)
        {
            if (m_AmmoInMagazineProperty != null)
                m_AmmoInMagazineProperty.Integer = ammo;
        }

        #region Attachments
        public void SetTrigger(ITrigger trigger)
        {
            var prevTrigger = Trigger;
            Trigger = trigger ?? m_BaseTrigger;

            if (prevTrigger != Trigger)
            {
                bool wasHeld = false;
                
                if (prevTrigger != null)
                {
                    wasHeld = prevTrigger.IsTriggerHeld;
                    prevTrigger.Shoot -= Shoot;
                    prevTrigger.Detach();
                }

                Trigger.Shoot += Shoot;
                Trigger.Attach();
                
                if (wasHeld)
                    Trigger.HoldTrigger();
                
                TriggerChanged?.Invoke(Trigger);
                AttachmentChanged?.Invoke();
            }
        }

        public void SetReloader(IReloader reloader)
        {
            var prevReloader = Reloader;
            Reloader = reloader ?? m_BaseReloader;

            if (prevReloader != Reloader)
            {
                if (prevReloader != null)
                {
                    prevReloader.Detach();
                    prevReloader.AmmoInMagazineChanged -= OnAmmoInMagazineChanged;
                }

                Reloader.Attach();
                Reloader.AmmoInMagazineChanged += OnAmmoInMagazineChanged;

                ReloaderChanged?.Invoke(Reloader);
                AttachmentChanged?.Invoke();
            }
        }

        public void SetAimer(IAimer aimer)
        {
            var prevAimer = Aimer;
            Aimer = aimer ?? m_BaseAimer;

            if (prevAimer != Aimer)
            {
                if (prevAimer != null)
                {
                    prevAimer.EndAim();
                    prevAimer.Detach();
                }

                Aimer.Attach();
                
                AimerChanged?.Invoke(Aimer);
                AttachmentChanged?.Invoke();
            }
        }

        public void SetShooter(IShooter shooter)
        {
            var prevShooter = Shooter;
            Shooter = shooter ?? m_BaseShooter;

            if (prevShooter != Shooter)
            {
                prevShooter?.Detach();
                Shooter.Attach();
                
                ShooterChanged?.Invoke(Shooter);
                AttachmentChanged?.Invoke();
            }
        }

        public void SetAmmo(IAmmo ammo)
        {
            var prevAmmo = Ammo;
            Ammo = ammo ?? m_BaseAmmo;

            if (prevAmmo != Ammo)
            {
                prevAmmo?.Detach();
                Ammo.Attach();
                
                AmmoChanged?.Invoke(Ammo);
                AttachmentChanged?.Invoke();
            }
        }

        public void SetRecoil(IRecoil recoil)
        {
            var prevRecoil = Recoil;
            Recoil = recoil ?? m_BaseRecoil;

            if (prevRecoil != Recoil)
            {
                prevRecoil?.Detach();
                Recoil.Attach();
                
                RecoilChanged?.Invoke(Recoil);
                AttachmentChanged?.Invoke();
            }
        }

        public void SetBulletEffect(IProjectileEffect projectileEffect)
        {
            var prevBulEffect = ProjectileEffect;
            ProjectileEffect = projectileEffect ?? m_BaseProjectileEffect;

            if (prevBulEffect != ProjectileEffect)
            {
                prevBulEffect?.Detach();
                ProjectileEffect.Attach();
                
                ProjectileEffectChanged?.Invoke(ProjectileEffect);
                AttachmentChanged?.Invoke();
            }
        }
        #endregion

        #region Input Handling
        public void Use(UsePhase usePhase)
        {
            if (usePhase == UsePhase.Start)
            {
                // Try reload on dry fire.
                if (Reloader.IsMagazineEmpty && GameplaySettings.Instance.AutoReloadOnDry)
                {
                    StartReloading();

                    if (IsReloading)
                        return;
                }
                
                // Try Cancel reload.
                if (IsReloading)
                {
                    CancelReloading();

                    if (!IsReloading)
                        return;
                }
                
                // Play the dry fire effects.
                if (Reloader.IsMagazineEmpty)
                {
                    m_DryFireEffects.PlayEffects(this);
                    return;
                }
            }

            bool canHoldTrigger = !UseBlocker.IsBlocked && !IsEquipping() && usePhase != UsePhase.End;
            if (canHoldTrigger)
                Trigger.HoldTrigger();
            else
                Trigger.ReleaseTrigger();
        }

        public void StartAiming()
        {
            bool canAim = !AimBlocker.IsBlocked && !IsEquipping() &&
                          (GameplaySettings.Instance.CanAimWhileReloading || !IsReloading);

            if (canAim)
                Aimer.StartAim();
        }

        public void EndAiming() => Aimer?.EndAim();

        public void StartReloading()
        {
            bool reloaded = !ReloadBlocker.IsBlocked && !IsEquipping() && Reloader.TryStartReload(Ammo);

            if (reloaded && IsAiming && !GameplaySettings.Instance.CanAimWhileReloading)
                EndAiming();
        }

        public void CancelReloading()
        {
            if (GameplaySettings.Instance.CancelReloadOnShoot)
            {
                if (!IsEquipping() && Reloader.TryCancelReload(Ammo, out var endDuration))
                    EquipOrHolsterTime = Time.time + endDuration + 0.05f;
            }
        }
        #endregion

        #region Crosshair
        private float m_ShootInaccuracy;
        private float m_BaseAccuracy = 1f;


        protected override float GetAccuracy()
        {
            float deltaTime = Time.fixedUnscaledDeltaTime;

            m_BaseAccuracy = AccuracyHandler.GetAccuracyMod() * (Aimer.IsAiming ? Aimer.AimAccuracyMod : Aimer.HipAccuracyMod);
            float targetAccuracy = Mathf.Clamp01(m_BaseAccuracy - m_ShootInaccuracy);
            
            float accuracyRecoverDelta = deltaTime * (Aimer.IsAiming ? Recoil.AimAccuracyRecover : Recoil.HipfireAccuracyRecover);
            m_ShootInaccuracy = Mathf.Clamp01(m_ShootInaccuracy - accuracyRecoverDelta);

            return targetAccuracy;
        }
        #endregion

        #region Action Blocking

        public ActionBlockHandler UseBlocker { get; } = new ActionBlockHandler();
        public ActionBlockHandler AimBlocker { get; } = new ActionBlockHandler(); 
        public ActionBlockHandler ReloadBlocker { get; } = new ActionBlockHandler();

        #endregion

        #region Editor
#if UNITY_EDITOR
        private void Reset()
        {
            UnityUtils.SafeOnValidate(this, () =>
            {
                if (ItemPropertyDefinition.TryGetWithName("Ammo In Magazine", out var def))
                    m_AmmoProperty = def.Id;
            });

            // Add the default components (can be removed).
            gameObject.AddComponent<CharacterUseBlocker>();
            gameObject.AddComponent<CharacterAimBlocker>();
            gameObject.AddComponent<CharacterReloadBlocker>();
        }
#endif
        #endregion
    }
}
