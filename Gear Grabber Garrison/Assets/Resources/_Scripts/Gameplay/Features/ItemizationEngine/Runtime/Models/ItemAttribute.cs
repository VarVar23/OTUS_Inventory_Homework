using System;
using System.Collections.Generic;

namespace Gameplay.Itemization.Models
{
    internal sealed class ItemAttribute
    {
        public string Id { get; private set; }
        public string Description { get; private set; }
        public string UpgradeDescription { get; private set; }
        public int Level { get; private set; }
        public bool CanUpgrade { get; private set; }
        public AttributeDataModel DataModel => _dataModel;

        public event Action<ItemAttribute> OnAttributeLevelUpdate;

        private readonly List<RuleChain> _chains = new();
        private AttributeDataModel _dataModel;

        public ItemAttribute(string id) => Id = id;

        public void Initialize(AttributeDataModel model, int level)
        {
            _dataModel = model;
            SetLevel(level);
        }

        public void AddChain(RuleChain chain) => _chains.Add(chain);

        public void UpdateDescriptions(string desc, string upgradeDesc) 
        { 
            Description = desc; 
            UpgradeDescription = upgradeDesc; 
        }

        public void SetLevel(int level)
        {
            Level = level;
            if (_dataModel == null) return;

            AttributeLevelData levelEntry = null;
            for (int i = 0; i < _dataModel.Levels.Count; i++)
            {
                if (_dataModel.Levels[i].Level == Level)
                {
                    levelEntry = _dataModel.Levels[i];
                    break;
                }
            }
            
            if (levelEntry == null)
            {
                throw new InvalidOperationException($"Missing level data for Attribute '{Id}' at level '{Level}'.");
            }

            Dictionary<string, float> blackboard = new(levelEntry.Values.Count);
            for (int i = 0; i < levelEntry.Values.Count; i++)
                blackboard[levelEntry.Values[i].Key] = levelEntry.Values[i].Value;

            for (int i = 0; i < _chains.Count; i++)
            {
                _chains[i].UpdateLockedData(blackboard);
            }

            CanUpgrade = false;
            for (int i = 0; i < _dataModel.Levels.Count; i++)
            {
                if (_dataModel.Levels[i].Level > Level)
                {
                    CanUpgrade = true;
                    break;
                }
            }

            OnAttributeLevelUpdate?.Invoke(this);
        }

        public List<ResolutionContext> ResolveTrigger(TriggerContext context)
        {
            List<ResolutionContext> output = new();
            for (int i = 0; i < _chains.Count; i++)
            {
                if (_chains[i].Trigger == context.Trigger)
                {
                    List<ResolutionContext> chainOutput = _chains[i].Execute(context);
                    if (chainOutput != null && chainOutput.Count > 0)
                        output.AddRange(chainOutput);
                }
            }
            return output;
        }
    }
}
    