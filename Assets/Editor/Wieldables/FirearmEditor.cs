using UnityEditor;
using UnityEngine;

namespace IdenticalStudios.WieldableSystem
{
    [CustomEditor(typeof(Firearm))]
    public class FirearmEditor : WieldableEditor
    {
        private Firearm m_Firearm;

        private SerializedProperty m_Aimer;
        private SerializedProperty m_Ammo;
        private SerializedProperty m_Recoil;
        private SerializedProperty m_Reloader;
        private SerializedProperty m_Shooter;
        private SerializedProperty m_Trigger;
        private SerializedProperty m_ProjectileEffect;

        private static bool m_DebugModeEnabled;


        protected override void DrawCustomInspector()
        {
            base.DrawCustomInspector();

            if (!Application.isPlaying)
            {
                if (!HasBaseModules())
                {
                    CustomGUILayout.Separator();

                    if (GUILayout.Button("Setup Firearm"))
                        SetupBaseModules();
                }

                if (m_Firearm != null)
                    SetModulesActivation();
            }
            else
            {
                EditorGUILayout.Space();
                
                GUI.backgroundColor = CustomGUIStyles.BlueColor;
                m_DebugModeEnabled = GUILayout.Toggle(m_DebugModeEnabled, "Debug Mode", "Button");
                GUI.backgroundColor = Color.white;

                if (m_DebugModeEnabled)
                    DrawFirearmDebug();
            }
        }

        private void DrawFirearmDebug()
        {
            GUI.enabled = false;
            DrawDebugLabel($"Is Aiming: {m_Firearm.Aimer.IsAiming}");
            DrawDebugLabel($"Is Reloading: {m_Firearm.Reloader.IsReloading}");
            DrawDebugLabel($"Ammo In Magazine: {m_Firearm.Reloader.AmmoInMagazine}");
            DrawDebugLabel($"Is Magazine Empty: {m_Firearm.Reloader.IsMagazineEmpty}");
            DrawDebugLabel($"Is Magazine Full: {m_Firearm.Reloader.IsMagazineFull}");
            DrawDebugLabel($"Is Trigger Held: {m_Firearm.Trigger.IsTriggerHeld}");
            DrawDebugLabel($"Current Accuracy: {m_Firearm.Accuracy}");
            DrawDebugLabel($"Current Heat Value: {m_Firearm.HeatValue}");
            DrawDebugLabel($"Current Weight: {m_Firearm.Weight}");
            Repaint();
            GUI.enabled = true;
        }

        private void DrawDebugLabel(string label)
        {
            GUILayout.Space(3f);
            GUILayout.Label(label);
            GUILayout.Space(3f);
        }
        
        private bool HasBaseModules()
        {
            bool hasAllBaseModules = m_Aimer.objectReferenceValue;
            hasAllBaseModules &= m_Ammo.objectReferenceValue;
            hasAllBaseModules &= m_Recoil.objectReferenceValue;
            hasAllBaseModules &= m_Reloader.objectReferenceValue;
            hasAllBaseModules &= m_Shooter.objectReferenceValue;
            hasAllBaseModules &= m_Trigger.objectReferenceValue;
            hasAllBaseModules &= m_ProjectileEffect.objectReferenceValue;

            return hasAllBaseModules;
        }

        private void SetupBaseModules()
        {
            SetupModule<FirearmBasicAimer, FirearmAimerBehaviour>(m_Aimer);
            SetupModule<FirearmInfiniteAmmo, FirearmAmmoBehaviour>(m_Ammo);
            SetupModule<FirearmPatternRecoil, FirearmRecoilBehaviour>(m_Recoil);
            SetupModule<FirearmBasicReloader, FirearmReloaderBehaviour>(m_Reloader);
            SetupModule<FirearmHitscanShooter, FirearmShooterBehaviour>(m_Shooter);
            SetupModule<FirearmSemiAutoTrigger, FirearmTriggerBehaviour>(m_Trigger);
            SetupModule<FirearmBasicProjectileEffect, FirearmProjectileEffectBehaviour>(m_ProjectileEffect);

            serializedObject.ApplyModifiedProperties();
        }

        private void SetupModule<TNewType, TCurrentModule>(SerializedProperty property) where TNewType : FirearmAttachmentBehaviour where TCurrentModule : FirearmAttachmentBehaviour
        {
            if (property.objectReferenceValue != null)
                return;

            TCurrentModule currentModule = m_Firearm.gameObject.GetComponent<TCurrentModule>();

            if (currentModule == null)
                property.objectReferenceValue = m_Firearm.gameObject.AddComponent<TNewType>();
            else
                property.objectReferenceValue = currentModule;
        }

        private void SetModulesActivation()
        {
            foreach (var module in m_Firearm.GetComponentsInChildren<FirearmAttachmentBehaviour>(true))
            {
                module.AttachOnStart = false;

                if (IsBaseModule(module))
                    module.AttachOnStart = true;
            }
        }

        private bool IsBaseModule(FirearmAttachmentBehaviour module)
        {
            var isBaseModule = module == m_Aimer.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Ammo.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Recoil.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Reloader.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Shooter.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Trigger.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_ProjectileEffect.objectReferenceValue as FirearmAttachmentBehaviour;

            return isBaseModule;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Firearm = wieldable as Firearm;

            m_Aimer = serializedObject.FindProperty("m_BaseAimer");
            m_Ammo = serializedObject.FindProperty("m_BaseAmmo");
            m_Recoil = serializedObject.FindProperty("m_BaseRecoil");
            m_Reloader = serializedObject.FindProperty("m_BaseReloader");
            m_Shooter = serializedObject.FindProperty("m_BaseShooter");
            m_Trigger = serializedObject.FindProperty("m_BaseTrigger");
            m_ProjectileEffect = serializedObject.FindProperty("m_BaseProjectileEffect");
        }
    }
}