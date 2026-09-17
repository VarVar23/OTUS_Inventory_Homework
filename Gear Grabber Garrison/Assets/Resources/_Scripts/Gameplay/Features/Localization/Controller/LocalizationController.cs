using UnityEngine.Localization.Settings;
using Zenject;

namespace Gameplay.Localization
{
    internal class LocalizationController : IInitializable
    {
        private LocalizationView _view;
        private LocalizationData _data;
        private int _currentIndex = 0;

        public LocalizationController(LocalizationView view, LocalizationData data)
        {
            _view = view;
            _data = data;
        }

        public void Initialize()
        {
            _view.OnChangeLanguageButtonClick += Change;
            SetLanguage();
        }

        private void Change()
        {
            _currentIndex++;
            _currentIndex = _currentIndex > _data.Languages.Count - 1 ? 0 : _currentIndex;

            _view.SetLanguageSprite(_data.Languages[_currentIndex].Icon);
            SetLanguage();
        }

        private void SetLanguage()
        {
            LocalizationSettings.SelectedLocale = _data.Languages[_currentIndex].Locale;
        }
    }
}