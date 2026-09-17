using Gameplay.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TEST : MonoBehaviour
{
    [SerializeField] private InventoryPayloadData _payload;

    [Inject] private IInventory _inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            var data = _inventory.GetSaveData();
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("InventoryTestSave", json);
            PlayerPrefs.Save();
            Debug.Log($"<color=green>Saved Inventory:</color> {json}");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (PlayerPrefs.HasKey("InventoryTestSave"))
            {
                string json = PlayerPrefs.GetString("InventoryTestSave");
                var data = JsonUtility.FromJson<InventorySaveData>(json);

                // Создаем фейковые пейлоады просто чтобы было что загружать (т.к. мы в TEST и у нас нет ItemizationSystem)
                var payloads = new List<InventoryPayloadData>();
                foreach (var item in data.Items)
                {
                    payloads.Add(new InventoryPayloadData
                    {
                        GUID = item.ItemID,
                        Icon = _payload.Icon, // Берем дефолтную иконку из инспектора
                        Type = _payload.Type
                    });
                }

                _inventory.Load(data, payloads);
                Debug.Log($"<color=yellow>Loaded Inventory from JSON:</color> {json}");
            }
        }
    }
}