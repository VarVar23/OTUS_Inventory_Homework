using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class InventoryAddItemsView : MonoBehaviour
    {
        public event Action OnAddItemsButtonClick;
        [field: SerializeField] public Button AddItemsButton { get; private set; }
        [SerializeField] private TMP_Text _countText;

        private void Start()
        {
            AddItemsButton.onClick.AddListener(() => OnAddItemsButtonClick?.Invoke());
        }

        public void SetCount(int count)
        {
            _countText.text = "x" + count.ToString();

            Active(count > 0);
        }

        private void Active(bool value)
        {
            AddItemsButton.gameObject.SetActive(value);
            _countText.gameObject.SetActive(value);
        }
    }
}