using System;
using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    internal class SpeedGameController : IInitializable
    {
        public event Action<int> OnSpeedGameChanged;

        private SpeedGameView _view;
        private SpeedGameData _data;
        private int _currentIndex;

        public SpeedGameController(SpeedGameView view, SpeedGameData data)
        {
            _view = view;
            _data = data;
        }

        public void Initialize()
        {
            _view.OnButtonSpeedClick += OnButtonSpeedClick;
            _view.SetText(_data.Speed[_currentIndex]);
            OnSpeedGameChanged?.Invoke(_data.Speed[_currentIndex]);
        }

        public void SetCurrentIndex(int value)
        {
            _currentIndex = Mathf.Clamp(value, 0, _data.Speed.Count);
        }

        public int GetCurrentIndex() => _currentIndex;

        private void OnButtonSpeedClick()
        {
            _currentIndex++;
            _currentIndex = _currentIndex >= _data.Speed.Count ? 0 : _currentIndex;
            _view.SetText(_data.Speed[_currentIndex]);
            OnSpeedGameChanged?.Invoke(_data.Speed[_currentIndex]);
        }
    }
}