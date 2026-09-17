using System;
using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    internal class ExitController : IInitializable
    {
        public event Action OnExit;
        private ExitView _view;

        public ExitController(ExitView view)
        {
            _view = view;
        }

        public void Initialize()
        {
            _view.OnExitButtonClick += Quit;
        }

        private void Quit()
        {
            OnExit?.Invoke();
            Application.Quit();
        }
    }
}