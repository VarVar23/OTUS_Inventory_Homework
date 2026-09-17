using UnityEngine;

namespace Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "InventorySettings", menuName = "Config/InventorySettings")]
    internal class InventorySettings : ScriptableObject
    {
        [field: SerializeField] public float MoveItemTime { get; private set; }
        [field: SerializeField] public float SnapDistance { get; private set; }
        [field: SerializeField] public bool SwapItems { get; private set; }

        [field: SerializeField] public float MoveInventoryTime { get; private set; }
        [field: SerializeField] public float OpenInventoryY { get; private set; }
        [field: SerializeField] public float CloseInventoryY { get; private set; }

        [field: SerializeField] public float MoveScrollToTargetTime { get; private set; }
    }
}
