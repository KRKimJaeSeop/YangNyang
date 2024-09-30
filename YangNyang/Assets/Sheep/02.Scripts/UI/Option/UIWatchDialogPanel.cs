using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWatchDialogPanel : UIPanel
{
    [Serializable]
    public class ReplayButtonInfo
    {
        [SerializeField]
        private Button _btn;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Dialog.Type _type;

        public void Initialize()
        {
            _btn.onClick.AddListener(Play);
        }
        public void Play()
        {
            DialogManager.Instance.EnterDialog(_type);
        }

        public void SetBtn()
        {
            if (GameDataManager.Instance.Storages.UnlockDialog.IsUnlockDialogID(_type))
            {
                _btn.interactable = true;
                _text.text = $"# {GameDataManager.Instance.Tables.Dialog.GetUnit(_type).localName.GetLocalizedString()}";
            }
            else
            {
                _btn.interactable = false;
                _text.text = $"# ???";
            }


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
