using System.Collections.Generic;
using Gameplay.Itemization.Models;
using Zenject;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemBuilderService
    {
        [Inject] private readonly JSONItemizationRegistry _registry;
        [Inject] private readonly ItemValueService _valueService;

        public Item BuildFromInstructions(BuildInstructions instructions)
        {
            List<ItemAttribute> liveAttributes = new();

            List<KeyValuePair<string, int>> attributeEntries = new(instructions.AttributeMap);

            string attributeId;
            int level;
            AttributeDataModel attributeData;

            for (int i = 0; i < attributeEntries.Count; i++)
            {
                attributeId = attributeEntries[i].Key;
                level = attributeEntries[i].Value;

                attributeData = _registry.GetAttribute(attributeId);
                if (attributeData == null) continue;

                liveAttributes.Add(AttributeFactory.Build(attributeData, level));
            }

            Item freshItem = new(
                instructions.GeneratedName, 
                instructions.SelectedType,
                instructions.Icon, 
                liveAttributes
            );

            _valueService.UpdateItemValue(freshItem);
            DescriptionGenerator.FillItemDescription(freshItem);

            return freshItem;
        }
    }
}