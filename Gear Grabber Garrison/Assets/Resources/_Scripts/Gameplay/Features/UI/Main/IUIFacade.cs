using System;

namespace Gameplay.UI
{
    public interface IUIFacade
    {
        public event Action<bool> OnPause;
        public event Action<int> OnSpeedGameChanged;
        public event Action OnExit;

        public void LoadSpeedIndex(int index);
        public int GetSpeedIndex();
    }
}