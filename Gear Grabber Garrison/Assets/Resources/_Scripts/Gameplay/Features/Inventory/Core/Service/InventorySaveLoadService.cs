using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventorySaveLoadService
    {
        [Inject] private InventoryRepository _repository;
        [Inject] private InventoryPutItemService _putItemService;
        [Inject] private InventoryUnitsRepository _unitsRepository;

        public InventorySaveData GetSaveData()
        {
            var data = new InventorySaveData();

            for (int i = 0; i < _repository.AllSlots.Count; i++)
            {
                var slot = _repository.AllSlots[i];
                if (slot.ItemView == null) continue;

                data.Items.Add(new ItemSlotData
                {
                    ItemID = slot.ItemView.Data.GUID,
                    CellIndex = i
                });
            }

            data.UnlockedUnits = _unitsRepository.GetUnlockedUnitIDs().Select(u => u.ToString()).ToList();

            return data;
        }

        public void Load(InventorySaveData data, List<InventoryPayloadData> payloads)
        {
            if (data == null || payloads == null) return;

            foreach (var unitId in data.UnlockedUnits)
            {
                if (Guid.TryParse(unitId, out var unit))
                {
                    _unitsRepository.UnlockUnit(unit);
                }
            }

            for (int i = 0; i < data.Items.Count; i++)
            {
                var itemData = data.Items[i];
                if (itemData.CellIndex < 0 || itemData.CellIndex >= _repository.AllSlots.Count) continue;

                var payload = payloads.Find(p => p.GUID == itemData.ItemID);
                if (payload == null) continue;

                var cell = _repository.AllSlots[itemData.CellIndex].CellView;
                _putItemService.TryPutItemToCell(payload, cell);
            }
        }
    }
}