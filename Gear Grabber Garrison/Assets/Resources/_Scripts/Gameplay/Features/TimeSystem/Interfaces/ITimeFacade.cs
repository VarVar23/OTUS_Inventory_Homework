using System;

namespace Gameplay.TimeSystem
{
    public interface ITimeFacade  
    {
        public event Action OnTick;
        public float DeltaTime { get; }
        public bool Pause { get; set; }
        public void SetTimeScale(int value);
    }
}