using UnityEngine;

namespace Gameplay.Inventory
{
    [CreateAssetMenu(fileName = "InventoryItemSettings", menuName = "Config/InventoryItemSettings")]
    internal class InventoryItemSettings : ScriptableObject
    {
        [field: SerializeField] public float RectScale { get; private set; }
        [field: SerializeField] public Sprite UpgradeArrowSprite { get; private set; }
        [field: SerializeField] public float UpgradeArrowScale { get; private set; } = 30f;
    }
}
