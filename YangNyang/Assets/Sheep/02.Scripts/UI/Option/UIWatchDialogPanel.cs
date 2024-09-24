using System;
using UnityEngine;
using UnityEngine.UI;

public class UIWatchDialogPanel : UIPanel
{
    [Serializable]
    public class ReplayButtonInfo
    {
        [SerializeField]
        private Button btn;
        [SerializeField]
        private Dialog.Type _type;

        public void Initialize()
        {
            btn.onClick.AddListener(Play);
        }
        public void Play()
        {
            DialogManager.Instance.EnterDialog(_type);
        }

        public void SetBtn()
        {
            btn.interactable = (GameDataManager.Instance.Storages.UnlockDialog.IsUnlockDialogID(_type));
        }
    }

    [SerializeField]
    private ReplayButtonInfo[] _replayBtns;

    protected override void Awake()
    {
        base.Awake();
        foreach (var btn in _replayBtns)
        {
            btn.Initialize();
        }

    }

    private void OnEnable()
    {
        foreach (var btn in _replayBtns)
        {
            btn.SetBtn();
        }
    }



}
