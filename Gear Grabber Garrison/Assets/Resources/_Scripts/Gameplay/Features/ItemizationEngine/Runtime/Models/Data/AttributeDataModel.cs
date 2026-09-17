using System;
using System.Collections.Generic;
using Gameplay.Itemization.InternalContracts;

namespace Gameplay.Itemization.Models
{
    [Serializable]
    internal sealed class AttributeDataModel
    {
        public string AttributeId;
        public List<string> ValueKeys = new();
        public List<AttributeLevelData> Levels = new();
        public List<RuleChainModel> RuleChains = new();
        public AttributeNamingData Naming; 
    }

    [Serializable]
    internal sealed class AttributeLevelData
    {
        public int Level;
        public int Cost;
        public List<AttributeValueEntry> Values = new();
    }

    [Serializable]
    internal sealed class RuleChainModel
    {
        public AttributeTrigger Trigger;
        public List<RuleStepModel> Steps = new();
    }

    [Serializable]
    internal sealed class RuleStepModel
    {
        public StepType Type;
        public string ValueKey;
        public Stat TargetStat;
        public ModificationType Mod;
        public AttributeEffect Effect;
        public string DescriptionTemplate;
    }

    [Serializable]
    internal sealed class AttributeValueEntry
    {
        public string Key;
        public int Value;
    }

    [Serializable]
    internal sealed class AttributeNamingData
    {
        public List<string> Prefixes = new();
        public List<string> Postfixes = new();
    }
}