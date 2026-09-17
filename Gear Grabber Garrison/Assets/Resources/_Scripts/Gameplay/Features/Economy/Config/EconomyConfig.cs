using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Economy
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "Config/Economy Config")]
    public class EconomyConfig : ScriptableObject
    {
        [field: SerializeField] public long GenerateItemPrice { get; private set; }
        [field: SerializeField] public List<UnitPayloadData> UnitsData { get; private set; }


        [Button("CreateGUIDS")]
        private void CreateGUIDS()
        {
            for(int i = 0; i < UnitsData.Count; i++)
            {
                UnitsData[i].CreateGUID();
            }
        }
    }
}
