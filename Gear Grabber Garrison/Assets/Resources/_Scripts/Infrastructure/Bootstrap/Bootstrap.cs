using UnityEngine;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    [Inject] private BootstrapStateMachine _bootstrapStateMachine;

    private void Awake()
    {
        _ = _bootstrapStateMachine.Run();
    }
}
