using System;
using System.Collections.Generic;

namespace Gameplay.Itemization.Models
{
    [Serializable]
    internal sealed class ItemTypeConfigModel
    {
        public List<TypeDefinition> Definitions = new();
    }

    [Serializable]
    internal sealed class TypeDefinition
    {
        public ItemType Type;
        public int BaseValue;
        public List<string> Names = new();
    }
}