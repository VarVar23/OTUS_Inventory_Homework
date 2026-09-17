using System;

namespace Gameplay.Itemization.Models
{
    public sealed class TriggerContext : Context
    {
        public Guid ItemOwnerId;
        public Guid TargetId;
        public AttributeTrigger Trigger;

        public void ImportVolatile(TriggerContext other)
        {
            ImportVolatileData(other);
            ItemOwnerId = other.ItemOwnerId;
            TargetId = other.TargetId;
            Trigger = other.Trigger;
        }
    }
}