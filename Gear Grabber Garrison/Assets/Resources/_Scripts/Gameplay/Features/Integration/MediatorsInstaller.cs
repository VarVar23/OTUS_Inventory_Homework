using Gameplay.Integration;
using UnityEngine;
using Zenject;

namespace Gameplay.Economy
{
    public class MediatorsInstaller : MonoInstaller
    {
        [SerializeField] private EconomyConfig _config;

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();
            Container.BindInterfacesAndSelfTo<EconomyMediator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BattleInventoryMediator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BattleItemizationMediator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TimeUIMediator>().AsSingle().NonLazy();
        }
    }
}
