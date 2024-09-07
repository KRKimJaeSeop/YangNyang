using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UINotificationPanel : UIPanel
{

    public override void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas,cbClose);
        StartCoroutine(CloseCoru());
    }
    IEnumerator CloseCoru()
    {
        yield return new WaitForSeconds(1f);
        Close();
    }

}
