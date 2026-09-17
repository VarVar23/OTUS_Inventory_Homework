using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Localization
{
    internal class LocalizationView : MonoBehaviour
    {
        public Action OnChangeLanguageButtonClick;

        [SerializeField] private Button _changeLanguageButton;
        private Image _languageImage;

        private void Awake()
        {
            _languageImage = _changeLanguageButton.GetComponent<Image>();
            _changeLanguageButton.onClick.AddListener(() => OnChangeLanguageButtonClick?.Invoke());
        }

        public void SetLanguageSprite(Sprite icon)
        {
            _languageImage.sprite = icon;
        }
    }
}