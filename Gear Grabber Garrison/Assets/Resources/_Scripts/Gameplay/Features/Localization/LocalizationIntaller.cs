using UnityEngine;
using Zenject;

namespace Gameplay.Localization
{
    internal class LocalizationIntaller : MonoInstaller
    {
        [SerializeField] private LocalizationView _localizationView;
        [SerializeField] private LocalizationData _localizationData;

        public override void InstallBindings()
        {
            Container.Bind<LocalizationView>().FromInstance(_localizationView).AsSingle();
            Container.Bind<LocalizationData>().FromInstance(_localizationData).AsSingle();

            Container.BindInterfacesAndSelfTo<LocalizationController>().AsSingle();
        }
    }
}