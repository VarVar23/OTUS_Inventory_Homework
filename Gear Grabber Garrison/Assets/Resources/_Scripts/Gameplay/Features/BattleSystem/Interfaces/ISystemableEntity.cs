using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public interface ISystemableEntity
    {
        public Team UnitTeam { get; set; }
        public SystemableComponents Components { get; set; }

        // public SystemAttributes _attributes { get; set; }
        public ReadOnlyAttributes Attributes { get; }
        public Vector3 Position { get; set; }

        public void LookAt(Vector3 target);
    }
}
