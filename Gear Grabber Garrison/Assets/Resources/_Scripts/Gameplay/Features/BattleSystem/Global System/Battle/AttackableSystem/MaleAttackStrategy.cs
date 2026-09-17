using UnityEngine;
using System;

namespace Gameplay.Features.Battlesystem
{
    public class MaleAttackStrategy : IAttacking, I_Strategy
    {
        private I_System _owner;
        private AttackStat _stat;
        private AttackSystem _attackSystem => _owner as AttackSystem;
        public void SetOwner(I_System owner)
        {
            _owner = owner;
        }
        public MaleAttackStrategy(I_System owner, AttackStat stat)
        {
            _stat = stat;
            _owner = owner;
            _unit = _owner.Owner as Unit;
        }
        
        private float _distance;
        private float _attackTime;                              
        private float _tickAccumulator;
        private Unit _unit;
        private Unit _enemy;
        private bool _isAttacking;
 
        public event Action<Unit, float> OnStunRequested;

        public bool EnemySearch(Unit unit)
        {
            // _unit = unit;
            
            if (_enemy == null || _enemy.IsDead)
            {
                _enemy = null;
                _tickAccumulator = 0f;
                _isAttacking = false;
                (_enemy, _distance) = UnitRegistry.GetNearestEnemy(unit.Position, unit.UnitTeam, _stat.AttackRange); 
                if (_enemy == null)
                    return false;
            }
            
            // float currentDistance = Vector3.Distance(unit.Position, _enemy.Position);
            // return currentDistance <= _stat.AttackRange;
            return true;
        }

        public Unit GetCurrentEnemy()
        {
            return _enemy;
        }
        public void OnTick(float dt)
        {
            if (!_stat.IsEnable) return;
            // Debug.Log("[стратегия ближний бой]>>> maxDamage:" + _stat.MaxDamage+" / speed:" + _stat.AttackSpeed+" / range:" + _stat.AttackRange);
            
            if (_unit != null && _unit.IsDead)
            {
                if (_isAttacking)
                {
                    _stat.AttackEnded();
                    _isAttacking = false;
                }
                _enemy = null;
                _tickAccumulator = 0f;
                return;
            }

            if (_enemy == null || _enemy.IsDead)
            {
                if (_isAttacking)
                {
                    _stat.AttackEnded();
                    _isAttacking = false;
                }
                _enemy = null;
                _tickAccumulator = 0f;
                if(!_attackSystem.TryAttack(_unit, _stat.AttackRange, ref _enemy))
                    return;
            }
            
            
            //цикл атакти
            var moveSys = _unit.Components.TryGetSystem(SystemType.MoveSystem) as MoveableSystem;
            if (moveSys != null)
                moveSys.Stat.isActive = false;

            _attackTime = 1f / _stat.AttackSpeed;
            _tickAccumulator += dt;
            
            if (_tickAccumulator >= _attackTime && !_isAttacking)
            {
                _isAttacking = true;
                _stat.AttackStarted();
            }
            
            while (_tickAccumulator >= _attackTime)
            {
                if (_enemy == null || _enemy.IsDead || (_unit != null && _unit.IsDead))
                {
                    if (_isAttacking)
                    {
                        _stat.AttackEnded();
                        _isAttacking = false;
                    }
                    _enemy = null;
                    _tickAccumulator = 0f;
                    return;
                }

                int damage = _stat.MaxDamage;
                // int damage = UnityEngine.Random.Range(_stat.MinDamage, _stat.MaxDamage + 1);
                (_enemy as IDamageable)?.TakeDamage(damage);
                _stat.AttackHit();
                
                //оглушение
                // if (_stat.StunChance > 0)
                // {
                //     OnStunRequested?.Invoke(_enemy, _stat.StunDuration);
                // }
                _tickAccumulator = 0;
            }
            
            if (_isAttacking && _tickAccumulator == 0)
            {
                _stat.AttackEnded();
                _isAttacking = false;
            }
        }
    }
}