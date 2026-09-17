namespace Gameplay.Features.Battlesystem
{
   public interface I_System
   {
      public SystemType Type { get; set; }
      public ISystemableEntity Owner { get; set; }
      public SystemTargets Targets { get; set; }

      public void Register();
      public void Unregister();
      public void SetOwner(ISystemableEntity owner);
   }
}