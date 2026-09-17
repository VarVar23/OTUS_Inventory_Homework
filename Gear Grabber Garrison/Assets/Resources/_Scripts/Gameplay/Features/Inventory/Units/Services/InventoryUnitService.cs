using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryUnitService : IInventoryUnit, IInitializable
    {
        public Action<Guid> OnBuyButtonClick { get; set; }

        public event Action<Guid, string> OnEquippedItem
        {
            add => _unitsRepository.OnEquippedItem += value;
            remove => _unitsRepository.OnEquippedItem -= value;
        }

        public event Action<Guid, string> OnUnequippedItem
        {
            add => _unitsRepository.OnUnequippedItem += value;
            remove => _unitsRepository.OnUnequippedItem -= value;
        }

        public event Action<Guid> OnAvaEnter;
        public event Action<Guid> OnAvaExit;
        public event Action<Guid> OnUnitCreated;

        [Inject] private InventoryUnitsRepository _unitsRepository;
        [Inject] private InventoryRepository _repository;
        [Inject] private UnitFactory _unitFactory;
        [Inject] private UnitReloadController _unitReloadController;

        public void Create(UnitPayloadData payload)
        {
            var unitView = _unitFactory.Create(payload);
            
            _unitsRepository.RegisterView(unitView, payload);
            _repository.RegisterCells(unitView.CellViews);

            unitView.OnBuyButtonClick += (id) => OnBuyButtonClick?.Invoke(id);
            unitView.OnAvaEnter += (id) => OnAvaEnter?.Invoke(id);
            unitView.OnAvaExit += (id) => OnAvaExit?.Invoke(id);
        }

        public void Initialize()
        {
            _unitReloadController.ChangeReloadPercent += ChangeReloadProcent;
        }

        public void Tick(float deltaTime)
        {
            _unitReloadController.Tick(deltaTime);
        }

        public Dictionary<Guid, List<string>> GetEquippedItems() => _unitsRepository.GetEquippedGUIDItems();
        public void SetEffects(Guid unitID, string effects) => _unitsRepository.SetUnitEffects(unitID, effects);
        public UnitPayloadData GetUnitPayloadData(Guid unitID) => _unitsRepository.GetUnitData(unitID);
        public void SetUnlockPriceColor(Guid unitID, Color color) => _unitsRepository.GetView(unitID).SetUnlockPriceColor(color);

        public void Unlock(Guid unitID)
        {
            _unitsRepository.UnlockUnit(unitID);
            OnUnitCreated?.Invoke(unitID);
        }

        public void StartReload(Guid guid, float reloadSeconds)
        {
            _unitReloadController.StartReload(guid, reloadSeconds);
        }

        public void StopReload(Guid guid)
        {
            _unitReloadController.StopReload(guid);
        }

        private void ChangeReloadProcent(Guid unitID, float value) => _unitsRepository.GetView(unitID).ChangeReloadProcent(value);
    }
}