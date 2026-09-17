using Zenject;
using Gameplay.Features.Battlesystem;
using Gameplay.Itemization;
using System;
using System.Collections.Generic;
using Gameplay.Itemization.Models;


namespace Gameplay.Integration
{
    public class BattleItemizationMediator  : IInitializable, IDisposable
    {
        [Inject] private IItemizationSystem _itemization;
        [Inject] private IBattleFacade _battle;

        public void Initialize()
        {
            _itemization.OnInventoryResolved += HandleInventoryResolved;
            _battle.OnEnemyDeath += HandleEnemyDeath;
        }

        public void Dispose()
        {
            _itemization.OnInventoryResolved -= HandleInventoryResolved;
            _battle.OnEnemyDeath -= HandleEnemyDeath;
        }

        private void HandleInventoryResolved(IReadOnlyList<IResolutionPayload> payloads)
        {
            foreach (IResolutionPayload payload in payloads)
            {
                HandleStatResoltion(payload);
            }
        }

        private void HandleEnemyDeath(Guid guid) => _itemization.RollFromLootTable(LootTable.TestDropTable);

        private void HandleTriggerResolveRequest(Guid callerId) //TriggerType triggerType, Guid targetId
        {
            TriggerContext context = new()
            {
                ItemOwnerId = callerId,
                // Trigger = triggerType,
                // TargetId = targetId
            };

            IReadOnlyList<IResolutionPayload> resolutionPayloads = _itemization.ActivateTrigger(context);

            foreach (IResolutionPayload payload in resolutionPayloads)
            {
                HandleEffectResoltion(payload);
                HandleStatResoltion(payload);
            }
        }

        private void HandleEffectResoltion(IResolutionPayload resolutionPayload)
        {
            if (resolutionPayload is IEffectResolutionPayload effectPayload)
            {
                // ResolveEffect
            }
        }

        private void HandleStatResoltion(IResolutionPayload resolutionPayload)
        {
            if (resolutionPayload is IStatResolutionPayload statPayload)
            {
                StatType statType = TypeMapping(statPayload.Stat);
                Func<float, float> valueChangeAction = ValueChangeMapping(statPayload.ModificationType, statPayload.Value);
                _battle.UpgradePlayerArmy(statPayload.TargetId, statType, valueChangeAction);
            }
        }

        private StatType TypeMapping(Stat stat)
        {
            switch (stat)
            {
                case Stat.MaxHealth:
                    return StatType.HealthMax;
                default:
                    return StatType.HealthMax;
            }
        }

        private Func<float, float> ValueChangeMapping(Itemization.ModificationType modificationType, float value)
        {
            return modificationType switch
            {
                Itemization.ModificationType.Add     => v => v + value,
                Itemization.ModificationType.Multiply=> v => v * value,
                Itemization.ModificationType.Set     => v => value,
                _                        => v => v // по умолчанию оставляем unchanged
            };
        }
    }
}