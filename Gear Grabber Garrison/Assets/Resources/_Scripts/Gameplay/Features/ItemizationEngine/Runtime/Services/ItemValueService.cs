using Gameplay.Itemization.Models;
using Zenject;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemValueService
    {
        private readonly JSONItemizationRegistry _registry;

        [Inject]
        public ItemValueService(JSONItemizationRegistry registry)
        {
            _registry = registry;
        }

        public void UpdateItemValue(Item item)
        {
            int currentValue = 0;
            int nextUpgradeTotal = 0;

            TypeDefinition typeConfig = _registry.GetTypeConfig(item.Type);
            currentValue += typeConfig?.BaseValue ?? 0;

            AttributeDataModel data;
            for (int i = 0; i < item.Attributes.Count; i++)
            {
                data = _registry.GetAttribute(item.Attributes[i].Id);
                if (data == null) continue;

                for (int j = 0; j < data.Levels.Count; j++)
                {
                    if (data.Levels[j].Level <= item.Attributes[i].Level)
                    {
                        currentValue += data.Levels[j].Cost;
                    }
                    
                    if (data.Levels[j].Level == item.Attributes[i].Level + 1)
                    {
                        nextUpgradeTotal += data.Levels[j].Cost;
                    }
                }
            }
            
            item.UpdateValueData(new ItemValueData
            {
                CurrentValue = currentValue,
                UpgradeValue = nextUpgradeTotal
            });
        }
    }
}