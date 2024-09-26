using MoreMountains.Feedbacks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 예, 아니오, 취소 등 확인받는 팝업이다.
/// </summary>
public class UIConfirmPanel : UIPanel
{
    [Serializable]
    public class Results
    {
        public bool isConfirm;
    }

    [Header("[Buttons]")]
    [SerializeField] protected Button _btnConfirm;
    [SerializeField] protected Button _btnCancel; // 취소/닫기 버튼
    [Header("[Texts]")]
    [SerializeField] protected TextMeshProUGUI _titleText;
    [SerializeField] protected TextMeshProUGUI _contentText;
    //[SerializeField] protected TextMeshProUGUI _confirmText;
    //[SerializeField] protected TextMeshProUGUI _cancleText;



    protected override void Awake()
    {
        base.Awake();

        if (_btnConfirm != null)
            _btnConfirm.onClick.AddListener(OnClickConfirm);
        if (_btnCancel != null)
            _btnCancel.onClick.AddListener(OnClickCancel);
    }

    public void Open(string title,
        string content,
        Canvas canvas = null,
        UnityAction<object> cbClose = null)
    {
        if (string.IsNullOrEmpty(content))
            return;

        base.Open(canvas, cbClose);

        _titleText.text = title;
        _contentText.text = content;
    }

    public override void Close()
    {
        // 닫힐 때 디폴트로 취소 처리.
        Close(false);
    }

    protected virtual void OnClickConfirm()
    {
        Close(true);
    }

    protected virtual void OnClickCancel()
    {
        Close(false);
    }

    protected void Close(bool isOK)
    {
        Results result = new Results();
        result.isConfirm = isOK;
        _results = result;

        //// 배너 광고
        //AdvertisingController.Instance.StopBanner();

        base.Close();
    }
}
