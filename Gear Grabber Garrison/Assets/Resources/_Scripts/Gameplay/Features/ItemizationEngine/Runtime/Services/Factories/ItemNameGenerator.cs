using System.Collections.Generic;
using Gameplay.Itemization.Models;
using Zenject;

namespace Gameplay.Itemization.Services
{
    internal sealed class ItemNameGenerator
    {
        private readonly JSONItemizationRegistry _registry;

        [Inject]
        public ItemNameGenerator(JSONItemizationRegistry registry)
        {
            _registry = registry;
        }

        public string GenerateName(ItemType type, List<string> rolledAttributeIds, out string baseNameUsed)
        {
            TypeDefinition typeConfig = _registry.GetTypeConfig(type);
            if (typeConfig == null || typeConfig.Names.Count == 0)
            {
                baseNameUsed = $"Unknown {type}";
                return baseNameUsed;
            }

            string baseName = typeConfig.Names[UnityEngine.Random.Range(0, typeConfig.Names.Count)];
            baseNameUsed = baseName;

            List<AttributeDataModel> namePool = GetNamePool(rolledAttributeIds);

            if (namePool.Count == 0) return baseName;

            string prefix = "";
            string postfix = "";

            AttributeDataModel prefixAttr = namePool[UnityEngine.Random.Range(0, namePool.Count)];
            if (prefixAttr.Naming.Prefixes.Count > 0)
            {
                prefix = prefixAttr.Naming.Prefixes[UnityEngine.Random.Range(0, prefixAttr.Naming.Prefixes.Count)];
                namePool.Remove(prefixAttr);
            }

            if (namePool.Count > 0)
            {
                AttributeDataModel postfixAttr = namePool[UnityEngine.Random.Range(0, namePool.Count)];
                if (postfixAttr.Naming.Postfixes.Count > 0)
                {
                    postfix = postfixAttr.Naming.Postfixes[UnityEngine.Random.Range(0, postfixAttr.Naming.Postfixes.Count)];
                }
            }

            string finalName = baseName;
            if (!string.IsNullOrEmpty(prefix)) finalName = $"{prefix} {finalName}";
            if (!string.IsNullOrEmpty(postfix)) finalName = $"{finalName} {postfix}";

            return finalName;
        }

        private List<AttributeDataModel> GetNamePool(List<string> rolledAttributeIds)
        {
            List<AttributeDataModel> namePool = new();
            for (int i = 0; i < rolledAttributeIds.Count; i++)
            {
                AttributeDataModel attributeData = _registry.GetAttribute(rolledAttributeIds[i]);
                if (attributeData != null && attributeData.Naming != null)
                    namePool.Add(attributeData);
            }

            return namePool;
        }
    }
}