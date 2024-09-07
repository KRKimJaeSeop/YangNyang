using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private Coroutine _showCoroutine;

    private Vector3 _originScale;
    private Vector3 _flipScale;
    private void Awake()
    {
        _originScale = transform.localScale;
        _flipScale = new Vector3(-(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    public void Flip(bool isFlip)
    {
        if (isFlip)
        {
            transform.localScale = _flipScale;
        }
        else
        {
            transform.localScale = _originScale;
        }

    }

    public void Show(string speechText, float showTime = 2.0f, bool isTypingAnim = false)
    {
        gameObject.SetActive(true);
        _text.text = speechText;

        // TypingAnim 구현 필요

        if (_showCoroutine != null)
        {
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }
        _showCoroutine = StartCoroutine(ShowCoroutine(showTime));
    }

    private IEnumerator ShowCoroutine(float showTime)
    {
        yield return new WaitForSeconds(showTime);
        gameObject.SetActive(false);

    }
}
