using System;

namespace Gameplay.Itemization.Models
{
    internal sealed class StatResolutionPayload : IStatResolutionPayload
    {
        public Guid ItemOwnerId { get; }
        public Guid TargetId { get; }
        public ResolutionPayloadKind Kind => ResolutionPayloadKind.Stat;
        public Stat Stat { get; }
        public ModificationType ModificationType { get; }
        public float Value { get; }

        public StatResolutionPayload(Guid itemOwnerId, Guid targetId, Stat stat, ModificationType modificationType, float value)
        {
            ItemOwnerId = itemOwnerId;
            TargetId = targetId;
            Stat = stat;
            ModificationType = modificationType;
            Value = value;
        }
    }
}