using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [Header("[SafeArea]")]
    [SerializeField, Tooltip("없다면 미세팅")]
    protected SafeAreaHandler safeAreaHandler;

    [Header("[Open]")]
    [SerializeField, Tooltip("패널 오픈 시 이벤트")]
    protected UnityEvent onOpen;

    [Header("[Close]")]
    [SerializeField, Tooltip("패널 닫힐 시 이벤트")]
    protected UnityEvent onClose;
    [SerializeField, Tooltip("없다면 미세팅")]
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
        if (canvas != null && safeAreaHandler != null)
            safeAreaHandler.SetCanvas(canvas);

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