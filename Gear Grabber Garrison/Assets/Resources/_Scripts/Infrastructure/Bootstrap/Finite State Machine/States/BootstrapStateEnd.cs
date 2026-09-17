using Cysharp.Threading.Tasks;

internal class BootstrapStateEnd : IBootstrapState
{
    private CurtainShowerView _curtainShowerView;

    public BootstrapStateEnd(CurtainShowerView curtainShowerView)
    {
        _curtainShowerView = curtainShowerView;
    }

    public async UniTask Enter()
    {
        await _curtainShowerView.Hide(1);
    }
}
