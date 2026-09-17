using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    internal class UIInstaller : MonoInstaller
    {
        [SerializeField] private PauseView _pauseView;
        [SerializeField] private SpeedGameView _speedGameView;
        [SerializeField] private SpeedGameData _speedGameData;
        [SerializeField] private ExitView _exitView;

        public override void InstallBindings()
        {
            Container.Bind<PauseView>().FromInstance(_pauseView).AsSingle();
            Container.Bind<SpeedGameView>().FromInstance(_speedGameView).AsSingle();
            Container.Bind<ExitView>().FromInstance(_exitView).AsSingle();
            Container.Bind<SpeedGameData>().FromInstance(_speedGameData).AsSingle();

            Container.BindInterfacesAndSelfTo<UIFacade>().AsSingle();
            Container.BindInterfacesAndSelfTo<SpeedGameController>().AsSingle();
            Container.BindInterfacesAndSelfTo<ExitController>().AsSingle();
        }
    }
}