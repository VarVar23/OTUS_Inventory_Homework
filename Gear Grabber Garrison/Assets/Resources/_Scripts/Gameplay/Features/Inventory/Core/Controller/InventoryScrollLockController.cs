using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class InventoryScrollLockController
    {
        private ScrollRect _scrollRect;

        public InventoryScrollLockController(UnitsView unitsView)
        {
            _scrollRect = unitsView.ScrollRect;
        }

        public void LockMove(bool locked)
        {
            _scrollRect.enabled = !locked;
        }
    }
}
