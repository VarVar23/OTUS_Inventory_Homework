using System;
using System.Collections.Generic;
using Gameplay.Itemization.InternalContracts;

namespace Gameplay.Itemization.Models
{
    internal sealed class StatChangeRule : IRule, IUseValues
    {
        private readonly Stat _stat;
        private readonly ModificationType _modType;
        private List<string> _keys = new();

        public StatChangeRule(Stat stat, ModificationType modType)
        {
            _stat = stat;
            _modType = modType;
        }

        public void StoreValueKeys(List<string> keys) => _keys = keys;

        public Context Process(Context context, List<ResolutionContext> output)
        {
            if (_keys == null || _keys.Count == 0)
            {
                throw new InvalidOperationException(
                    $"[RuleChain Error] StatChangeRule for '{_stat}' has no ValueKeys! " +
                    "Did you forget to call .WithValues() in the AttributeFactory?");
            }

            Guid ownerId = (context as TriggerContext)?.ItemOwnerId ?? Guid.Empty;
            Guid targetId = (context as TriggerContext)?.TargetId ?? Guid.Empty;
            for (int i = 0; i < _keys.Count; i++)
            {
                float amount = context.Get(_keys[i]);
                output.Add(new StatResolutionContext
                {
                    ItemOwnerId = ownerId,
                    TargetId = targetId,
                    Stat = _stat,
                    ModificationType = _modType,
                    Value = amount
                });
            }

            return context;
        }
    }
}