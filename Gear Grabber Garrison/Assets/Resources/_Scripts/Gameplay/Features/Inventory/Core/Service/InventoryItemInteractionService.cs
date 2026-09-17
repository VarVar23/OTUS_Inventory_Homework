using Gameplay.Input;
using Zenject;

namespace Gameplay.Inventory
{
    internal class InventoryItemInteractionService
    {
        [Inject] private InventoryFastButtonChecker _fastButtonChecker;
        [Inject] private InventoryFastMode _fastMode;
        [Inject] private InventoryMoveService _moveService;
        [Inject] private InventoryCellService _cellService;
        [Inject] private InventoryCellFinder _cellFinder;
        [Inject] private InventoryItemParentController _parentController;
        [Inject] private InventoryRepository _repository;
        [Inject] private InventoryMultiCellService _multiCellService;
        [Inject] private InventoryTooltipController _tooltipController;
        [Inject] private InventoryScrollLockController _scrollLockController;
        [Inject] private InventoryScrollToTargetController _scrollToTargetController;
        [Inject] private IInputStateReader _input;

        public void Initialize()
        {
            _cellService.MoveToCell += _moveService.OnMoveToCell;
        }

        public void InitializeItem(InventoryItemView item)
        {
            item.OnDown += OnDown;
            item.OnUp += OnUp;
            item.OnEnter += OnEnter;
            item.OnExit += OnExit;
        }

        public void UnSubscribeItem(InventoryItemView item)
        {
            item.OnDown -= OnDown;
            item.OnUp -= OnUp;
            item.OnEnter -= OnEnter;
            item.OnExit -= OnExit;
        }

        private void OnDown(InventoryItemView item)
        {
            _scrollLockController.LockMove(true);

            var slot = _repository.GetSlotByItem(item);
            _parentController.SetPriorityParent(item);

            if (_fastButtonChecker.FastMode)
            {
                var target = _fastMode.GetTargetCell(item);

                if (target != null)
                {
                    _scrollToTargetController.ScrollToTarget(target.transform, () =>
                    {
                        _moveService.MoveAnimation(item.transform, target.transform, () => _parentController.SetParent(item.transform, target.transform));
                        _cellService.PutItem(target, item);
                    });
                }
            }
            else
            {
                _moveService.AddCursorMovable(item.transform);
            }

            _tooltipController.HideItem();

            if (slot.CellView.CellType == InventoryCellType.Bag && !_fastButtonChecker.FastMode)
            {
                _multiCellService.SetVisibleDragSelected(true);
            }
        }

        private void OnUp(InventoryItemView item)
        {
            _moveService.RemoveCursorMovable();

            if (!_fastButtonChecker.FastMode)
            {
                _cellService.PutItem(_cellFinder.FindNearestCell(item), item);
            }

            _multiCellService.SetVisibleDragSelected(false);
            _scrollLockController.LockMove(false);
        }

        private void OnEnter(InventoryItemView view)
        {
            if(!_input.IsPrimaryPointerPressed)
            {
                _tooltipController.SetItemInfo(view);
            }
        }

        private void OnExit(InventoryItemView view)
        {
            _tooltipController.HideItem();
        }
    }
}