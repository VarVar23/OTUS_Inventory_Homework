using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Inventory
{
    internal class UnitsView : MonoBehaviour
    {
        [field: SerializeField] public Transform Content { get; private set; }
        [field: SerializeField] public ScrollRect ScrollRect { get; private set; }
    }
}