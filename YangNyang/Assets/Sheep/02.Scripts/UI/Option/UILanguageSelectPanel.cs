using Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UILanguageSelectPanel : UIPanel
{
    [SerializeField]
    private Button _engBtn;
    [SerializeField]
    private Button _korBtn;
    protected override void Awake()
    {
        base.Awake();
        _engBtn.onClick.AddListener(OnClickEngBtn);
        _korBtn.onClick.AddListener(OnClickKorBtn);
    }
    private void OnClickEngBtn()
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale("en");
        _feedback_popSound.PlayFeedbacks();
        LocalizationSettings.SelectedLocale = locale;
        GameDataManager.Instance.Storages.Preference.SetLanguageCode("en");
    }
    private void OnClickKorBtn()
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale("ko-KR");
        _feedback_popSound.PlayFeedbacks();
        LocalizationSettings.SelectedLocale = locale;
        GameDataManager.Instance.Storages.Preference.SetLanguageCode("ko-KR");
    }






}
