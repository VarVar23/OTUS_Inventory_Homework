using DG.Tweening;
using Gameplay.Input;
using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryMoveItemController : ITickable
    {
        private InventorySettings _settings;
        private Transform _movable;
        private Vector3 _offset;

        private readonly IInputStateReader _inputStateReader;

        public InventoryMoveItemController(InventorySettings settings, IInputStateReader inputStateReader)
        {
            _settings = settings;
            _inputStateReader = inputStateReader;
        }

        public void Tick()
        {
            if (_movable != null)
            {
                CursorMovable();
            }
        }

        public void AddCursorMovable(Transform movable)
        {
            _movable = movable;
            _offset = movable.position - (Vector3)_inputStateReader.PointerPosition;
        }

        public void RemoveCursorMovable() => _movable = null;

        public void MoveAnimation(Transform item, Vector3 target, Action onComplete)
        {
            item.DOMove(target, _settings.MoveItemTime).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void MoveWithoutAnimation(Transform item, Vector3 target)
        {
            item.position = target;
        }

        private void CursorMovable()
        {
            _movable.position = (Vector3)_inputStateReader.PointerPosition + _offset;
        }
    }
}