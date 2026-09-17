using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    internal class SpeedGameView : MonoBehaviour
    {
        public event Action OnButtonSpeedClick;

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _text;


        private void Start()
        {
            _button.onClick.AddListener(() => OnButtonSpeedClick?.Invoke());
        }

        public void SetText(int speed)
        {
            _text.text = speed + "X";
        }
    }
}