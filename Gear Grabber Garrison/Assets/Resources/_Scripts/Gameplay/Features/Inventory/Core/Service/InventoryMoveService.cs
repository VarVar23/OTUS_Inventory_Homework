using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryMoveService
    {
        private InventoryMoveItemController _moveController;
        private InventoryItemParentController _parentController;

        public InventoryMoveService(InventoryMoveItemController moveController, InventoryItemParentController parentController)
        {
            _moveController = moveController;
            _parentController = parentController;
        }

        public void OnMoveToCell(Transform item, Transform target)
        {
            _parentController.SetPriorityParent(item);
            MoveAnimation(item, target, () => _parentController.SetParent(item, target));
        }

        public void MoveAnimation(Transform item, Transform target, System.Action onComplete) => _moveController.MoveAnimation(item, target.position, onComplete);
        
        public void MoveWithoutAnimation(Transform item, Vector3 target) => _moveController.MoveWithoutAnimation(item, target);
       
        public void AddCursorMovable(Transform movable) => _moveController.AddCursorMovable(movable);
      
        public void RemoveCursorMovable() => _moveController.RemoveCursorMovable();
    }
}