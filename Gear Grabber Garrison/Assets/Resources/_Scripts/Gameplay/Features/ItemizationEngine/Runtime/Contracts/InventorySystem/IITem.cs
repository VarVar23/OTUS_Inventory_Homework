using UnityEngine;

namespace Gameplay.Itemization
{
    public interface IItem : IHasID
    {
        public string ItemName { get; }
        public ItemType Type { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        public string UpgradeDescription { get; }
        public bool CanUpgrade { get; }
        public ItemValueData ValueData { get; }

        public void Dispose();
    }
}