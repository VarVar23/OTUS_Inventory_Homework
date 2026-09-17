using System;
using UnityEngine;

namespace Gameplay.Economy
{
    [Serializable]
    public struct UnitData
    {
        public Guid UnitID;
        public Sprite Ava;
        public long Price;
    }
}