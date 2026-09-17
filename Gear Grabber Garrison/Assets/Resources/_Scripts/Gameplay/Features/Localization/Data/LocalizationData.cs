using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Localization
{
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "Config/LocalizationData")]
    internal class LocalizationData : ScriptableObject
    {
        [field: SerializeField] public List<Language> Languages { get; private set; }
    }
}