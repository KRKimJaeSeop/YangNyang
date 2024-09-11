using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIOptionPanel : UIPanel
{
    [SerializeField]
    private Button _languageBtn;
    [SerializeField]
    private Slider _bgmVolumeSlier;
    [SerializeField]
    private Slider _sfxVolumeSlier;
    [SerializeField]
    private Button _watchDialogBtn;
    [SerializeField]
    private Button _creditBtn;
    [SerializeField]
    private Button _termsBtn;

    protected override void Awake()
    {
        base.Awake();
        _bgmVolumeSlier.onValueChanged.AddListener(OnBGMValueChanged);
        _sfxVolumeSlier.onValueChanged.AddListener(OnSFXValueChanged);
        _languageBtn.onClick.AddListener(OnClickLangaugeBtn);
        _watchDialogBtn.onClick.AddListener(OnClickDialogBtn);
        _creditBtn.onClick.AddListener(OnClickCreditBtn);
        _termsBtn.onClick.AddListener(OnClickTermsBtn);
    }

    public override void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);

        _bgmVolumeSlier.value = GameDataManager.Instance.Storages.Preference.GetVolume(AudioManager.MixerGroup.BGM);
        _sfxVolumeSlier.value = GameDataManager.Instance.Storages.Preference.GetVolume(AudioManager.MixerGroup.SFXMaster);

    }
  
    void OnBGMValueChanged(float value)
    {
        AudioManager.Instance.SetVolume(AudioManager.MixerGroup.BGM, value);
        GameDataManager.Instance.Storages.Preference.SetVolume(AudioManager.MixerGroup.BGM, value);
    }
    void OnSFXValueChanged(float value)
    {
        AudioManager.Instance.SetVolume(AudioManager.MixerGroup.SFXMaster, value);
        GameDataManager.Instance.Storages.Preference.SetVolume(AudioManager.MixerGroup.SFXMaster, value);
    }
    void OnClickASD()
    {
        Debug.Log("Dl qnqnsdp ");           
    }
    private void OnClickLangaugeBtn()
    {
        UIManager.Instance.OpenLanguageSelectPanel();
    }
    private void OnClickDialogBtn()
    {
        UIManager.Instance.OpenWatchDialogPanel();

    }
    private void OnClickCreditBtn()
    {
        UIManager.Instance.OpenCreditPanel();
    }
    private void OnClickTermsBtn()
    {
        UIManager.Instance.OpenNotificationPanel("�������.");
    }

}