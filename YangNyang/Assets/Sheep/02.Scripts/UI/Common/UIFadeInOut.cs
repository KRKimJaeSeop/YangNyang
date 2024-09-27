using System;
using System.Collections;
using UnityEngine;

public class UIFadeInOut : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private bool isPlaying = false;


    /// <summary>
    /// 검정->투명.
    /// </summary>
    public void FadeIn(Action callback = null)
    {
        gameObject.SetActive(true);
        _animator.Play("FadeIn");
        StartCoroutine(FadeInOutCoroutine(callback));
    }

    /// <summary>
    /// 투명->검정.
    /// </summary>
    public void FadeOut(Action callback)
    {
        gameObject.SetActive(true);
        _animator.Play("FadeOut");
        StartCoroutine(FadeInOutCoroutine(callback));
    }

    public void EndAnimation()
    {
        isPlaying = false;
    }
    private IEnumerator FadeInOutCoroutine(Action callback = null)
    {
        isPlaying = true;
        yield return new WaitUntil(() => !isPlaying);
        callback?.Invoke();
        gameObject.SetActive(false);
    }

}
