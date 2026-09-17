using Cysharp.Threading.Tasks;
using System.Collections.Generic;

internal class BootstrapStateMachine
{
    private List<IBootstrapState> _states;

    public BootstrapStateMachine(List<IBootstrapState> states)
    {
        _states = states;
    }

    public async UniTask Run()
    {
        foreach (var state in _states)
        {
            await state.Enter();
        }
    }
}