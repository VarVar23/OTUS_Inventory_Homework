using System;

namespace Gameplay.Features.Battlesystem
{
    interface IAttacking : ISystemTickable
    {
        bool EnemySearch(Unit unit);
        Unit GetCurrentEnemy();
        new void OnTick(float dt);   
   
        event Action<Unit, float> OnStunRequested;
    }
}
