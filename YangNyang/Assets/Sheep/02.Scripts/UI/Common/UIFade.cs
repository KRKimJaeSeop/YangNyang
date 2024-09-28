using System;
using System.Collections;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private bool isPlaying = false;

    public void OpenFadeOutIn(Action onFadeOut = null , Action onFadeIn = null)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeInOutCoroutine(onFadeOut,onFadeIn));
    }
    public void EndAnimation()
    {
        isPlaying = false;
    }

    private IEnumerator FadeInOutCoroutine(Action onFadeOut = null, Action onFadeIn = null)
    {
        //Fade Out
        isPlaying = true;
        _animator.Play("FadeOut");
        yield return new WaitUntil(() => !isPlaying);
        //Fade Out Callback
        onFadeOut?.Invoke();

        //Fade In
        isPlaying = true;
        _animator.Play("FadeIn");
        yield return new WaitUntil(() => !isPlaying);
        //Fade In Callback

        //yield return new WaitForEndOfFrame();
        onFadeIn?.Invoke();
        gameObject.SetActive(false);


    }


}
