using UnityEngine;

namespace Gameplay.Input
{
    public interface IInputStateReader
    {
        Vector2 PointerPosition { get; }
        bool IsPrimaryPointerPressed { get; }
        bool IsFastModeModifierPressed { get; }
    }
}