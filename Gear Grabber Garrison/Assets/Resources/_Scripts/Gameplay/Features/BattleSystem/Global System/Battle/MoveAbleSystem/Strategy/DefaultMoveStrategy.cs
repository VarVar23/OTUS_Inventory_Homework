using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class DefaultMoveStrategy : I_Strategy, ISystemTickable
    {
        private I_System _owner;
        private MoveStat _stat;
        private ReadOnlyAttributes _attributes;
        private Vector3 _tacticalTarget;
        private Vector3 _supremeTarget;
        private Vector3 _target;

        public void SetOwner(I_System owner)
        {
            _owner = owner;
        }
        public void SetStat(MoveStat stat)
        {
            _stat = stat;
        }

        public DefaultMoveStrategy(I_System owner, MoveStat stat)
        {
            _stat = stat;
            _owner = owner;
        }

        public void SetSupremeTarget(Vector3 target)
        {
            _supremeTarget = target;
        }

        public bool FindStrategyTarget()
        {
            
            if (_owner == null) return false;

            Team team = _owner.Owner.UnitTeam;
            Team enemyTeam = team == Team.Player ? Team.Enemy : Team.Player;

            if (UnitRegistry.GetAliveCount(enemyTeam) == 0)
            {
                _target = _supremeTarget;
                return false;
            }

            var (enemy, distance) = UnitRegistry.GetNearestEnemy(_owner.Owner.Position, team);

            if (enemy != null && distance <= _stat.PatrolRadius)
            {
                _tacticalTarget = enemy.Position;
                _target = _tacticalTarget;
                return true;
            }

            _target = _supremeTarget;
            return false;
        }

        public void OnTick(float dt)
        {
            Debug.Log("FindStrategyTarget");
            if (_stat.isActive == false)
            {
                _stat.isActive = true;
                return;
            }

            if (_owner != null && !(_owner as Unit).IsDead)
            {
                FindStrategyTarget();
                var speed = _stat.MoveSpeed;

                var currentPosition = _owner.Owner.Position;
                var targetPosition = _target;

                _stat.IsMoving = MovementUtility.TryMove(ref currentPosition, targetPosition, speed, dt);
                if (_stat.IsMoving)
                {
                    _owner.Owner.Position = currentPosition;
                    _owner.Owner.LookAt(targetPosition);
                }
            }
            else
                _stat.IsMoving = false;

            _stat.UpdateMovementState();
        }
    }
}