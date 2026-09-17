using UnityEngine;
using Zenject;
namespace Gameplay.TimeSystem{

    public class TimeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TimeFacade>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TimeManager>().AsSingle().NonLazy();
            Debug.Log("[TimeInstaller] Started!");
        }
    }
}
   
