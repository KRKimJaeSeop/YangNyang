using System.Collections;
using System.Collections.Generic;
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
        LocalizationSettings.SelectedLocale = locale;
    }
    private void OnClickKorBtn()
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale("ko-KR");
        LocalizationSettings.SelectedLocale = locale;
    }






}
