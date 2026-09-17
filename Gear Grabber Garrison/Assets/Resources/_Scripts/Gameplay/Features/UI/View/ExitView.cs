using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    internal class ExitView : MonoBehaviour
    {
        public event Action OnExitButtonClick;
        [SerializeField] private Button _exit;

        private void Start()
        {
            _exit.onClick.AddListener(() => OnExitButtonClick?.Invoke());
        }
    }
}