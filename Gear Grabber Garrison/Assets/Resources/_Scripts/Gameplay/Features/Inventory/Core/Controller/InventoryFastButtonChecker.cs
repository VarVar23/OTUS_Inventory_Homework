using Gameplay.Input;
using UnityEngine;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryFastButtonChecker : ITickable
    {
        public bool FastMode { get; private set; }

        private readonly IInputStateReader _inputStateReader;

        private InventoryFastButtonChecker(IInputStateReader inputStateReader)
        {
            _inputStateReader = inputStateReader;
        }

        public void Tick()
        {
            //if(_inputStateReader.IsFastModeModifierPressed && !_inputStateReader.IsPrimaryPointerPressed)
            //{
            //    FastMode = true;
            //}

            //if(!_inputStateReader.IsFastModeModifierPressed)
            //{
            //    FastMode = false;
            //}

            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && !UnityEngine.Input.GetMouseButton(0))
            {
                FastMode = true;
            }

            if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift))
            {
                FastMode = false;
            }
        }
    }
}