using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionPanel : UIPanel
{
    [SerializeField]
    private Button _languageBtn;
    [SerializeField]
    private Slider _masterVolumeSlier;
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
        _masterVolumeSlier.onValueChanged.AddListener(OnMasterValueChanged);
        _bgmVolumeSlier.onValueChanged.AddListener(OnBGMValueChanged);
        _sfxVolumeSlier.onValueChanged.AddListener(OnSFXValueChanged);
        _languageBtn.onClick.AddListener(OnClickLangaugeBtn);
        _watchDialogBtn.onClick.AddListener(OnClickDialogBtn);
        _creditBtn.onClick.AddListener(OnClickCreditBtn);
        _termsBtn.onClick.AddListener(OnClickTermsBtn);
    }
    void OnMasterValueChanged(float value)
    {
        Debug.Log("OnMasterValueChanged Value: " + value);
    }
    void OnBGMValueChanged(float value)
    {
        Debug.Log("OnBGMValueChanged Value: " + value);
    }
    void OnSFXValueChanged(float value)
    {
        Debug.Log("OnSFXValueChanged Value: " + value);
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
        UIManager.Instance.OpenNotificationPanel("¾à°ü¶ç¿ì±â.");
    }

}
