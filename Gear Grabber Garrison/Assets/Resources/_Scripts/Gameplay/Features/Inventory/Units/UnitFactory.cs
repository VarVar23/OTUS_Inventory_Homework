using UnityEngine;

namespace Gameplay.Inventory
{
    internal class UnitFactory
    {
        private readonly UnitsView _unitsView;
        private readonly UnitView _prefab;

        public UnitFactory(UnitsView unitsView, UnitView prefab)
        {
            _unitsView = unitsView;
            _prefab = prefab;
        }

        public UnitView Create(UnitPayloadData payload)
        {
            var unitView = Object.Instantiate(_prefab, _unitsView.Content);
            unitView.Init(payload);
            return unitView;
        }
    }
}
