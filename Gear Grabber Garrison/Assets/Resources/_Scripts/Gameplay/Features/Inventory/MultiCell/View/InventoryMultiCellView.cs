using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class InventoryMultiCellView : MonoBehaviour
    {
        public event Action OnUpgradeButtonClick;
        public event Action OnDestroyButtonClick;
        public event Action OnGenerateButtonClick;

        [field: SerializeField] public Button UpgradeButton {  get; private set; }
        [field: SerializeField] public Button DestroyButton { get; private set; }
        [field: SerializeField] public Button GenerateButton { get; private set; }

        [SerializeField] private InventoryMulticellInfoView _infoView;
        [SerializeField] private GameObject _activeItemOnSelected;
        [SerializeField] private GameObject _activeItemOnDrag;
        [SerializeField] private GameObject _deactiveItemOnSelected;
        [SerializeField] private TMP_Text _upgradePrice;
        [SerializeField] private TMP_Text _destroyPrice;
        [SerializeField] private TMP_Text _generatePrice;

        private void Start()
        {
            UpgradeButton.onClick.AddListener(() => OnUpgradeButtonClick?.Invoke());
            DestroyButton.onClick.AddListener(() => OnDestroyButtonClick?.Invoke());
            GenerateButton.onClick.AddListener(() => OnGenerateButtonClick?.Invoke());
        }

        public void SetVisibleItemManager(bool value)
        {
            _activeItemOnSelected.SetActive(value);
            _deactiveItemOnSelected.SetActive(!value);
        }

        public void SetVisibleDragSelected(bool value)
        {
            _activeItemOnDrag.SetActive(value);
        }

        public void SetInfo(InventoryPayloadData data)
        {
            _infoView.SetInfo(data.UpgradeDescription);
            SetUpgradePriceText(data.UpgradePrice);
            SetDestroyPriceText(data.DestroyPrice);

            UpgradeButton.gameObject.SetActive(data.CanUpgrade);
            _upgradePrice.gameObject.SetActive(data.CanUpgrade);
        }

        public void SetUpgradePriceText(int value) => _upgradePrice.text = value.ToString();
        public void SetDestroyPriceText(int value) => _destroyPrice.text = value.ToString();
        public void SetGeneratePriceText(long value) => _generatePrice.text = value.ToString();
        public void SetUpgradePriceColor(Color color) => _upgradePrice.color = color;
        public void SetDestroyPriceColor(Color color) => _destroyPrice.color = color;
        public void SetGeneratePriceColor(Color color) => _generatePrice.color = color;
    }
}