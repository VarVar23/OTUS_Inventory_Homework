using System;

namespace Gameplay.Itemization.Models
{
    internal sealed class EffectResolutionPayload : IEffectResolutionPayload
    {
        public Guid ItemOwnerId { get; }
        public Guid TargetId { get; }
        public ResolutionPayloadKind Kind => ResolutionPayloadKind.Effect;
        public AttributeEffect Effect { get; }
        public float? Value { get; }

        public EffectResolutionPayload(Guid itemOwnerId, Guid targetId, AttributeEffect effect, float? value)
        {
            ItemOwnerId = itemOwnerId;
            TargetId = targetId;
            Effect = effect;
            Value = value;
        }
    }
}