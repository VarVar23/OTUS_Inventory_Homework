using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

internal class BootstrapStateFirebase : IBootstrapState
{
    public async UniTask Enter()
    {
        await Task.Delay(1); // Имитация загрузки чего-то
        Debug.Log("FireBase подключен");
    }
}
