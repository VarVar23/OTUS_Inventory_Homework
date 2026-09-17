using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class InventoryItemView : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<InventoryItemView> OnUp;
        public event Action<InventoryItemView> OnDown;
        public event Action<InventoryItemView> OnEnter;
        public event Action<InventoryItemView> OnExit;
        public InventoryPayloadData Data;
        private Image _upgradeImage;

        public void Initialize(InventoryPayloadData data, Image upgradeImage)
        {
            Data = data;
            _upgradeImage = upgradeImage;
            GetComponent<Image>().sprite = data.Icon;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDown?.Invoke(this);

        public void OnPointerEnter(PointerEventData eventData) => OnEnter?.Invoke(this);

        public void OnPointerExit(PointerEventData eventData) => OnExit?.Invoke(this);

        public void OnPointerUp(PointerEventData eventData) => OnUp?.Invoke(this);

        public void SetUpgradeVisual(bool isUpgrade)
        {
            if(_upgradeImage != null) // TEMP
                _upgradeImage.gameObject.SetActive(isUpgrade);
        }
    }
}