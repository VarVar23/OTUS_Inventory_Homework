using Zenject;

namespace Gameplay.Input
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInputStateReader>().To<InputSystemStateReader>().AsSingle();
        }
    }
}