using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher 
{
    public async UniTask Switch(string sceneName)
    {
        if(KnownValues.Scene.IsKnown(sceneName))
        {
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
        }
        else
        {
            Debug.LogError("Scene not switch");
        }
    }
}