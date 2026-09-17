namespace Gameplay.Itemization
{
    public interface IEffectResolutionPayload : IResolutionPayload
    {
        AttributeEffect Effect { get; }
        float? Value { get; }
    }
}