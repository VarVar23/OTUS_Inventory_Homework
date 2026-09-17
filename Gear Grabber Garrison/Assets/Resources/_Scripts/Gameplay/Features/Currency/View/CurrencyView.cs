using TMPro;
using UnityEngine;

namespace Gameplay.Currency
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _balanceText;       

        public void UpdateBalance(long balance)
        {
            _balanceText.text = balance.ToString();
        }
    }
}
