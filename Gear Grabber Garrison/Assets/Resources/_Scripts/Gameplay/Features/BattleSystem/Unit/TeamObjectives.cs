using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public static class TeamObjectives
    {
        public static Vector3 PlayerBasePosition { get; set; }
        public static Vector3 EnemyBasePosition { get; set; }

        public static Vector3 GetEnemyBase(Team myTeam)
        {
            return myTeam == Team.Player ? EnemyBasePosition : PlayerBasePosition;
        }

        public static void SetBasePosition(Team team, Vector3 position)
        {
            if (team == Team.Player)
                PlayerBasePosition = position;
            else
                EnemyBasePosition = position;
        }
    }
}