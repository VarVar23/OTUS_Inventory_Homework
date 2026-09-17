using UnityEngine;

namespace Gameplay.Currency
{
    [CreateAssetMenu(fileName = "CurrencyData", menuName = "Config/CurrencyData")]
    internal class CurrencyData : ScriptableObject
    {
        [field: SerializeField] public Color Success { get; private set; }
        [field: SerializeField] public Color Failure { get; private set; }
    }
} 