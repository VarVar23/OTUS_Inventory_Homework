using System;
using System.Collections.Generic;
using Gameplay.Itemization.InternalContracts;

namespace Gameplay.Itemization.Models
{
    internal sealed class RuleChain
    {
        public AttributeTrigger Trigger { get; private set; }
        private readonly List<IRule> _pipeline = new();
        private readonly TriggerContext _internalContext = new();

        public void UpdateLockedData(Dictionary<string, float> data)
            => _internalContext.LoadLockedData(data);

        public RuleChain(AttributeTrigger trigger) => Trigger = trigger;

        public RuleChain Where(Func<Context, bool> predicate)
        {
            _pipeline.Add(new FilterRule(predicate));
            return this;
        }

        public RuleChain Select(Action<Context> transformation)
        {
            _pipeline.Add(new TransformRule(transformation));
            return this;
        }

        public RuleChain Do(AttributeEffect effect)
        {
            _pipeline.Add(new EffectRule(effect));
            return this;
        }

        public RuleChain ChangeStat(Stat stat, ModificationType modType)
        {
            _pipeline.Add(new StatChangeRule(stat, modType));
            return this;
        }

        public RuleChain WithValues(params string[] keys)
        {
            if (_pipeline.Count > 0 && _pipeline[^1] is IUseValues valueAwareRule)
                valueAwareRule.StoreValueKeys(new List<string>(keys));
            return this;
        }

        public RuleChain AddCustomRule(IRule customRule)
        {
            _pipeline.Add(customRule);
            return this;
        }

        public List<ResolutionContext> Execute(TriggerContext triggerData)
        {
            List<ResolutionContext> output = new();

            _internalContext.ImportVolatile(triggerData);

            Context current = _internalContext;
            for (int i = 0; i < _pipeline.Count; i++)
            {
                current = _pipeline[i].Process(current, output);
                if (current == null) break;
            }

            _internalContext.ResetVolatile();
            return output;
        }
    }
}