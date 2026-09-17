using System;

namespace Gameplay.Itemization
{
    public interface IResolutionPayload
    {
        Guid ItemOwnerId { get; }
        Guid TargetId { get; }
        ResolutionPayloadKind Kind { get; }
    }
}