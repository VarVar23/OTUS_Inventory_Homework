using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Inventory
{
    public interface IInventoryUnit
    {
        public event Action<Guid, string> OnEquippedItem;
        public event Action<Guid, string> OnUnequippedItem;
        public event Action<Guid> OnUnitCreated;
        public Action<Guid> OnBuyButtonClick { get; set; }
        
        public Dictionary<Guid, List<string>> GetEquippedItems();
        public UnitPayloadData GetUnitPayloadData(Guid unitID);
        public void Unlock(Guid unitID);
        public void Create(UnitPayloadData payload);
        public void SetEffects(Guid unitID, string effects);
        public void SetUnlockPriceColor(Guid unitID, Color color);
        public void StartReload(Guid guid, float reloadSeconds);
        public void StopReload(Guid guid);
        public void Tick(float deltaTime);
    }
}