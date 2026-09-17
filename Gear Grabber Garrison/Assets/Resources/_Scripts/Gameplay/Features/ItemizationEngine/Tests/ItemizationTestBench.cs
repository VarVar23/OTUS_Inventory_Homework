using UnityEngine;
using System;

namespace Gameplay.Itemization.Tests 
{
    public class ItemizationTestBench : MonoBehaviour
    {
        [Header("Dependencies")]
        public MockPlayer Player; // Assign your MockPlayer GameObject here

        [Header("Configuration")]
        public LootTable DropTable = LootTable.TestDropTable;
        public LootTable CraftTable = LootTable.TestCraftTable;

        private Guid _lastGeneratedId = Guid.Empty;
        private int _unclaimedItemCount = 0;

        private ItemizationSystem System => MockBootstrap.Instance.ItemSystem;

        void Start()
        {
            if (Player != null)
            {
                Debug.Log($"<color=green>[TestBench]</color> Player {Player.Id} Registered.");
            }
            System.OnUnclaimedQueueUpdate += UpdateQueueCount;
        }

        void Update()
        {
            // --- GENERATION ---

            // (R) Roll from Loot Table
            if (Input.GetKeyDown(KeyCode.R))
            {
                System.RollFromLootTable(DropTable);
                Debug.Log($"<color=cyan>[TestBench]</color> Rolled {DropTable}. Unclaimed: {_unclaimedItemCount}");
            }

            // (F) Craft from Table
            if (Input.GetKeyDown(KeyCode.F))
            {
                IItem crafted = System.CraftFromLootTable(CraftTable);
                UpdateLastGenerated(crafted.Id);
            }

            // --- INVENTORY ---

            // (C) Claim Item (Move from Drop Queue to "Inventory")
            if (Input.GetKeyDown(KeyCode.C))
            {
                IItem claimed = System.ClaimNextPendingItem();
                if (claimed != null) UpdateLastGenerated(claimed.Id);
            }

            // (D) Delete Item
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_lastGeneratedId != Guid.Empty)
                {
                    System.DeleteItem(_lastGeneratedId);
                    Debug.Log($"<color=red>[TestBench]</color> Deleted Item.");
                    _lastGeneratedId = Guid.Empty;
                }
            }

            // --- PROGRESSION ---

            // (U) Upgrade Item
            if (Input.GetKeyDown(KeyCode.U) && _lastGeneratedId != Guid.Empty)
            {
                System.UpgradeItem(_lastGeneratedId);
                IItem item = System.GetItemData(_lastGeneratedId);
                Debug.Log($"<color=green>[Upgrade Success]</color> New {item.ItemName}'s state:\n{item.UpgradeDescription}\n"+
                    $"Sell: {item.ValueData.CurrentValue}g | Next Upgrade: {item.ValueData.UpgradeValue}g");
            }

            // --- LOGIC / ATTRIBUTES ---

            // (E) EQUIP
            if (Input.GetKeyDown(KeyCode.E) && _lastGeneratedId != Guid.Empty)
            {
                IItem item = System.GetItemData(_lastGeneratedId);
                if (Player.TryEquip(item))
                {
                    System.EquipItem(Player.Id, item.Id);
                    Debug.Log($"<color=green>[Equip]</color> {item.ItemName} put in {item.Type} slot.");
                    _lastGeneratedId = Guid.Empty; // Item is now in a slot, not "selected"
                    LogPlayerSummary();
                }
            }

            // (Q) UNEQUIP
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (Player.TryUnequipAny(out IItem unequippedItem))
                {
                    System.UnequipItem(Player.Id, unequippedItem.Id);
                    UpdateLastGenerated(unequippedItem.Id);
                    Debug.Log($"<color=yellow>[Unequip]</color> Removed {unequippedItem.ItemName} from player.");
                    LogPlayerSummary();
                }
            }
        }

        private void UpdateLastGenerated(Guid newId)
        {
            // LIFETIME MANAGEMENT: If we had a previous unequipped item and we replace it, delete it.
            if (_lastGeneratedId != Guid.Empty && _lastGeneratedId != newId)
            {
                System.DeleteItem(_lastGeneratedId);
            }

            _lastGeneratedId = newId;
            if (_lastGeneratedId != Guid.Empty)
            {
                IItem item = System.GetItemData(_lastGeneratedId);
                Debug.Log(
                    $"<color=cyan>[TestBench]</color> Selection: {item.ItemName} ({item.Type})\n" +
                    $"{item.UpgradeDescription}\n" +
                    $"Sell: {item.ValueData.CurrentValue}g | Next Upgrade: {item.ValueData.UpgradeValue}g");
            }
        }

        private void LogPlayerSummary()
        {
            string summary = System.GetOwnerDescription(Player.Id);
            Debug.Log($"<color=#32a8a2>[Owner Summary - {Player.Id}]</color>\n{summary}");
        }

        private void UpdateQueueCount(int count) => _unclaimedItemCount = count;

        public void Dispose()
        {
            if (System != null)
            {
                System.OnUnclaimedQueueUpdate -= UpdateQueueCount;
            }
        }
    }
}