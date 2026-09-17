using Cysharp.Threading.Tasks;
using UnityEngine;

internal class BootstrapStateSceneSwitcher : IBootstrapState
{
    private SceneSwitcher _sceneSwitcher;

    public BootstrapStateSceneSwitcher(SceneSwitcher sceneSwitcher)
    {
        _sceneSwitcher = sceneSwitcher;
    }

    public async UniTask Enter()
    {
        await _sceneSwitcher.Switch(KnownValues.Scene.GAME);

        Debug.Log("Сцена переключена");
    }
}
