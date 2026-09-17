using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InitStates();

        Container.Bind<BootstrapStateMachine>().AsSingle();

        Container.Bind<SceneSwitcher>().AsSingle();
        Container.Bind<CurtainShowerView>().FromComponentInHierarchy().AsSingle();
    }

    private void InitStates()
    {
        Container.BindInterfacesAndSelfTo<BootstrapStateStart>().AsSingle();
        Container.BindInterfacesAndSelfTo<BootstrapStateFirebase>().AsSingle();
        Container.BindInterfacesAndSelfTo<BootstrapStateSceneSwitcher>().AsSingle();
        Container.BindInterfacesAndSelfTo<BootstrapStateAddresables>().AsSingle();
        Container.BindInterfacesAndSelfTo<BootstrapStateEnd>().AsSingle();
    }
}