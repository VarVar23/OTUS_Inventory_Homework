using TMPro;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryTooltipInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _tooltip;
        [SerializeField] private float _startHeight;
        [SerializeField] private float _addHeightEveryInfo;
        [SerializeField] private float _offsetPositionX;
        [SerializeField] private float _offsetPositionY;

        public void SetInfo(string info)
        {
            _text.text = info;
            _text.ForceMeshUpdate();

            _tooltip.sizeDelta = new Vector2(_tooltip.sizeDelta.x, _startHeight + (_text.textInfo.lineCount * _addHeightEveryInfo));
        }

        public void SetPosition(Transform item)
        {
            _tooltip.position = item.position;
            _tooltip.anchoredPosition += new Vector2(_offsetPositionX, _offsetPositionY);
        }

        public void Active(bool value)
        {
            _tooltip.gameObject.SetActive(value);
        }
    }
}