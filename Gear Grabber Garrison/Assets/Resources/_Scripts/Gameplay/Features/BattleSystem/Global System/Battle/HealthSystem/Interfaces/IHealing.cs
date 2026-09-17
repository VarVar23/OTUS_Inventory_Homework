namespace Gameplay.Features.Battlesystem
{
    public interface IHealing
    {
        void SetStat(HealthStat stat);
        void SetOwnerSystem(I_System owner);
    }
}