using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Gameplay.TimeSystem
{
    public class TimeManager : ITickable
    {
        public static TimeManager Instance;
        public float BaseTicksPerSecond = 24f;
        public float TimeScale = 1f;

        public event Action OnTick;
        
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


        
        private float _tickAccumulator = 0f;
        public float GameTime =>_gameTime;
        public float DeltaTime { get; private set; }
        private  float  _gameTime;
        private string _debugText;

        // private void Awake()
        // {
        //     if (Instance == null)
        //     {
        //         Instance = this;
        //         DontDestroyOnLoad(gameObject);
        //     }
        //     else
        //         Destroy(gameObject);
        // }
         
        public void Tick()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.P))
                IsPaused = !IsPaused;
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftBracket))
                TimeScale = Mathf.Max(_minTimeScale, TimeScale - _timeScaleStep);
            
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightBracket))
                TimeScale = (int)Mathf.Min(_maxTimeScale, TimeScale + _timeScaleStep);
            
            
            _debugText = "Общее время : " + (int)_gameTime +"сек.  <- '[' TimeScale:" + TimeScale + " ']' -> . 'P' -пауза.";
            
            if(IsPaused) return;
            
            _tickAccumulator += Time.deltaTime;
            float dt = 1f / BaseTicksPerSecond;
            float effectiveInterval = dt / TimeScale;

            while (_tickAccumulator >= effectiveInterval)
            {
                _tickAccumulator -= effectiveInterval;
                DoTick(dt);  
            } 
        }

        private void DoTick(float dt)
        {
            DeltaTime = dt;
            OnTick?.Invoke();
            _gameTime += dt;
        }

      
        // Полезный метод для отладки - показать текущее состояние
        public string GetDebugInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== TickManager Debug Info ===");
             
            return sb.ToString();
        }
        
    
        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 700, 400),
                $"{_debugText}\n\n{GetDebugInfo()}");
        }
    }
}