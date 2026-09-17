namespace Gameplay.Features.Battlesystem
{
    public class FireStrategy : I_Strategy
    {
        private I_System _owner;
        private ReadOnlyAttributes _attributes;

        public FireStrategy() { }

        public FireStrategy(ReadOnlyAttributes attributes, I_System owner)
        {
            _attributes = attributes;
            _owner = owner;
        }

        public void SetOwner(I_System owner)
        {
            _owner = owner;
        }

        public void SetAttributes(ReadOnlyAttributes attributes)
        {
            _attributes = attributes;
        }

        public void Tick(float dt)
        {
        }
    }
}