using System;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryTooltipService : IInitializable
    {
        private InventoryTooltipController _controller;
        private InventoryUnitsRepository _repository;
        private InventoryUnitService _unitService;

        public InventoryTooltipService(InventoryTooltipController controller, InventoryUnitsRepository unitsRepository, InventoryUnitService unitService)
        {
            _controller = controller;
            _repository = unitsRepository;
            _unitService = unitService;
        }

        public void Initialize()
        {
            _unitService.OnAvaEnter += Enter;
            _unitService.OnAvaExit += Exit;
        }

        private void Exit(Guid unitId)
        {
            _controller.HideUnit();
        }

        private void Enter(Guid unitId)
        {
            var unitData = _repository.GetUnitData(unitId);
            var unitView = _repository.GetView(unitId);
            _controller.SetUnitInfo(unitView, unitData.Effects, unitData.Name);
        }
    }
}