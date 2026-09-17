using Gameplay.Itemization.InternalContracts;

namespace Gameplay.Itemization.Models
{
    internal sealed class StatResolutionContext : ResolutionContext
    {
        public Stat Stat;
        public ModificationType ModificationType;
        public float Value;
    }
}