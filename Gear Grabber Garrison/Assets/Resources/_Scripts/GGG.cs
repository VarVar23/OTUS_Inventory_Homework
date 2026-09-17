using DG.Tweening;
using UnityEngine;

public class GGG : MonoBehaviour
{
    [SerializeField] private Vector2 _target;
    [SerializeField] private float _time;
    private Enemy _enemy;

    private void Update()
    {
        _enemy.Update(_target, _time);
    }
}

public class Enemy
{
    private EnemyMoveController _moveController;
    private EnemyCheckCollider _checkCollider;

    public void Update(Vector2 targetPosition, float time)
    {
        if(_checkCollider.EnemyCollisionWithPlayer() == false)
        {
            _moveController.Move(targetPosition, time);
        }
    }
}

public class EnemyMoveController
{
    private Transform _enemy;

    public void Move(Vector2 targetPosition, float time)
    {
        _enemy.DOMove(targetPosition, time);
    }
}

public class EnemyCheckCollider
{
    public bool EnemyCollisionWithPlayer()
    {
        return true;
    }
}