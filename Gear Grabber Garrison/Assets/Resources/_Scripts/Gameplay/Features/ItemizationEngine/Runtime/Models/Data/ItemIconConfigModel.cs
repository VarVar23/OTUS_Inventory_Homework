using System;
using System.Collections.Generic;

namespace Gameplay.Itemization.Models
{
    [Serializable]
    internal sealed class ItemIconConfigModel
    {
        public List<IconMapping> Mappings = new();
    }

    [Serializable]
    internal sealed class IconMapping
    {
        public string BaseName;
        public string SpritePath;
    }
}