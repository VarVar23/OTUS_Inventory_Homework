namespace Gameplay.Itemization
{
    public interface IStatResolutionPayload : IResolutionPayload
    {
        Stat Stat { get; }
        ModificationType ModificationType { get; }
        float Value { get; }
    }
}