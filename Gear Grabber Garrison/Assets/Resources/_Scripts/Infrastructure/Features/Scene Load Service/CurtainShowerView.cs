using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CurtainShowerView : SingletonMonoBehaviour<CurtainShowerView>
{
    private CanvasGroup _canvasGroup;

    private void Init()
    {
        if (_canvasGroup != null) return;

        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public async UniTask Show(float time)
    {
        Init();

        gameObject.SetActive(true);

        await _canvasGroup.DOFade(1, time).ToUniTask();
    }

    public async UniTask Hide(float time)
    {
        Init();

        await _canvasGroup.DOFade(0, time).ToUniTask();

        gameObject.SetActive(false);
    }
}