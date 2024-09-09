using DG.Tweening;
using System;
using System.Collections;
using System.Text;
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

    public void Show(string speechText, float showTime = 2.0f, bool isTypingAnim = false, Action callback = null)
    {
        gameObject.SetActive(true);
        _text.text = AddLineBreaks(speechText, 10);
        if (_showCoroutine != null)
        {
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }
        _showCoroutine = StartCoroutine(ShowCoroutine(showTime, callback));
    }

    private IEnumerator ShowCoroutine(float showTime, Action callback = null)
    {
        yield return new WaitForSeconds(showTime);
        gameObject.SetActive(false);
        callback?.Invoke();
    }

    private string AddLineBreaks(string text, int maxLineLength)
    {
        string[] words = text.Split(' ');
        StringBuilder result = new StringBuilder();
        StringBuilder line = new StringBuilder();

        foreach (string word in words)
        {
            if ((line.Length + word.Length) > maxLineLength)
            {
                result.AppendLine(line.ToString().TrimEnd());
                line.Clear();
            }
            line.Append(word).Append(" ");
        }

        result.Append(line.ToString().TrimEnd());
        return result.ToString();
    }
}
