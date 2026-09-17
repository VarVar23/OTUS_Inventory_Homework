namespace Gameplay.Features.Battlesystem
{
    public enum SpawnMode
    {
        Immediate,   // Все сразу на точках спавна
        Interval,    // Каждые N секунд из SpawnPoint
        OnDeath      // После смерти предыдущего юнита
    }
}