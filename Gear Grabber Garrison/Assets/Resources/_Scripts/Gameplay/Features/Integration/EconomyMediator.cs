using System;
using Gameplay.Currency;
using Gameplay.Inventory;
using Gameplay.Itemization;
using Gameplay.Economy;
using Zenject;
using UnityEngine;

namespace Gameplay.Integration
{
    public class MockOwner: IHasID // TEMP
    {
        public Guid Id { get; private set; }

        public void SetID(Guid id)
        {
            Id = id;
        }   
    }
    public class EconomyMediator : IInitializable, IDisposable
    {
        private readonly IInventory _inventory;
        private readonly ICurrency _currency;
        private readonly ItemizationSystem _itemization;
        private readonly EconomyConfig _config;

        public EconomyMediator(IInventory inventory, ICurrency currency, ItemizationSystem itemization, EconomyConfig config)
        {
            _inventory = inventory;
            _currency = currency;
            _itemization = itemization;
            _config = config;
        }

        public void Initialize()
        {
            _inventory.OnMultiCellItemUpdate += UpdatePriceColor;
            _inventory.OnUpgradeButtonClick += HandleUpgradeClick;
            _inventory.OnDestroyButtonClick += HandleDestroyClick;
            _inventory.OnGenerateButtonClick += HandleGenerateClick;
            _inventory.OnGetItemButtonClick += HandleClaimRequest;
            _inventory.Unit.OnBuyButtonClick += HandleBuyUnitClick;
            _inventory.Unit.OnEquippedItem += HandleEquippedItem;
            _inventory.Unit.OnUnequippedItem += OnUnequippedItem;
            _itemization.OnUnclaimedQueueUpdate += HandleLootQueueUpdate;
            _currency.OnChanged += HandleCurrencyChanged;

            _inventory.SetGeneratePrice(_config.GenerateItemPrice);

            foreach (var data in _config.UnitsData)
            {
                data.SetID(); // TEMP
                _inventory.Unit.Create(data.Clone());

                MockOwner ownerEntity = new(); // TEMP
                ownerEntity.SetID(data.ID); // TEMP
            }

            _inventory.UpdateUpgradeVisuals(_currency.Balance);
            UpdatePriceColor();
        }

        public void Dispose()
        {
            _inventory.OnMultiCellItemUpdate -= UpdatePriceColor;
            _inventory.OnUpgradeButtonClick -= HandleUpgradeClick;
            _inventory.OnDestroyButtonClick -= HandleDestroyClick;
            _inventory.OnGenerateButtonClick -= HandleGenerateClick;
            _inventory.OnGetItemButtonClick -= HandleClaimRequest;
            _inventory.Unit.OnBuyButtonClick -= HandleBuyUnitClick;
            _inventory.Unit.OnEquippedItem -= HandleEquippedItem;
            _inventory.Unit.OnUnequippedItem -= OnUnequippedItem;
            _itemization.OnUnclaimedQueueUpdate -= HandleLootQueueUpdate;
            _currency.OnChanged -= HandleCurrencyChanged;
        }

        private void UpdatePriceColor()
        {
            _inventory.SetGeneratePriceColor(_currency.GetTrySpendColor(_config.GenerateItemPrice));

            foreach(var unit in _config.UnitsData)
            {
                _inventory.Unit.SetUnlockPriceColor(unit.ID, _currency.GetTrySpendColor(unit.Price));
            }

            if(_inventory.GetMultiCellItemData() != null)
            {
                var upgragePrice = _inventory.GetMultiCellItemData().UpgradePrice;
                _inventory.SetUpgradePriceColor(_currency.GetTrySpendColor(upgragePrice));
            }
        }

        private void OnUnequippedItem(Guid ownerID, string itemID)
        {
            if (Guid.TryParse(itemID, out Guid item))
            {
                _itemization.UnequipItem(ownerID, item);
                _inventory.Unit.SetEffects(ownerID, _itemization.GetOwnerDescription(ownerID));
            }
        }

        private void HandleEquippedItem(Guid ownerID, string itemID)
        {
            if (Guid.TryParse(itemID, out Guid item))
            {
                _itemization.EquipItem(ownerID, item);
                _inventory.Unit.SetEffects(ownerID, _itemization.GetOwnerDescription(ownerID));
            }
        }

        private void HandleUpgradeClick(string itemID)
        {
            if (Guid.TryParse(itemID, out Guid guid))
            {
                IItem item = _itemization.GetItemData(guid);
                if (item != null && item.CanUpgrade)
                {
                    int upgradePrice = item.ValueData.UpgradeValue;
                    
                    if (_currency.TrySpend(upgradePrice))
                    {
                        IItem upgradedItem = _itemization.UpgradeItem(guid);
                        var payload = CreatePayload(upgradedItem);
                        _inventory.TryUpdateItemData(itemID, payload);
                        _inventory.UpdateUpgradeVisuals(_currency.Balance);
                    }
                }
            }
        }

        private void HandleDestroyClick(string itemID)
        {
            if (Guid.TryParse(itemID, out Guid guid))
            {
                IItem item = _itemization.GetItemData(guid);
                if (item != null)
                {
                    int sellPrice = item.ValueData.CurrentValue;
                    _itemization.DeleteItem(guid);
                    _currency.Add(sellPrice);
                }
            }
        }

        private void HandleGenerateClick()
        {
            long price = _config.GenerateItemPrice;

            if (_currency.TrySpend(price))
            {
                IItem generatedItem = _itemization.CraftFromLootTable(LootTable.TestCraftTable);
                
                if (generatedItem != null)
                {
                    _inventory.TryPutGeneratedItem(CreatePayload(generatedItem));
                    _inventory.UpdateUpgradeVisuals(_currency.Balance);
                }
            }
        }

        private void HandleBuyUnitClick(Guid unitID)
        {
            long price = _inventory.Unit.GetUnitPayloadData(unitID).Price;

            if (price >= 0 && _currency.TrySpend(price))
            {
                _inventory.Unit.Unlock(unitID);
            }
        }

        private void HandleLootQueueUpdate(int queueCount)
        {
            _inventory.UpdateCountAvailableItems(queueCount);
        }

        private void HandleClaimRequest()
        {
            if (_inventory.IsFull) return;

            IItem claimedItem = _itemization.ClaimNextPendingItem();
            if (claimedItem != null)
            {
                InventoryPayloadData payload = CreatePayload(claimedItem);
                _inventory.TryPutItem(payload);
                _inventory.UpdateUpgradeVisuals(_currency.Balance);
            }
        }

        private void HandleCurrencyChanged(long balance)
        {
            _inventory.UpdateUpgradeVisuals(balance);
            UpdatePriceColor();
        }

        private InventoryPayloadData CreatePayload(IItem item)
        {
            var cellType = (InventoryCellType)Enum.Parse(typeof(InventoryCellType), item.Type.ToString());

            return new InventoryPayloadData
            {
                GUID = item.Id.ToString(),
                Icon = item.Icon,
                Type = cellType,
                UpgradePrice = item.ValueData.UpgradeValue,
                DestroyPrice = item.ValueData.CurrentValue,
                CanUpgrade = item.CanUpgrade,
                Description = item.Description,
                UpgradeDescription = item.UpgradeDescription
            };
        }
    }
}
