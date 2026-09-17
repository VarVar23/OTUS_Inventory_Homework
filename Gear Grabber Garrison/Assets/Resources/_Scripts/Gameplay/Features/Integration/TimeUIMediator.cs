using Gameplay.TimeSystem;
using Gameplay.UI;
using System;
using Zenject;

namespace Gameplay.Integration
{
    public class TimeUIMediator : IInitializable, IDisposable
    {
        [Inject] private ITimeFacade _time;
        [Inject] private IUIFacade _ui;

        public void Initialize()
        {
            _ui.OnPause += HandlePause;
            _ui.OnSpeedGameChanged += HandleSpeed;
        }

        public void Dispose()
        {
            _ui.OnPause -= HandlePause;
            _ui.OnSpeedGameChanged -= HandleSpeed;
        }

        private void HandlePause(bool value) => _time.Pause = value;

        private void HandleSpeed(int value)
        {
            _time.SetTimeScale(value);
        }
    }
}