using TMPro;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryMulticellInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _parent;
        [SerializeField] private float _startHeight;
        [SerializeField] private float _addHeightEveryInfo;

        public void SetInfo(string info)
        {
            _text.text = info;
            _text.ForceMeshUpdate();

            _parent.sizeDelta = new Vector2(_parent.sizeDelta.x, _startHeight + (_text.textInfo.lineCount * _addHeightEveryInfo));
        }
    }
}