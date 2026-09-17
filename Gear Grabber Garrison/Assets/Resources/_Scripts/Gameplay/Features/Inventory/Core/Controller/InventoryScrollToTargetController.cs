using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class InventoryScrollToTargetController
    {
        private ScrollRect _scrollRect;
        private InventorySettings _settings;
        private float _scrollableHeight;

        public InventoryScrollToTargetController(UnitsView unitsView, InventorySettings settings)
        {
            _scrollRect = unitsView.ScrollRect;
            _settings = settings;
        }

        public void ScrollToTarget(Transform target, System.Action onComplete = null)
        {
            if (!CanScroll(target))
            {
                onComplete?.Invoke();
                return;
            }

            float normalizedPos = CalculateNormalizedPosition(target);

            if (Mathf.Approximately(_scrollRect.verticalNormalizedPosition, normalizedPos))
            {
                onComplete?.Invoke();
                return;
            }

            DOTween.To(
                () => _scrollRect.verticalNormalizedPosition,
                value => _scrollRect.verticalNormalizedPosition = value,
                normalizedPos,
                _settings.MoveScrollToTargetTime
            ).OnComplete(() => onComplete?.Invoke());
        }

        private bool CanScroll(Transform target)
        {
            _scrollableHeight = 0f;

            if (_scrollRect == null || _scrollRect.content == null || !target.IsChildOf(_scrollRect.content))
                return false;

            var viewport = _scrollRect.viewport != null ? _scrollRect.viewport : (RectTransform)_scrollRect.transform;
            _scrollableHeight = _scrollRect.content.rect.height - viewport.rect.height;

            return _scrollableHeight > 0f;
        }

        private float CalculateNormalizedPosition(Transform target)
        {
            var content = _scrollRect.content;
            var viewport = _scrollRect.viewport != null ? _scrollRect.viewport : (RectTransform)_scrollRect.transform;

            float targetY = -content.InverseTransformPoint(target.position).y;
            float desiredOffset = Mathf.Clamp(targetY - viewport.rect.height * 0.5f, 0f, _scrollableHeight);

            return Mathf.Clamp01(1f - desiredOffset / _scrollableHeight);
        }
    }
}
