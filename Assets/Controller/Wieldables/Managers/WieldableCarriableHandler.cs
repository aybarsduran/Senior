using IdenticalStudios.InputSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.WieldableSystem
{
    public sealed class WieldableCarriableHandler : CharacterBehaviour, IWieldableCarriableHandler, ISaveableComponent
    {
        public ICarriable Carriable => m_Carriable;
        public int CarryCount => m_Carriable == null ? 0 : m_Carriable.CarryCount;

        public event UnityAction ObjectCarryStarted;
        public event UnityAction ObjectCarryStopped;
        public event UnityAction<int> CarriedCountChanged;

        [SerializeField]
        private InputContextGroup m_CarryContext;

        [Title("Settings")]

        [SerializeField]
        [Tooltip("The layer mask that will be used in checking for obstacles when items are dropped.")]
        private LayerMask m_DropObstacleMask = Physics.DefaultRaycastLayers;

        private ICarriable m_Carriable;
        private ObjectDropper m_CarriableDropper;
        private IWieldablesController m_WieldablesController;
        private IWieldableSelectionHandler m_SelectionHandler;
        private ICharacterMotor m_Motor;

        private readonly Dictionary<DataIdReference<CarriableDefinition>, ICarriable> m_InSceneCarriables = new();


        protected override void OnBehaviourEnabled()
        {
            GetModule(out m_Motor);
            GetModule(out m_SelectionHandler);
            GetModule(out m_WieldablesController);

            m_CarriableDropper = new ObjectDropper(Character, Character.ViewTransform, m_DropObstacleMask);
        }

        public bool TryCarryObject(DataIdReference<CarriableDefinition> carriableId)
        {
            if (carriableId.IsNull)
                return false;

            if (m_Carriable == null)
            {
                if (m_InSceneCarriables.TryGetValue(carriableId, out var carriable))
                    m_Carriable = carriable;
                else
                    m_Carriable = SpawnCarriableWithId(carriableId);

                if (m_Carriable != null && m_WieldablesController.TryEquipWieldable(m_Carriable, 1.3f))
                    StartObjectCarry();
                else
                    m_Carriable = null;
            }

            if (m_Carriable != null && carriableId == m_Carriable.Definition && m_Carriable.TryAddCarriable(1))
            {
                CarriedCountChanged?.Invoke(m_Carriable.CarryCount);
                return true;
            }

            return false;
        }

        public void UseCarriedObject()
        {
            if (m_Carriable == null)
                return;

            if (m_Carriable.TryUseCarriable() && m_Carriable.CarryCount == 0)
                StopObjectCarry();
        }

        public void DropCarriedObjects(int amount)
        {
            if (m_Carriable == null || amount <= 0 || (m_WieldablesController.ActiveWieldable != m_Carriable))
                return;

            float dropHeightMod = m_Motor.Height / m_Motor.DefaultHeight;
            if (m_Carriable.TryDropCarriable(amount, dropHeightMod))
            {
                CarriedCountChanged?.Invoke(m_Carriable.CarryCount);
                if (m_Carriable.CarryCount == 0)
                    StopObjectCarry();
            }
        }

        private void StartObjectCarry()
        {
            Carriable.HolsteringEnded += ForceDropAllCarriables;
            InputManager.PushContext(m_CarryContext);
            ObjectCarryStarted?.Invoke();
        }

        private void StopObjectCarry()
        {
            Carriable.HolsteringEnded -= ForceDropAllCarriables;
            m_Carriable = null;

            ObjectCarryStopped?.Invoke();

            m_SelectionHandler.SelectAtIndex(m_SelectionHandler.SelectedIndex);
            InputManager.PopContext(m_CarryContext);
        }

        private void ForceDropAllCarriables() => DropCarriedObjects(CarryCount);

        private ICarriable SpawnCarriableWithId(DataIdReference<CarriableDefinition> carriableDef)
        {
            var carriable = m_WieldablesController.AddWieldable(carriableDef.Def.Wieldable, true, false) as ICarriable;
            m_InSceneCarriables.Add(carriableDef, carriable);
            carriable.InitializeCarriable(carriableDef, m_CarriableDropper);
            carriable.SetVisibility(false);

            return carriable;
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            var carriedDef = (DataIdReference<CarriableDefinition>)members[0];
            int carriedCount = (int)members[1];

            for (int i = 0; i < carriedCount; i++)
                TryCarryObject(carriedDef);
        }

        public object[] SaveMembers()
        {
            var members = new object[]
            {
                (m_Carriable == null ? DataIdReference<CarriableDefinition>.Empty : m_Carriable.Definition), // [0] Definition
                (m_Carriable == null ? 0 : m_Carriable.CarryCount)  // [1] Carry Count
            };

            return members;
        }
        #endregion
    }
}