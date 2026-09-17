using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Input
{
    internal sealed class InputSystemStateReader : IInputStateReader, IDisposable, ITickable
    {
        private readonly InputSystem_GGG _actions;

        public InputSystemStateReader()
        {
            _actions = new InputSystem_GGG();
            _actions.UI.Enable();
        }

        public Vector2 PointerPosition => _actions.UI.PointerPosition.ReadValue<Vector2>();
        public bool IsPrimaryPointerPressed => _actions.UI.PointerButton.IsPressed();
        public bool IsFastModeModifierPressed { get; private set; }

        private bool _wasModifierPressed;

        public void Tick()
        {
            ModifierFastPressedCheck();
        }

        private void ModifierFastPressedCheck()
        {
            bool current = _actions.UI.FastModeModifier.IsPressed();
            IsFastModeModifierPressed = current && !_wasModifierPressed;
            _wasModifierPressed = current;
        }

        public void Dispose()
        {
            _actions.UI.Disable();
            _actions.Dispose();
        }
    }
}