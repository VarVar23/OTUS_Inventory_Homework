using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    /// <summary>
    /// Глобальный реестр всех юнитов на сцене.
    /// Отвечает за хранение и базовые запросы.
    /// Внутренний — для низкоуровневых систем.
    /// </summary>
    internal static class UnitRegistry
    {
        private static Dictionary<Guid, Unit> _units = new Dictionary<Guid, Unit>();

        // === РЕГИСТРАЦИЯ ===

        public static void Register(Unit unit)
        {
            if (unit != null && !_units.ContainsKey(unit.Id))
            {
                _units[unit.Id] = unit;
            }
        }

        public static void Unregister(Unit unit)
        {
            if (unit != null)
            {
                _units.Remove(unit.Id);
            }
        }

        // === БАЗОВЫЕ ЗАПРОСЫ ===

        public static IEnumerable<Unit> GetAllUnits() => _units.Values;

        public static Unit GetByGuid(Guid id)
            => _units.TryGetValue(id, out var unit) ? unit : null;

        public static IEnumerable<Unit> GetByTeam(Team team)
            => _units.Values.Where(u => u.UnitTeam == team);

        public static int GetAliveCount(Team team)
            => _units.Values.Count(u => u.UnitTeam == team && !u.IsDead);

        // === ПРОСТРАНСТВЕННЫЕ ЗАПРОСЫ (Поиск врагов) ===

        /// <summary>
        /// Найти ближайшего врага
        /// </summary>
        public static (Unit, float) GetNearestEnemy(Vector2 position, Team myTeam, float maxRange = Mathf.Infinity)
        {
            Unit nearest = null;
            float minDist = Mathf.Infinity;

            foreach (var unit in _units.Values)
            {
                if (unit == null || unit.IsDead || unit.UnitTeam == myTeam) continue;

                float dist = Vector2.Distance(position, unit.transform.position);
                if (dist <= maxRange && dist < minDist)
                {
                    minDist = dist;
                    nearest = unit;
                }
            }

            return (nearest, minDist);
        }

        /// <summary>
        /// Найти всех врагов в радиусе
        /// </summary>
        public static List<Unit> GetEnemiesInRange(Vector2 position, Team myTeam, float range)
        {
            List<Unit> result = new List<Unit>();

            foreach (var unit in _units.Values)
            {
                if (unit == null || unit.IsDead || unit.UnitTeam == myTeam) continue;

                float dist = Vector2.Distance(position, unit.transform.position);
                if (dist <= range)
                {
                    result.Add(unit);
                }
            }

            return result;
        }

        /// <summary>
        /// Проверить, есть ли враг в радиусе (быстрая проверка)
        /// </summary>
        public static bool HasEnemyInRange(Vector2 position, Team myTeam, float range)
        {
            foreach (var unit in _units.Values)
            {
                if (unit == null || unit.IsDead || unit.UnitTeam == myTeam) continue;

                float dist = Vector2.Distance(position, unit.transform.position);
                if (dist <= range) return true;
            }

            return false;
        }

        // === ОТЛАДКА ===

        public static void PrintDebugInfo()
        {
            Debug.Log($"=== UnitRegistry Debug ===");
            Debug.Log($"Total units: {_units.Count}");
            Debug.Log($"Player alive: {GetAliveCount(Team.Player)}");
            Debug.Log($"Enemy alive: {GetAliveCount(Team.Enemy)}");
        }

        public static void Clear()
        {
            _units.Clear();
        }
    }
}