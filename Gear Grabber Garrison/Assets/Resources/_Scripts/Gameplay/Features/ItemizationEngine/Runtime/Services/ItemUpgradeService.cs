using Gameplay.Itemization.Models;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemUpgradeService
    {
        private readonly ItemValueService _valueService;

        public ItemUpgradeService(ItemValueService valueService)
        {
            _valueService = valueService;
        }
        
        public void UpgradeItem(Item item)
        {
            ItemAttribute attributeToUpgrade;
            // When an attribute updates and triggers Item.HandleAttributeUpdate, it will be removed from this list. 
            // By going backwards, we avoid the "Skipping" bug caused by index shifting.
            for (int i = item.UpgradeableAttributes.Count - 1; i >= 0; i--)
            {
                attributeToUpgrade = item.UpgradeableAttributes[i];
                attributeToUpgrade.SetLevel(attributeToUpgrade.Level + 1);
                DescriptionGenerator.FillAttributeDescription(attributeToUpgrade);
            }

            _valueService.UpdateItemValue(item);
            DescriptionGenerator.FillItemDescription(item);
        }
    }
}