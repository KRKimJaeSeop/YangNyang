using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UINotificationPanel : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI _contentText;

    public void Open(string content, Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);
        _contentText.text = content;
        StartCoroutine(CloseCoru());
    }
    IEnumerator CloseCoru()
    {
        yield return new WaitForSeconds(1f);
        Close();
    }

}
