using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class InventoryBuilder
    {
        private InventoryView _inventoryView;
        private InventoryItemSettings _itemSettings;

        public InventoryBuilder(InventoryView inventoryView, InventoryItemSettings itemSettings)
        {
            _inventoryView = inventoryView;
            _itemSettings = itemSettings;
        }

        public InventoryItemView GenerateItem(InventoryPayloadData payload)
        {
            var gameObject = new GameObject();
            var item = gameObject.AddComponent<InventoryItemView>();
            var rect = gameObject.AddComponent<RectTransform>();
            gameObject.AddComponent<Image>();

            gameObject.name = "Item";
            gameObject.transform.SetParent(_inventoryView.ItemParent);
            rect.position = _inventoryView.ItemSpawnPoint.position;
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(_itemSettings.RectScale, _itemSettings.RectScale);

            var arrowObj = new GameObject("UpgradeArrow");
            var arrowImage = arrowObj.AddComponent<Image>();
            var arrowRect = arrowObj.GetComponent<RectTransform>();
            
            arrowObj.transform.SetParent(gameObject.transform);
            arrowImage.sprite = _itemSettings.UpgradeArrowSprite;
            
            arrowRect.anchorMin = new Vector2(1, 0); 
            arrowRect.anchorMax = new Vector2(1, 0);
            arrowRect.pivot = new Vector2(1, 0);
            arrowRect.anchoredPosition = Vector2.zero;
            arrowRect.transform.localScale = Vector3.one;
            arrowRect.sizeDelta = new Vector2(_itemSettings.UpgradeArrowScale, _itemSettings.UpgradeArrowScale);
            
            arrowObj.SetActive(false);

            item.Initialize(payload, arrowImage);
            return item;
        }
    }
}