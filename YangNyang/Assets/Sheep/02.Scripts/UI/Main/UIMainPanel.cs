using Localization;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMainPanel : UIPanel
{
    [Header("[UIMainPanel]")]
    [Header("Status Bar UI")]
    [SerializeField]
    private TextMeshProUGUI _dayText;
    [SerializeField]
    private TextMeshProUGUI _levelText;
    [SerializeField]
    private TextMeshProUGUI _expText;
    [SerializeField]
    private TextMeshProUGUI _woolText;
    [SerializeField]
    private TextMeshProUGUI _goldText;
    [SerializeField]
    private LocalizationData _researchLocal;
    [SerializeField]
    private LocalizationData _levelLocal;
    [SerializeField]
    private LocalizationData _expLocal;
    [SerializeField]
    private LocalizationData _dayLocal;
    [SerializeField]
    private LocalizationData _goldTextLocal;
    [SerializeField]
    private LocalizationData _woolTextLocal;

    [Header("Button Bar UI")]
    [SerializeField]
    private Button _sellBtn;
    [SerializeField]
    private Button _researchBtn;
    [SerializeField]
    private Button _collectionBtn;
    [SerializeField]
    private GameObject _redDot;
    [SerializeField]
    private Button _optionBtn;

    [Header("Remote Asset UI")]
    [SerializeField]
    private Image _overlayBranch;
    [SerializeField]
    private Image _overlayGrass;
    [SerializeField]
    private Image[] _docBar;

    private Vector2 _rectOrigin;
    private Canvas _canvas;

    [SerializeField]
    private GameObject _buffGaugeParent;
    [SerializeField]
    private Image _buffGaugeFillImage;

    private Coroutine _buffSheepSpawn;


    protected override void Awake()
    {
        base.Awake();
        _sellBtn.onClick.AddListener(OnClickShopBtn);
        _collectionBtn.onClick.AddListener(OnClickCollectionBtn);
        _optionBtn.onClick.AddListener(OnClickOptionBtn);
        _researchBtn.onClick.AddListener(OnClickResearchBtn);
        SetEnvironmentUI();
        _rectOrigin = _docBar[0].rectTransform.sizeDelta;
    }

    private void OnEnable()
    {
        UserStorage.OnUpdateExp += UserStorage_OnUpdateExp;
        UserStorage.OnUpdateDay += UserStorage_OnUpdateDay;
        UserStorage.OnUpdateLevel += UserStorage_OnUpdateLevel;
        CurrencyStorage.OnUpdateCurrency += CurrencyStorage_OnUpdateCurrency;
        UnlockSheepStorage.OnUnlockSheep += UnlockSheepStorage_OnUnlockSheep;
        AdvertisingController.OnBannerActive += AdvertisingController_OnBannerActive;
        PreferenceStorage.OnUpdateLanguage += OnUpdateLanguage;
    }



    private void OnDisable()
    {
        UserStorage.OnUpdateExp -= UserStorage_OnUpdateExp;
        UserStorage.OnUpdateDay -= UserStorage_OnUpdateDay;
        UserStorage.OnUpdateLevel -= UserStorage_OnUpdateLevel;
        UnlockSheepStorage.OnUnlockSheep -= UnlockSheepStorage_OnUnlockSheep;
        AdvertisingController.OnBannerActive -= AdvertisingController_OnBannerActive;
        PreferenceStorage.OnUpdateLanguage -= OnUpdateLanguage;
    }


    private void SetEnvironmentUI()
    {
        _overlayBranch.sprite =
            AddressableManager.Instance.GetAsset<Sprite>(AddressableManager.RemoteAssetCode.OverlayBranch);
        _overlayGrass.sprite =
            AddressableManager.Instance.GetAsset<Sprite>(AddressableManager.RemoteAssetCode.OverlayGrass);
        foreach (var item in _docBar)
        {
            item.sprite =
                AddressableManager.Instance.GetAsset<Sprite>(AddressableManager.RemoteAssetCode.DocBar);
        }
    }

    public override void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);
        _canvas = canvas;
        SetUI();
    }
    private void OnUpdateLanguage(string code)
    {
        SetUI();
    }
    private void SetUI()
    {
        var userStorage = GameDataManager.Instance.Storages.User;
        UserStorage_OnUpdateDay(userStorage.Day);
        UserStorage_OnUpdateLevel(userStorage.ResearchLevel);
        UserStorage_OnUpdateExp(userStorage.ResearchExp);
        List<CurrencyStorage.CurrencyData> currencyList = GameDataManager.Instance.Storages.Currency.Data.currencies;
        foreach (var currency in currencyList)
        {
            CurrencyStorage_OnUpdateCurrency(currency.code, currency.amount);
        }

    }
  
    private void UserStorage_OnUpdateDay(int day)
    {
        if (GameDataManager.Instance.Storages.Preference.GetLanguageCode() == "ko-KR")
        {
            _dayText.text = $"{day} {_dayLocal.GetLocalizedString()}";
        }
        else
        {
            _dayText.text = $"{_dayLocal.GetLocalizedString()} {day}";

        }
    }

    private void UserStorage_OnUpdateLevel(int level)
    {
        _levelText.text = $"{_levelLocal.GetLocalizedString()}:{level}";
    }
    private void UserStorage_OnUpdateExp(long exp, long amount = 0)
    {
        _expText.text = $"{_expLocal.GetLocalizedString()}:{exp}";
    }
    private void CurrencyStorage_OnUpdateCurrency(Currency.Type code, long total, long amount = 0)
    {
        switch (code)
        {
            case Currency.Type.Wool:
                _woolText.text = $"{_woolTextLocal.GetLocalizedString()}: {total}";
                break;
            case Currency.Type.Gold:
                _goldText.text = $"{_goldTextLocal.GetLocalizedString()}: {total} ({string.Format("{0:D2}", ((total * 100) / GameManager.Instance.TargetGoldAmount))}%)";
                break;
            default:
                break;
        }
    }

    private void UnlockSheepStorage_OnUnlockSheep(int id)
    {
        _redDot.SetActive(true);
    }
    #region Buttons
    private void OnClickCollectionBtn()
    {
        UIManager.Instance.OpenCollectionPanel();
        _feedback_popSound.PlayFeedbacks(); 
        _redDot.SetActive(false);
    }
    private void OnClickShopBtn()
    {
        _feedback_popSound.PlayFeedbacks(); 
        UIManager.Instance.OpenSellPanel(
            (close) =>
            {
                GameDataManager.Instance.Storages.Currency.Save();
            });
    }
    private void OnClickOptionBtn()
    {
        _feedback_popSound.PlayFeedbacks(); 
        UIManager.Instance.OpenOptionPanel(
            (close) =>
            {
                GameDataManager.Instance.Storages.Preference.Save();
            });
    }
    private void OnClickResearchBtn()
    {
        _feedback_popSound.PlayFeedbacks(); 
        UIManager.Instance.OpenResearchPanel(
             (close) =>
              {
                  GameDataManager.Instance.Storages.User.Save();
                  GameDataManager.Instance.Storages.Currency.Save();
              });
    }
    #endregion

    private void AdvertisingController_OnBannerActive(bool isShow)
    {
        _docBar[0].rectTransform.anchorMin = new Vector2(0, 1);
        _docBar[0].rectTransform.anchorMax = new Vector2(1, 1);
        _docBar[0].rectTransform.pivot = new Vector2(0.5f, 1);

        // 높이 설정 (예: 100 픽셀)
        float newHeight = GetBannerHeightInCanvasUnits(isShow);
        if (isShow)
        {
            _docBar[0].rectTransform.sizeDelta += new Vector2(_docBar[0].rectTransform.sizeDelta.x, newHeight);
        }
        else
        {
            _docBar[0].rectTransform.sizeDelta = _rectOrigin;
        }
        // 화면의 맨 위에 배치
        _docBar[0].rectTransform.anchoredPosition = new Vector2(0, 0);

    }
    float GetBannerHeightInCanvasUnits(bool isShow)
    {
        if (isShow)
        {
#if UNITY_EDITOR

            return 150;

#else
            float bannerHeightInPixels = AdvertisingController.Instance.GetBannerHeightByPixel();
            float canvasScaleFactor = _canvas.scaleFactor;
            float bannerHeightInCanvasUnits = bannerHeightInPixels / canvasScaleFactor;
            return bannerHeightInCanvasUnits;
#endif
        }
        else
        {
            return 0;
        }

    }

    public void StartBuffCountdown()
    {
        _buffGaugeParent.SetActive(true);

        if (_buffSheepSpawn != null)
        {
            StopCoroutine(_buffSheepSpawn);
        }

        _buffGaugeFillImage.fillAmount = 0;
        _buffSheepSpawn =
            StartCoroutine(BuffCountdown());

    }
    private IEnumerator BuffCountdown()
    {
        var wfs = new WaitForSeconds(0.1f);
        float duration = 30f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += 0.1f;
            _buffGaugeFillImage.fillAmount = Mathf.Lerp(0, 1, elapsedTime / duration);
            yield return wfs;
        }
        _buffGaugeParent.SetActive(false);

    }

}