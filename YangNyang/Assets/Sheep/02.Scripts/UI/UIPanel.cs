using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [Serializable]
    public class GuideDialogSetting
    {
        [SerializeField, Tooltip("���̵���̾�αװ� ���ٸ� enum None����.")]
        private Dialog.Type _dialogType = Dialog.Type.None;
        public Button closeBtn;
        public Button openBtn;
        [SerializeField]
        private GameObject[] _uiGuideObjects;

        public Dialog.Type GetDialogType()
        {
            return _dialogType;
        }

        public void SetActiveObjects(bool isTrue)
        {
            foreach (var item in _uiGuideObjects)
            {
                item.SetActive(isTrue);
            }
        }
    }

    [Header("[SafeArea]")]
    [SerializeField, Tooltip("���ٸ� �̼���")]
    protected SafeAreaHandler safeAreaHandler;

    [SerializeField, Tooltip("���ٸ� �̼���")]
    protected Button[] closeButtons;

    [SerializeField, Tooltip("UI���̵� ���̾�α�. ���ٸ� �̼���")]
    private GuideDialogSetting _guide;

    protected UnityAction<object> _cbClose = null;
    protected object _results = null;
    [Header("Feel")]
    [SerializeField, Tooltip("���ٸ� �̼���")]
    protected MMF_Player _feedback_popSound;
    [SerializeField, Tooltip("���ٸ� �̼���")]
    protected MMF_Player _feedback_OnEnable;


    protected virtual void Awake()
    {
        foreach (var button in closeButtons)
        {
            button.onClick.AddListener(OnClickClose);
        }
        if (_guide.GetDialogType() != Dialog.Type.None)
        {
            _guide.closeBtn.onClick.AddListener(OnClickGuideCloseBtn);
            _guide.openBtn.onClick.AddListener(OnClickGuideOpenBtn);
        }
    }

    public virtual void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        this.gameObject.SetActive(true);
        if (canvas != null && safeAreaHandler != null)
            safeAreaHandler.SetCanvas(canvas);
        _cbClose = cbClose;
        _results = null;
        SetGuideDialogObjects(_guide.GetDialogType());
        Begin();
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
        _feedback_OnEnable?.PlayFeedbacks();
    }

    private void End()
    {
        this.gameObject.SetActive(false);
        _feedback_popSound?.PlayFeedbacks();
        _cbClose?.Invoke(_results);
    }

    private void OnClickGuideOpenBtn()
    {
        _guide.SetActiveObjects(true);
        _feedback_popSound.PlayFeedbacks();
    }
    private void OnClickGuideCloseBtn()
    {
        _guide.SetActiveObjects(false);
        _feedback_popSound.PlayFeedbacks();
    }
    private void SetGuideDialogObjects(Dialog.Type type)
    {
        if (type == Dialog.Type.None)
            return;

        if (!GameDataManager.Instance.Storages.UnlockDialog.IsUnlockDialogID(type))
        {
            GameDataManager.Instance.Storages.UnlockDialog.UnlockDialog(type);
            _guide.SetActiveObjects(true);
        }
        else
        {
            _guide.SetActiveObjects(false);
        }
    }
}