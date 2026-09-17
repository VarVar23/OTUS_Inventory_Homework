using Cysharp.Threading.Tasks;

internal interface IBootstrapState
{
    public UniTask Enter();
}
