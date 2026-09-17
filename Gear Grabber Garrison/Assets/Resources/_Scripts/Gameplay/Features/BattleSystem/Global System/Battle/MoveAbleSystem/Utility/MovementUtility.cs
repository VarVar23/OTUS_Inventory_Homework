using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public static class MovementUtility
    {
        public static bool TryMove(ref Vector3 currentPosition, Vector3 targetPosition, float speed, float dt)
        {
            Vector3 moveDirection = targetPosition - currentPosition;
            float distance = moveDirection.magnitude;
            float moveDistance = dt * speed;
            if (distance <= moveDistance)
            {
                //пришли
                currentPosition = targetPosition;
                return false;
            }
            else
            {
                //идем
                Vector3 newPosition = currentPosition + moveDirection.normalized * moveDistance;
                currentPosition = newPosition;
                return true;
            }
        }
    }
}