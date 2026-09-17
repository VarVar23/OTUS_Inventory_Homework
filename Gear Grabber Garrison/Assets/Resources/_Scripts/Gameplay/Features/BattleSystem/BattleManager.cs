using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay.Features.Battlesystem
{
    public class BattleManager : MonoBehaviour, ISystemTickable
    {
        [Inject] private TickManager  _tickManager;
        public PlayerFactory PlayerFactory => _playerFactory;
        [Inject] private PlayerFactory _playerFactory;
        public EnemyFactory EnemyFactory => _enemyFactory;
        [Inject] private EnemyFactory _enemyFactory;
         
        [SerializeField] private BattleScenario _scenario;
        
        private Dictionary<Guid, Army> _playerArmies = new Dictionary<Guid, Army>();
        private List<Army> _currentEnemyArmies = new List<Army>();
        
        private ArmyConfig PlayerArmyConfig => _scenario.PlayerArmy;
        private WaveConfig EnemyWaveConfig => _scenario.Waves;
        
        private int _currentWave;
        private float _waveTimer;
        private bool _waitingForNextWave;
        private bool _battleStarted;

        public bool IsBattleActive => _battleStarted;
        public int CurrentWave => _currentWave;
        public int TotalWaves => _scenario?.Waves?.Armies?.Count ?? 0;

       
        public void Start()
        {
            if (_scenario == null)
            { 
                Debug.LogError("[BattleManager] BattleScenario is not assigned!");
                return;
            }
            
            _battleStarted = true;
            CreateEnemyFromWave(0);
            // var guid = Guid.NewGuid();
            //  _playerArmies.Add(guid, new Army(guid, PlayerArmyConfig, _playerFactory));
            TickManager.Instance.RegisterTickable(this);
        }

        //Игрок Создает армию
        public void CreatePlayerArmy(Guid guid)
        {
            //передаем guid
            Army newArmy = new Army(guid, PlayerArmyConfig, _playerFactory);
            //Добавляем в словарь армию
            _playerArmies.Add(guid, newArmy);
            // Debug.Log("[BattleManager] Adding player army: "+guid+"! count armies = " +_playerArmies.Count);
        }
        
        
        public void OnTick(float dt)
        {
            if (!_battleStarted || _scenario == null) return;

            foreach (var playerArmy in _playerArmies)
            {
                playerArmy.Value.SpawnTick(dt);
            }

            foreach (var enamyArmy in _currentEnemyArmies)
                enamyArmy.SpawnTick(dt);

            if (_waitingForNextWave)
            {
                _waveTimer += dt;
                if (_waveTimer >= _scenario.IntervalBetweenWaves)
                {
                    _waitingForNextWave = false;
                    _waveTimer = 0f;
                    CreateEnemyFromWave(_currentWave);
                }
                return;
            }

            if (IsWaveComplete())
            {
                _currentWave++;
                if (_currentWave < _scenario.Waves.Armies.Count)
                {
                    _waitingForNextWave = true;
                    _waveTimer = 0f;
                }
                else
                {
                    OnVictory();
                }
            }
        }
        
        
        
        private void CreateEnemyFromWave(int waveIndex)
        {
            var waves = _scenario.Waves;
            _currentEnemyArmies.Clear();

            if (waveIndex >= waves.Armies.Count) return;

            var enamyArmyConfig = waves.Armies[waveIndex];
            if (enamyArmyConfig != null)
            {
                Army EnamyArmy = new Army(enamyArmyConfig, _enemyFactory);
                _currentEnemyArmies.Add(EnamyArmy);
            }

            Debug.Log($"[BattleManager] Wave {waveIndex} started");
        }

        private bool IsWaveComplete()
        {
            if (_currentEnemyArmies.Count == 0) return false;
            return _currentEnemyArmies.All(a => a.IsComplete);
        }

        private void OnVictory()
        {
            _battleStarted = false;
            Debug.Log("[BattleManager] Victory! All waves completed.");
        }

         public Army GetPlayerArmy(Guid guid) => _playerArmies[guid];

        private void OnDestroy()
        {
            _tickManager?.UnregisterTickable(this);
            foreach (var army in _currentEnemyArmies)
                army.Clear();
            foreach (var army in _playerArmies)
                army.Value.Clear();
        }
    }
}