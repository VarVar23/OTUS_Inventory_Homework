using System.Linq;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class AttackTargetSerch  
    {
        public Unit EnemySearch(Unit unit, float range)
        {
            if (unit == null)
                return null;
             
            Unit enemy;
            float distance; 
            (enemy, distance) = UnitRegistry.GetNearestEnemy(unit.Position, unit.UnitTeam, range); 
            // Debug.Log("enemy:"+enemy+" >> distance:"+distance);
            if (enemy != null)
                return enemy;
                
                // if(distance <= range) return true;
            // float currentDistance = Vector3.Distance(unit.Position, _enemy.Position);
            // return currentDistance <= _stat.AttackRange;
            return null;
        }
    }
}
