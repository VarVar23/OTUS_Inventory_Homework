using UnityEngine;
using Zenject;

namespace Gameplay.Currency
{
    public class CurrencyInstaller : MonoInstaller
    {
        [SerializeField] private CurrencyData _currencyData;
        [SerializeField] private CurrencyView _view;

        public override void InstallBindings()
        {
            Container.Bind<CurrencyRepository>().AsSingle();

            Container.Bind<CurrencyData>().FromInstance(_currencyData).AsSingle();
            Container.Bind<CurrencyView>().FromInstance(_view).AsSingle();

            Container.BindInterfacesAndSelfTo<Currency>().AsSingle();
        }
    }
}