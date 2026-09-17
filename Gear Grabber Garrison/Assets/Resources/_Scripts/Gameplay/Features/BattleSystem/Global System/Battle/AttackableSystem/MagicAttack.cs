// using UnityEngine;
// using System;
//
// public class MagicAttack : IAttacking 
// {
//     private float _distance;
//     private float _attackTime => 1f / _config.AttackSpeed;
//     private float _tickAccumulator;
//     private Unit _enemy;
//     private Unit _owner;
//     private AttackConfig _config;
//     private bool _isCasting;
//     
//     public event Action OnAttackStarted;
//     public event Action OnAttackHit;
//     public event Action OnAttackEnded;
//     
//     public void SetConfig(AttackConfig config)
//     {
//         _config = config;
//     }
//     
//     public bool EnemySearch(Unit unit)
//     {
//         _owner = unit;
//         
//         if (_enemy == null || _enemy.IsDead)
//         {
//             if (_isCasting)
//             {
//                 OnAttackEnded?.Invoke();
//                 _isCasting = false;
//             }
//             _enemy = null;
//             _tickAccumulator = 0f;
//             (_enemy, _distance) = UnitRegistry.GetNearestEnemy(unit.Position, unit.Team, _config.AttackRange);
//             if (_enemy == null)
//                 return false;
//         }
//         
//         float currentDistance = Vector3.Distance(unit.Position, _enemy.Position);
//         bool hasTarget = currentDistance <= _config.AttackRange;
//         
//         if (hasTarget && !_isCasting)
//         {
//             _isCasting = true;
//             OnAttackStarted?.Invoke();
//         }
//         else if (!hasTarget && _isCasting)
//         {
//             _isCasting = false;
//             OnAttackEnded?.Invoke();
//         }
//         
//         return hasTarget;
//     }
//
//     public Unit GetCurrentEnemy()
//     {
//         return _enemy;
//     }
//
//     public void Tick(float dt)
//     {
//         bool isRivalsNotExist = (_enemy == null || _enemy.IsDead || _owner.IsDead);
//         
//         if (isRivalsNotExist)
//         {
//             if (_isCasting)
//             {
//                 OnAttackEnded?.Invoke();
//                 _isCasting = false;
//             }
//             _enemy = null;
//             _tickAccumulator = 0f;
//             return;
//         }
//         
//         _tickAccumulator += dt;
//         while (_tickAccumulator >= _attackTime)
//         {
//             int damage = UnityEngine.Random.Range(_config.MinDamage, _config.MaxDamage + 1);
//             (_enemy as IDamageable)?.TakeDamage(damage);
//             OnAttackHit?.Invoke();
//             
//             if (_config.StunChance > 0 && _enemy.StunSystem != null)
//             {
//                 _enemy.StunSystem.TryApplyStun(_config.StunChance, _config.StunDuration);
//             }
//             
//             _tickAccumulator -= _attackTime;
//         }
//     }
// }
