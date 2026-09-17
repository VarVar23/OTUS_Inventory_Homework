using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Itemization.Models
{
    internal sealed class BuildInstructions
    {
        public ItemType SelectedType;
        public string GeneratedName;
        public Sprite Icon;
        public Dictionary<string, int> AttributeMap = new();
    }
}