using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIResultPanel : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private TextMeshProUGUI _contentText;
    public void Open(string title, string content, Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);
        _titleText.text = title;
        _contentText.text = content;

    }


}
