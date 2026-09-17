using Cysharp.Threading.Tasks;
using DG.Tweening;

internal class BootstrapStateStart : IBootstrapState
{
    private CurtainShowerView _curtainShowerView;

    public BootstrapStateStart(CurtainShowerView curtainShowerView)
    {
        _curtainShowerView = curtainShowerView;
    }

    public async UniTask Enter()
    {
        DOTween.Init();

        await _curtainShowerView.Show(1); //Config
    }
}