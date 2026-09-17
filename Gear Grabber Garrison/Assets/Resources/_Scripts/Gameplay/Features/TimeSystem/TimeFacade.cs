using System;
using Zenject;

namespace Gameplay.TimeSystem
{
    public class TimeFacade : ITimeFacade
    {
        public event Action OnTick { add => _timeManager.OnTick += value; remove => _timeManager.OnTick -= value; }
        public bool Pause { get => _timeManager.IsPaused; set => _timeManager.IsPaused = value; }
        public float DeltaTime => _timeManager.DeltaTime;


        [Inject] private TimeManager _timeManager;

        public void SetTimeScale(int value)
        {
            _timeManager.TimeScale = value;
        }
    }
}