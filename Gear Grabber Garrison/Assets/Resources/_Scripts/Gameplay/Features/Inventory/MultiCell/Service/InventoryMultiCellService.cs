using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryMultiCellService
    {
        public event Action<InventoryItemView> OnDestroyRequest;
        public event Action<string> OnUpgradeButtonClick;
        public event Action<string> OnDestroyButtonClick;
        public event Action OnMultiCellItemUpdate;

        public event Action OnGenerateButtonClick
        {
            add => _multiCellView.OnGenerateButtonClick += value;
            remove => _multiCellView.OnGenerateButtonClick -= value;
        }

        private InventoryMultiCellRepository _multiCellRepository;
        private InventoryMultiCellView _multiCellView;
        private InventoryDestroyer _destroyer;

        public InventoryMultiCellService(InventoryMultiCellRepository multiCellRepository, InventoryMultiCellView multiCellView, InventoryDestroyer destroyer)
        {
            _multiCellRepository = multiCellRepository;
            _multiCellView = multiCellView;
            _destroyer = destroyer;
        }

        public void Initialize()
        {
            _multiCellRepository.OnMultiCellSet += MultiCellSet;

            _multiCellView.UpgradeButton.onClick.AddListener(() =>
            {
                if (_multiCellRepository.Slot.ItemView != null)
                    OnUpgradeButtonClick?.Invoke(_multiCellRepository.Slot.ItemView.Data.GUID);
            });

            _multiCellView.DestroyButton.onClick.AddListener(() =>
            {
                var item = _multiCellRepository.Slot.ItemView;
                if (item == null) return;
                var guid = item.Data.GUID;
                OnDestroyRequest?.Invoke(item);
                _destroyer.DestroyItem(item);
                OnDestroyButtonClick?.Invoke(guid);
            });
        }

        public void Load(InventoryCellView cell, InventoryItemView item)
        {
            _multiCellRepository.Load(cell, item);
        }

        public void SetVisibleDragSelected(bool value)
        {
            _multiCellView.SetVisibleDragSelected(value);
        }

        public InventoryPayloadData GetData() => _multiCellRepository.Slot?.ItemView?.Data;
        public void SetGeneratePrice(long price) => _multiCellView.SetGeneratePriceText(price);
        public void SetGeneratePriceColor(Color color) => _multiCellView.SetGeneratePriceColor(color);
        public void SetUpgradePriceColor(Color color) => _multiCellView.SetUpgradePriceColor(color);

        public void UpdateData()
        {
            var view = _multiCellRepository.Slot.ItemView;
           
            if (view != null)
            {
                _multiCellView.SetInfo(view.Data);
                OnMultiCellItemUpdate?.Invoke();
            }
        }

        private void MultiCellSet(InventoryItemView view)
        {
            _multiCellView.SetVisibleItemManager(view != null);

            if (view != null)
            {
                _multiCellView.SetInfo(view.Data);
                OnMultiCellItemUpdate?.Invoke();
            }
        }
    }
}