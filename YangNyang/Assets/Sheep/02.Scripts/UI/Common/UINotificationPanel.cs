using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UINotificationPanel : UIPanel
{
    [SerializeField]
    private TextMeshProUGUI _contentText;

    private Coroutine _showPanelCoroutine;
    public void Open(string content, Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);
        if (_showPanelCoroutine != null)
        {
            StopCoroutine(_showPanelCoroutine);
        }
        _contentText.text = content;      
        _showPanelCoroutine =StartCoroutine(ShowPanelCoroutine());
    }
    IEnumerator ShowPanelCoroutine()
    {
        yield return new WaitForSeconds(5f);
        Close();
    }

}
