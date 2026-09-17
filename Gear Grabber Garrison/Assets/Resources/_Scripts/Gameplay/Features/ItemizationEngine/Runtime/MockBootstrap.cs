using Gameplay.Itemization.Services;
using UnityEngine;
using Zenject;

namespace Gameplay.Itemization
{
    public class MockBootstrap : MonoBehaviour
    {
        public static MockBootstrap Instance { get; private set; }

        public ItemizationSystem ItemSystem { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        [Inject]  
        public void Construct(ItemizationSystem system, JSONItemizationRegistry registry)
        {
            registry.Initialize();
            ItemSystem = system;

            Debug.Log("<color=green>[Bootstrap]</color> Itemization System Injected.");
        }

    }
    }