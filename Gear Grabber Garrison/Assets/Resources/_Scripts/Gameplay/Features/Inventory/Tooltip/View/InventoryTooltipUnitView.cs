using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay.Inventory
{
    internal class InventoryTooltipUnitView : InventoryTooltipInfoView
    {
        [SerializeField] private TMP_Text _name;

        public void SetInfo(string info, string name)
        {
            _name.text = name;
            SetInfo(info);
        }
    }
}