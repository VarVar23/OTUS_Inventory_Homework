namespace Gameplay.Features.Battlesystem
{
    public interface IDestructible : IHealthable
    {
        void OnDestroyed();
    }
}