using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Itemization;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class Army
    {
        //Ария получает фабрику и кофиг. Создает юнитов автоматически по сценарию
        public event Action<Unit, float> OnArmySpawn;
        private readonly ArmyConfig _config;
        private readonly IUnitFactory _factory;
        private readonly List<Unit> _activeUnits = new List<Unit>();
        private readonly List<(Unit unit, float respawnTime)> _respawnQueue = new List<(Unit, float)>();
        private float _spawnTimer;
        private int _spawnedCount;
        private float _elapsedTime;
        private bool _allSpawned;

        public Guid Id { get; }
        public ArmyConfig Config => _config;
        public int AliveCount => _activeUnits.Count(u => u != null && u.IsAlive);
        public int SpawnedCount => _spawnedCount;

        public bool IsComplete
        {
            get
            {
                bool allDead = _allSpawned && _activeUnits.All(u => u == null || u.IsDead);
                bool survived = _config.SurviveTime > 0f && _elapsedTime >= _config.SurviveTime;
                return allDead || survived;
            }
        }

        public Army(Guid guid, ArmyConfig config, IUnitFactory factory)
        {
            _config = config ?? new ArmyConfig();
            _factory = factory;
            Id = guid;
        }
        public Army (ArmyConfig config, IUnitFactory factory)
        {
            _config = config ?? new ArmyConfig();
            _factory = factory;
            Id = Guid.NewGuid();
        }
        
        
        //!!!!!!!Спавн Юнита!!!!!!!!!!//
        private void SpawnUnit(int index)
        {
            var (spawnPoint, targetPoint) = GetSpawnAndTarget(index);

            Vector3 position = spawnPoint != null
                ? spawnPoint.transform.position
                : Vector3.zero;

            float offsetX = UnityEngine.Random.Range(-_config.SpawnSpreadX, _config.SpawnSpreadX);
            position.x += offsetX;

            var target = targetPoint != null ? targetPoint.GetComponent<ISystemableEntity>() : null;
            var unit = _factory.Create(_config.UnitPrefab, position, target, this);
            if (unit == null)
                return;

            unit.RespawnTime = _config.RespawnDelay;
            unit.OnRespawnStarted += OnUnitRespawnStarted;
            _activeUnits.Add(unit);
            
            _spawnedCount++;
        }

        public void SpawnTick(float dt)
        {
            ProcessRespawnQueue(dt);
            if (IsComplete)
                return;

            _elapsedTime += dt;

            switch (_config.SpawnMode)
            {
                case SpawnMode.Immediate:
                    SpawnAllImmediately();
                    break;
                case SpawnMode.Interval:
                    SpawnWithInterval(dt);
                    break;
                case SpawnMode.OnDeath:
                    SpawnOnDeath();
                    break;
            }


        }

        private void SpawnAllImmediately()
        {
            if (_allSpawned) return;

            for (int i = 0; i < _config.UnitCount; i++)
            {
                SpawnUnit(i);
            }
            _allSpawned = true;
        }

        private void SpawnWithInterval(float dt)
        {
            if (_allSpawned) return;

            _spawnTimer += dt;
            if (_spawnTimer >= _config.SpawnInterval)
            {
                _spawnTimer = 0f;
                SpawnUnit(_spawnedCount);

                if (_spawnedCount >= _config.UnitCount)
                    _allSpawned = true;
            }
        }

        private void SpawnOnDeath()
        {
            if (_allSpawned) return;

            int deadCount = _activeUnits.Count(u => u == null || u.IsDead);
            while (_spawnedCount < _config.UnitCount && _spawnedCount < deadCount + 1)
            {
                SpawnUnit(_spawnedCount);
            }

            if (_spawnedCount >= _config.UnitCount)
                _allSpawned = true;
        }

        

        private (GameObject spawn, GameObject target) GetSpawnAndTarget(int index)
        {
            GameObject spawnPoint = _config.SpawnPoint;
            GameObject targetPoint = _config.TargetPoint;

            if (index < _config.UnitOverrides?.Count && _config.UnitOverrides[index] != null)
            {
                var overrideData = _config.UnitOverrides[index];
                if (overrideData.SpawnPoint != null)
                    spawnPoint = overrideData.SpawnPoint;
                if (overrideData.TargetPoint != null)
                    targetPoint = overrideData.TargetPoint;
                else if (overrideData.SpawnPoint != null && _config.TargetPoint == null)
                    targetPoint = overrideData.SpawnPoint;
            }

            return (spawnPoint, targetPoint);
        }

        private void OnUnitRespawnStarted(Unit unit)
        {
            _activeUnits.Remove(unit);

            if (_config.CanRespawn)
            {
                // Debug.Log("Юнит " + unit.name + " умер. Случился респавн = " + _config.RespawnDelay);
                OnArmySpawn?.Invoke(unit,_config.RespawnDelay);
                _respawnQueue.Add((unit, _config.RespawnDelay));
            }
        }

        private void ProcessRespawnQueue(float dt)
        {
            for (int i = _respawnQueue.Count - 1; i >= 0; i--)
            {
                var item = _respawnQueue[i];
                item.respawnTime -= dt;

                if (item.respawnTime <= 0f)
                {
                    RespawnUnit(item.unit);
                    _respawnQueue.RemoveAt(i);
                }
                else
                {
                    _respawnQueue[i] = item;
                }
            }
        }

        private void RespawnUnit(Unit unit)
        {
            if (unit == null) return;

            var (spawnPoint, targetPoint) = GetSpawnAndTarget(_spawnedCount);

            Vector3 position = spawnPoint != null
                ? spawnPoint.transform.position
                : Vector3.zero;

            float offsetX = UnityEngine.Random.Range(-_config.SpawnSpreadX, _config.SpawnSpreadX);
            position.x += offsetX;

            unit.Position = position;
            unit.transform.position = position;
            unit.Enable();
            unit.SetSupremeTarget(targetPoint != null ? targetPoint.GetComponent<ISystemableEntity>() : null);

            _activeUnits.Add(unit);
            _spawnedCount++;
        }

        public void UpgradeAllUnits(StatType stat, Func<float,float> valueChangeAction)
        {
            Func<float,float> change = valueChangeAction;
            foreach (var unit in _activeUnits)
            {
                if (unit != null)
                {
                    float curentState = unit.GetWritableAttributes().GetValue(stat);
                    var newValue = valueChangeAction(curentState);
                    unit.GetWritableAttributes().SetValue(stat, newValue);
                    unit.ClearAttributeCache();
                    Debug.Log($"[Army]unit count = {_activeUnits.Count}. UpgradeAllUnits(stat: {stat}, newValue: {newValue})");
                    
                }
            }

            foreach (var (unit, _) in _respawnQueue)
            {
                if (unit != null)
                {
                    float curentState = unit.GetWritableAttributes().GetValue(stat);
                    var newValue = valueChangeAction(curentState);
                    unit.GetWritableAttributes().SetValue(stat, newValue);
                }
            }
        }

        public IEnumerable<Unit> GetAliveUnits()
        {
            return _activeUnits.Where(u => u != null && u.IsAlive);
        }

        public void Clear()
        {
            foreach (var unit in _activeUnits)
            {
                if (unit != null)
                {
                    unit.OnRespawnStarted -= OnUnitRespawnStarted;
                    UnityEngine.Object.Destroy(unit.gameObject);
                }
            }
            _activeUnits.Clear();
            _respawnQueue.Clear();
        }
    }
}