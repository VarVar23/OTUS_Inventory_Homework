namespace Gameplay.Features.Battlesystem
{
    public interface ISystemTickable
    {
        void OnTick(float deltaTime);
    }
}