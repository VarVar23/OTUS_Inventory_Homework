using UnityEngine;

namespace Gameplay.Features.Battlesystem
{
    public class EntitiTarget : MonoBehaviour, ISystemableEntity
    {
        public Team UnitTeam { get; set; } 
        public SystemableComponents Components { get; set; }
        public Vector3 Position { get => transform.position; set => transform.position = value; }
        public ReadOnlyAttributes Attributes => null;

        public void LookAt(Vector3 target)
        {
            
        } 
    }
}
