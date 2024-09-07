using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{

    [Header("[Open]")]
    [SerializeField, Tooltip("�г� ���� �� �̺�Ʈ")]
    protected UnityEvent onOpen;

    [Header("[Close]")]
    [SerializeField, Tooltip("�г� ���� �� �̺�Ʈ")]
    protected UnityEvent onClose;
    [SerializeField, Tooltip("���ٸ� �̼���")]
    protected Button[] closeButtons;


    protected UnityAction<object> _cbClose = null;
    protected object _results = null;


    protected virtual void Awake()
    {
        foreach (var button in closeButtons)
        {
            button.onClick.AddListener(OnClickClose);
        }
    }

    public virtual void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        this.gameObject.SetActive(true);
        _cbClose = cbClose;
        _results = null;

        Begin();
    }
    public void PunchAnimation(GameObject go)
    {
        go.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }

    public virtual void Close()
    {
        if (!isActiveAndEnabled)
            return;
        End();
    }

    protected void OnClickClose()
    {
        Close();
    }


    private void Begin()
    {
        onOpen?.Invoke();
    }

    private void End()
    {
        this.gameObject.SetActive(false);

        _cbClose?.Invoke(_results);
        onClose?.Invoke();
    }

    IEnumerator WaitCoroutine(float waitTime, Action cbFinished)
    {
        yield return new WaitForSeconds(waitTime);
        cbFinished?.Invoke();
    }
}