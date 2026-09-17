using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    internal class PauseView : MonoBehaviour
    {
        public event Action<bool> OnPause;

        [SerializeField] private Toggle _pause;

        private void Start()
        {
            _pause.onValueChanged.AddListener((value) => OnPause?.Invoke(value));
        }
    }
}