using System;

namespace Gameplay.Itemization.Models
{
    internal abstract class ResolutionContext
    {
        public Guid ItemOwnerId;
        public Guid TargetId;
    }
}