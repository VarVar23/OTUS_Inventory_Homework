using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
     
    public enum TickPriority
    {
        Player = 0,
        Enemy = 1,
        Default = 2
    }

    public class TickManager
    {
        public static TickManager Instance { get; private set; }
        public TickManager()
        {
            Instance = this;
            InitializeTickables();
        }
        public float BaseTicksPerSecond = 24f;
        public float TimeScale = 1f;
        
        public event System.Action<bool> OnPauseChanged;
        private bool _isPaused;
        
        private float _timeScaleStep = 1f;
        private float _minTimeScale = 0.5f;
        private float _maxTimeScale = 300f;
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused != value)
                {
                    _isPaused = value;
                    OnPauseChanged?.Invoke(_isPaused);
                }
            }
        }

        // Двумерная структура: [SystemType][Priority] -> List<ITickable>
        private Dictionary<SystemType, List<ISystemTickable>[]> _tickablesBySystemAndPriority;
        
        // Кеш для быстрого доступа к порядку SystemType
        private SystemType[] _systemTypeOrder;
        
        private float _tickAccumulator = 0f;
        public float GameTime =>_gameTime;
        private  float  _gameTime;
        private string _debugText;

        public void InitializeTickables()
        {
            int priorityCount = System.Enum.GetValues(typeof(TickPriority)).Length;
            var systemTypes = System.Enum.GetValues(typeof(SystemType));
            
            _tickablesBySystemAndPriority = new Dictionary<SystemType, List<ISystemTickable>[]>();
            _systemTypeOrder = new SystemType[systemTypes.Length];
            
            int index = 0;
            SystemType noneValue = SystemType.None;
            
            foreach (SystemType systemType in systemTypes)
            {
                // None обрабатываем отдельно - поставим в конец
                if (systemType == SystemType.None)
                {
                    noneValue = systemType;
                    continue;
                }
                
                _systemTypeOrder[index++] = systemType;
                _tickablesBySystemAndPriority[systemType] = new List<ISystemTickable>[priorityCount];
                for (int i = 0; i < priorityCount; i++)
                    _tickablesBySystemAndPriority[systemType][i] = new List<ISystemTickable>();
            }
            
            // None ставим в конец - это не-системные тикаблы (юниты, фабрики и т.д.)
            _systemTypeOrder[index] = noneValue;
            _tickablesBySystemAndPriority[noneValue] = new List<ISystemTickable>[priorityCount];
            for (int i = 0; i < priorityCount; i++)
                _tickablesBySystemAndPriority[noneValue][i] = new List<ISystemTickable>();
        }

        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.P))
        //         IsPaused = !IsPaused;
        //     
        //     if (Input.GetKeyDown(KeyCode.LeftBracket))
        //         TimeScale = Mathf.Max(_minTimeScale, TimeScale - _timeScaleStep);
        //     
        //     if (Input.GetKeyDown(KeyCode.RightBracket))
        //         TimeScale = (int)Mathf.Min(_maxTimeScale, TimeScale + _timeScaleStep);
        //     
        //     
        //     _debugText = "Общее время : " + (int)_gameTime +"сек.  <- '[' TimeScale:" + TimeScale + " ']' -> . 'P' -пауза.";
        //     
        //     
        //     
        //     if(IsPaused) return;
        //     
        //     _tickAccumulator += Time.deltaTime;
        //     float dt = 1f / BaseTicksPerSecond;
        //     float effectiveInterval = dt / TimeScale;
        //
        //     while (_tickAccumulator >= effectiveInterval)
        //     {
        //         _tickAccumulator -= effectiveInterval;
        //         DoTick(dt);  
        //     } 
        //     
        // }

        public void DoTick(float dt)
        {
            
            // Первый цикл: перебираем типы систем в порядке enum
            // None будет последним (не-системные тикаблы: юниты, фабрики и т.д.)
            foreach (var systemType in _systemTypeOrder)
            {
                var priorityArrays = _tickablesBySystemAndPriority[systemType];
                
                // Второй цикл: перебираем приоритеты внутри типа системы
                for (int priority = 0; priority < priorityArrays.Length; priority++)
                {
                    var tickables = priorityArrays[priority];
                    
                    // Тикаем все системы данного типа с данным приоритетом
                    for (int i = tickables.Count - 1; i >= 0; i--)
                    {
                        if (tickables[i] != null)
                            tickables[i].OnTick(dt);
                        else
                            tickables.RemoveAt(i);
                    }
                }
            }
            
            _gameTime += dt;
        }

        public void RegisterTickable(ISystemTickable systemTickable, SystemType type = SystemType.None, TickPriority priority = TickPriority.Default)
        {
            if (systemTickable == null) return;
            
            if (_tickablesBySystemAndPriority.TryGetValue(type, out var priorityArrays))
            {
                var list = priorityArrays[(int)priority];
                if (!list.Contains(systemTickable))
                    list.Add(systemTickable);
            }
        }

        public void UnregisterTickable(ISystemTickable systemTickable)
        {
            if (systemTickable == null) return;
            
            // Ищем и удаляем из всех типов систем и приоритетов
            foreach (var kvp in _tickablesBySystemAndPriority)
            {
                for (int i = 0; i < kvp.Value.Length; i++)
                {
                    kvp.Value[i].Remove(systemTickable);
                }
            }
        }
        
        // Полезный метод для отладки - показать текущее состояние
        public string GetDebugInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== TickManager Debug Info ===");
            
            foreach (var systemType in _systemTypeOrder)
            {
                if (systemType == SystemType.None) continue;
                
                var priorityArrays = _tickablesBySystemAndPriority[systemType];
                int totalTickables = 0;
                for (int i = 0; i < priorityArrays.Length; i++)
                    totalTickables += priorityArrays[i].Count;
                
                if (totalTickables > 0)
                {
                    sb.AppendLine($"\n{systemType}: {totalTickables} tickables");
                    for (int i = 0; i < priorityArrays.Length; i++)
                    {
                        if (priorityArrays[i].Count > 0)
                            sb.AppendLine($"  Priority {(TickPriority)i}: {priorityArrays[i].Count}");
                    }
                }
            }
            
            return sb.ToString();
        }
        
    
        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 700, 400),
                $"{_debugText}\n\n{GetDebugInfo()}");
        }
    }
}