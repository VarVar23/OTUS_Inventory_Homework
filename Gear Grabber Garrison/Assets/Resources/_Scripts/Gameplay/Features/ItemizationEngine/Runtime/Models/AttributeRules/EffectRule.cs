using System;
using System.Collections.Generic;
using Gameplay.Itemization.InternalContracts;
using UnityEngine;

namespace Gameplay.Itemization.Models
{
    internal sealed class EffectRule : IRule, IUseValues
    {
        private readonly AttributeEffect _effect;
        private List<string> _keys = new();

        public EffectRule(AttributeEffect effect) => _effect = effect;

        public void StoreValueKeys(List<string> keys) => _keys = keys;

        public Context Process(Context context, List<ResolutionContext> output)
        {
            Guid ownerId = (context as TriggerContext)?.ItemOwnerId ?? Guid.Empty;
            Guid targetId = (context as TriggerContext)?.TargetId ?? Guid.Empty;
            var result = new EffectResolutionContext
            {
                ItemOwnerId = ownerId,
                TargetId = targetId,
                Effect = _effect,
                Value = null
            };

            if (_keys == null || _keys.Count == 0)
            {
                Debug.LogWarning($"[EffectRule] No keys found for {_effect}. Treating as parameterless effect.");
                output.Add(result);
                return context;
            }

            result.Value = context.Get(_keys[0]);
            output.Add(result);
            return context;
        }
    }
}