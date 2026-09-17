using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

internal class BootstrapStateAddresables : IBootstrapState
{
    private readonly DiContainer _container;

    public BootstrapStateAddresables(DiContainer container)
    {
        _container = container;
    }

    public async UniTask Enter()
    {
        var addressables = new string[]
        {
            KnownValues.Addresables.GAME
        };

        for(int i = 0; i < addressables.Length; i++)
        {
            var prefab = await Addressables
                .LoadAssetAsync<GameObject>(addressables[i])
                .ToUniTask();

            var instance = _container.InstantiatePrefab(prefab); 
            SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
        }    

        Debug.Log("Addressables: GameplayContext");
    }
}
