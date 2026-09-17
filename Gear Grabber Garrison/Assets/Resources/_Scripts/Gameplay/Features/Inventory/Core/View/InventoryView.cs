using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryView : MonoBehaviour
    {
        [field: SerializeField] public Transform PriorityItemParent { get; private set; }
        [field: SerializeField] public Transform ItemParent { get; private set; }
        [field: SerializeField] public Transform ItemSpawnPoint { get; private set; }
        [field: SerializeField] public List<InventoryCellView> AllCells { get; private set; }

        [SerializeField] private Toggle _openInventoryToggle;
        [Inject] private InventorySettings _settings;

        private void Start()
        {
            _openInventoryToggle.onValueChanged.AddListener(SetVisible);
        }

        public void SetVisible(bool visible)
        {
            float offsetX = visible ? _settings.OpenInventoryY : _settings.CloseInventoryY;
            
            GetComponent<RectTransform>().DOAnchorPosY(offsetX, _settings.MoveInventoryTime);
        }
    }
}