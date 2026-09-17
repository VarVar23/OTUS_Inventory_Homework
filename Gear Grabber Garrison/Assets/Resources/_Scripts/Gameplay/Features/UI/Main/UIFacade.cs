using System;
using Zenject;

namespace Gameplay.UI
{
    public class UIFacade : IUIFacade
    {
        public event Action<bool> OnPause { add => _pauseView.OnPause += value; remove => _pauseView.OnPause -= value; }
        public event Action<int> OnSpeedGameChanged { add => _speedController.OnSpeedGameChanged += value; remove => _speedController.OnSpeedGameChanged -= value; }
        public event Action OnExit { add => _exitController.OnExit += value; remove => _exitController.OnExit -= value; }

        [Inject] private PauseView _pauseView;
        [Inject] private SpeedGameController _speedController;
        [Inject] private ExitController _exitController;

        public void LoadSpeedIndex(int index) => _speedController.SetCurrentIndex(index);
        public int GetSpeedIndex() => _speedController.GetCurrentIndex();
    }
}