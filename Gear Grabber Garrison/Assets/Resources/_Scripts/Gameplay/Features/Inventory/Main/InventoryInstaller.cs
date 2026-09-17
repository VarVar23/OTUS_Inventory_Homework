using UnityEngine;
using Zenject;

namespace Gameplay.Inventory
{
    public class InventoryInstaller : MonoInstaller
    {
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private InventoryMultiCellView _multiCellView;
        [SerializeField] private InventorySettings _inventorySettings;
        [SerializeField] private InventoryItemSettings _inventoryItemSettings;
        [SerializeField] private InventoryAddItemsView _inventoryAddItemsView;
        [SerializeField] private UnitsView _unitsView;
        [SerializeField] private UnitView _unitViewPrefab;
        [SerializeField] private InventoryTooltipInfoView _tooltipInfoView;
        [SerializeField] private InventoryTooltipUnitView _tooltipUnitView;

        public override void InstallBindings()
        {
            Container.Bind<InventoryView>().FromInstance(_inventoryView).AsSingle();
            Container.Bind<InventoryMultiCellView>().FromInstance(_multiCellView).AsSingle();
            Container.Bind<InventorySettings>().FromInstance(_inventorySettings).AsSingle();
            Container.Bind<InventoryItemSettings>().FromInstance(_inventoryItemSettings).AsSingle();
            Container.Bind<InventoryAddItemsView>().FromInstance(_inventoryAddItemsView).AsSingle();

            Container.Bind<InventoryTooltipInfoView>().FromInstance(_tooltipInfoView).AsSingle();
            Container.Bind<InventoryTooltipUnitView>().FromInstance(_tooltipUnitView).AsSingle();

            Container.Bind<UnitsView>().FromInstance(_unitsView).AsSingle();
            Container.Bind<UnitView>().FromInstance(_unitViewPrefab).AsSingle();

            Container.BindInterfacesAndSelfTo<InventoryMoveItemController>().AsSingle();
            Container.BindInterfacesAndSelfTo<InventoryService>().AsSingle();
            Container.BindInterfacesAndSelfTo<InventoryTooltipService>().AsSingle();
            Container.BindInterfacesAndSelfTo<Inventory>().AsSingle();
            Container.BindInterfacesAndSelfTo<InventoryFastButtonChecker>().AsSingle();
            Container.BindInterfacesAndSelfTo<InventoryUnitService>().AsSingle();

            Container.Bind<InventoryCellFinder>().AsSingle();
            Container.Bind<InventoryCellService>().AsSingle();
            Container.Bind<InventoryItemParentController>().AsSingle();
            Container.Bind<InventoryFastMode>().AsSingle();
            Container.Bind<InventoryMoveService>().AsSingle();
            Container.Bind<InventoryBuilder>().AsSingle();
            Container.Bind<InventoryDestroyer>().AsSingle();
            Container.Bind<InventoryTooltipController>().AsSingle();
            Container.Bind<InventoryScrollLockController>().AsSingle();
            Container.Bind<InventoryScrollToTargetController>().AsSingle();

            Container.Bind<InventoryRepository>().AsSingle();
            Container.Bind<InventoryMultiCellRepository>().AsSingle();
            Container.Bind<InventoryUnitsRepository>().AsSingle();

            Container.Bind<InventoryVisibilityService>().AsSingle();
            Container.Bind<InventoryItemInteractionService>().AsSingle();
            Container.Bind<InventoryLoadService>().AsSingle();
            Container.Bind<InventoryMultiCellService>().AsSingle();
            Container.Bind<InventoryPutItemService>().AsSingle();
            Container.Bind<UnitReloadController>().AsSingle();
            Container.Bind<InventorySaveLoadService>().AsSingle();
            Container.Bind<UnitFactory>().AsSingle();
        }
    }
}