using System;

namespace Gameplay.Features.Battlesystem
{
    namespace Gameplay.Stats
    {
        public interface IHasID
        {
            Guid GuId { get; }
        }
    }
}