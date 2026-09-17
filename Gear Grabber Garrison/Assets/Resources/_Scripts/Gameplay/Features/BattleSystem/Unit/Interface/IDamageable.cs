namespace Gameplay.Features.Battlesystem
{
    public interface IDamageable : IHealthable
    {
        void TakeDamage(int damage);
        bool IsAlive { get; }
    }
}