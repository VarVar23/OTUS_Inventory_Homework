using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay.Inventory
{
    public class Inventory : IInventory, IInitializable
    {
        public IInventoryUnit Unit => _unitService;

        public event Action OnGenerateButtonClick { add => _multiCellService.OnGenerateButtonClick += value; remove => _multiCellService.OnGenerateButtonClick -= value; }
        public event Action OnGetItemButtonClick { add => _coreService.OnGetItemButtonClick += value; remove => _coreService.OnGetItemButtonClick -= value; }
        public event Action OnMultiCellItemUpdate { add => _multiCellService.OnMultiCellItemUpdate += value; remove => _multiCellService.OnMultiCellItemUpdate -= value; }
        public Action<string> OnUpgradeButtonClick { get; set; }
        public Action<string> OnDestroyButtonClick { get; set; }
        public bool IsFull => _coreService.IsFull;

        [Inject] private InventoryService _coreService;
        [Inject] private InventoryMultiCellService _multiCellService;
        [Inject] private InventoryUnitService _unitService;
        [Inject] private InventoryPutItemService _putItemService;
        [Inject] private InventorySaveLoadService _saveLoadService;



        public void Initialize()
        {
            _multiCellService.OnUpgradeButtonClick += (guid) => OnUpgradeButtonClick?.Invoke(guid);
            _multiCellService.OnDestroyButtonClick += (guid) => OnDestroyButtonClick?.Invoke(guid);
        }

        public InventorySaveData GetSaveData() => _saveLoadService.GetSaveData();

        public void Load(InventorySaveData data, List<InventoryPayloadData> payloads) => _saveLoadService.Load(data, payloads);

        public void UpdateCountAvailableItems(int count) => _coreService.UpdateCountAvailableItems(count);

        public void SetGeneratePrice(long price) => _multiCellService.SetGeneratePrice(price);
        public void SetGeneratePriceColor(Color color) => _multiCellService.SetGeneratePriceColor(color);

        public bool TryPutGeneratedItem(InventoryPayloadData payload) => _putItemService.TryPutGeneratedItem(payload);

        public bool TryPutItem(InventoryPayloadData payload) => _putItemService.TryPutItem(payload);

        public bool TryUpdateItemData(string itemID, InventoryPayloadData payload) => _coreService.TryUpdateItemData(itemID, payload);

        public void UpdateUpgradeVisuals(long currentBalance) => _coreService.UpdateUpgradeVisuals(currentBalance);

        public InventoryPayloadData GetMultiCellItemData() => _multiCellService.GetData();

        public void SetUpgradePriceColor(Color color) => _multiCellService.SetUpgradePriceColor(color);
    }
}