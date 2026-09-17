using System;

namespace Gameplay.Features.Battlesystem
{
    public class StunStat
    {
        public bool IsStunned { get; set; }
        public float RemainingTime { get; set; }

        public event Action OnStunStarted;
        public event Action OnStunEnded;

        public StunStat()
        {
            IsStunned = false;
            RemainingTime = 0f;
        }

        public void Restart(float duration)
        {
            RemainingTime = duration;
            if (!IsStunned)
            {
                IsStunned = true;
                OnStunStarted?.Invoke();
            }
        }

        public void Clear()
        {
            IsStunned = false;
            RemainingTime = 0f;
            OnStunStarted = null;
            OnStunEnded = null;
        }

        public void End()
        {
            IsStunned = false;
            RemainingTime = 0f;
            OnStunEnded?.Invoke();
        }
    }
}