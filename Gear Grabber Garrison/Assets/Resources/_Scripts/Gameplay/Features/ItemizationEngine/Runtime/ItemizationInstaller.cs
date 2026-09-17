using Gameplay.Itemization.Services;
using Zenject;

namespace Gameplay.Itemization
{
    public class ItemizationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<JSONItemizationRegistry>().AsSingle().NonLazy();
            
            InstallNonDependentBindings();
            
            InstallSecondLayerBindings();
            
            InstallThirdLayerBindings();

            Container.BindInterfacesAndSelfTo<ItemizationSystem>().AsSingle();
        }

        // Technically, some depend on JSONItemizationRegistry, so don't forget to bind it first
        private void InstallNonDependentBindings()
        {
            Container.Bind<ItemValueService>().AsSingle();
            Container.Bind<ItemIconPicker>().AsSingle();
            Container.Bind<ItemNameGenerator>().AsSingle();
            Container.Bind<ItemPool>().AsSingle();
            Container.Bind<ItemOwnerRegistry>().AsSingle();
            Container.Bind<AttributeEffectManager>().AsSingle();
        }

        private void InstallSecondLayerBindings()
        {
            Container.Bind<BuildInstructionsGenerator>().AsSingle();
            Container.Bind<ItemBuilderService>().AsSingle();
            Container.Bind<ItemUpgradeService>().AsSingle();
        }

        private void InstallThirdLayerBindings()
        {
            Container.Bind<ItemPipelineManager>().AsSingle();
        }
    }
}