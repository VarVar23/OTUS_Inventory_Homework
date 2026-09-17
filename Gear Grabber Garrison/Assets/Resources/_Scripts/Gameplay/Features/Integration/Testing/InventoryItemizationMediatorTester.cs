using UnityEngine;
using Zenject;
using Gameplay.Itemization;
using Gameplay.Itemization.Services;


namespace Gameplay.Integration
{
    /// <summary>
    /// The scene-level entry point that initializes the Itemization Engine 
    /// and connects it to the Inventory via the Mediator.
    /// </summary>
    public sealed class ItemizationIntegrationBootstrapper : MonoBehaviour
    {     
        [Inject] private ItemizationSystem _itemizationSystem;
        [Inject] private JSONItemizationRegistry _itemizationRegistry;

        void Update()
        {
            // (R) Roll from Loot Table
            if (Input.GetKeyDown(KeyCode.R))
            {
                _itemizationRegistry.Initialize();
                _itemizationSystem.RollFromLootTable(LootTable.TestDropTable);
                Debug.Log($"<color=cyan>[TestBench]</color> Rolled {LootTable.TestDropTable}.");
            }
        }
    }
}