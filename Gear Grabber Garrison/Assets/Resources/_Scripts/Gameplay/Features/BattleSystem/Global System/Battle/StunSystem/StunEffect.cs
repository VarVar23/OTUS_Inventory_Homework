
namespace Gameplay.Features.Battlesystem
{
    public static class StunEffect
    {
        public static bool Apply(ISystemableEntity target, float baseDuration)
        {
            var existing = target.Components.TryGetSystem(SystemType.StunSystem) as StunSystem;
            if (existing != null)
            {
                existing.Restart(baseDuration);
                return true;
            }

            var stun = StunSystem.Create(target.Attributes, target, baseDuration);
            if (stun == null)
                return false;

            target.Components.AttachSystem(stun);
            return true;
        }
    }
}