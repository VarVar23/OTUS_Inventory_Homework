using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryDestroyer
    {
        private InventoryMultiCellRepository _multiCellRepository;
        private InventoryRepository _repository;

        public InventoryDestroyer(InventoryMultiCellRepository multiCellRepository, InventoryRepository repository)
        {
            _multiCellRepository = multiCellRepository;
            _repository = repository;
        }

        public void DestroyItem(InventoryItemView item)
        {
            if(item == null) return;

            var cell = _repository.GetCellByItem(item);
            _multiCellRepository.SetItem(null);
            _repository.TrySetItem(null, cell);

            GameObject.Destroy(item.gameObject);
        }
    }
}