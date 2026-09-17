using System;

namespace Gameplay.Features.Battlesystem
{
    public class MoveStat
    {
        //Статы  
        private StatParam _moveSpeed;
        private StatParam _patrolRadius;
        
        public float MoveSpeed => _moveSpeed?.value ?? 0.000001f;
        public float PatrolRadius => _patrolRadius.value;

        
        //конструктор
        public MoveStat(StatParam moveSpeed, StatParam movePatrolRadius)
        {
            _moveSpeed = moveSpeed;
            _patrolRadius = movePatrolRadius;
        }
        
        
        // События
        public event Action OnMovementStarted;
        public event Action OnMovementStopped;
        // public event Action OnTargetReached;
        // public event Action<Vector3> OnTargetSet;
        
        
        // Свойства
        public bool isActive = true;
        public bool IsMoving { get; set; }
        private bool _wasMoving = false;
        // private Vector3 _currentPosition;
        // public Vector3 TargetPosition { get; private set; }
        
        
        // public event Action<Vector3> OnPositionChanged;
        
        
       
        
        public MoveStat()
        {
            // _currentPosition = Vector3.zero;
            // TargetPosition = Vector3.zero;
            // IsMoving = false;
        }
        
        // public void Init(Vector3 startPosition)
        // {
        //     _currentPosition = startPosition;
        //     TargetPosition = startPosition;
        // }
        //
        // public void SetPosition(Vector3 position)
        // {
        //     _currentPosition = position;
        // }
        //
        // public void UpdatePosition(Vector3 newPosition)
        // {
        //     if (_currentPosition != newPosition)
        //     {
        //         _currentPosition = newPosition;
        //         OnPositionChanged?.Invoke(newPosition);
        //     }
        // }
        
        public void UpdateMovementState()
        {
            if (IsMoving && !_wasMoving)
            {
                OnMovementStarted?.Invoke();
            }
            else if (!IsMoving && _wasMoving)
            {
                OnMovementStopped?.Invoke();
            }
            
            _wasMoving = IsMoving;
        }

        public void Clear()
        {
            OnMovementStarted = null;
            OnMovementStopped = null;
        }
        
        public void Reset()
        {
            //сброс системы. обнуление
        }
        
    }
}