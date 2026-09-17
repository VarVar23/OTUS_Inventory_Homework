using Gameplay.Inventory;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitView : MonoBehaviour
{
    public event Action<Guid> OnBuyButtonClick;
    public event Action<Guid> OnAvaEnter;
    public event Action<Guid> OnAvaExit;

    [field: SerializeField] internal InventoryCellView[] CellViews { get; private set; }
    [SerializeField] private UnitAvaView _avaView;
    [SerializeField] private Button _priceButton;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private GameObject _unitPanel;
    [SerializeField] private GameObject _unitLockPanel;
    private Guid _id;

    public void Init(UnitPayloadData payload)
    {
        _id = payload.ID;
        _priceText.text = payload.Price.ToString();
        _avaView.AvaImage.sprite = payload.Ava;

        _priceButton.onClick.AddListener(() => OnBuyButtonClick?.Invoke(_id));
        _avaView.OnEnter += () => OnAvaEnter?.Invoke(_id);
        _avaView.OnExit += () => OnAvaExit?.Invoke(_id);
    }

    public void Unlock(bool value)
    {
        _unitPanel.SetActive(value);
        _unitLockPanel.SetActive(!value);
    }

    public void SetUnlockPriceColor(Color color) => _priceText.color = color;
    public Transform GetAvaTransform() => _avaView.transform;
    public void ChangeReloadProcent(float value) => _avaView.ChangeReloadProcent(value);
}