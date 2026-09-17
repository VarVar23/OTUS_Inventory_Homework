using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class OpenUnitsView : MonoBehaviour
    {
        [SerializeField] private RectTransform _allUnitsPanel;
        [SerializeField] private GameObject _openIcon;
        [SerializeField] private GameObject _closeIcon;
        [SerializeField] private Toggle _openToggle;
        [SerializeField] private float _openOffsetX;
        [SerializeField] private float _closeOffsetX;
        [SerializeField] private float _timeAnimation;
        private Tween _tween;

        private void Start()
        {
            _openToggle.onValueChanged.AddListener(ChangeVisible);
        }

        private void ChangeVisible(bool value)
        {
            float targetPositionX = value ? _openOffsetX : _closeOffsetX;

            _openIcon.SetActive(!value);
            _closeIcon.SetActive(value);

            if (_tween != null) _tween.Kill();
            if(value) _allUnitsPanel.gameObject.SetActive(true);

            _tween = _allUnitsPanel.DOAnchorPosX(targetPositionX, _timeAnimation).OnComplete(() => 
            _allUnitsPanel.gameObject.SetActive(value));
        }
    }
}