using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitAvaView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event Action OnEnter;
    public event Action OnExit;
    [field: SerializeField] public Image AvaImage { get; private set; }
    [SerializeField] private Image _fillBarImage;

    public void OnPointerEnter(PointerEventData eventData) => OnEnter?.Invoke();
    public void OnPointerExit(PointerEventData eventData) => OnExit?.Invoke();
    public void ChangeReloadProcent(float value)
    {
        _fillBarImage.fillAmount = value;

        bool ready = value <= 0;
        _fillBarImage.gameObject.SetActive(!ready);
    }
}