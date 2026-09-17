using Zenject;

namespace Gameplay.Features.Battlesystem
{
    public class BattleInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<TickManager>().AsSingle().NonLazy();
            
            Container.Bind<BattleManager>().FromComponentInHierarchy().AsSingle(); // Ищет компонент в текущей иерархии
            Container.Bind<EnemyFactory>().AsSingle();
            Container.Bind<PlayerFactory>().AsSingle();
            // Container.Bind<IUnitFactory>().To<PlayerFactory>().AsSingle();
            // Container.Bind<Army>().AsSingle().NonLazy();
            // Container.Bind<IBattleFacade>().To<BattleFacade>().AsSingle();
            Container.BindInterfacesTo<BattleFacade>().AsSingle().NonLazy();
        }
    }
}