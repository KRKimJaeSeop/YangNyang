using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMainPanel : UIPanel
{
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

    protected override void Awake()
    {
        base.Awake();
        _sellBtn.onClick.AddListener(OnClickShopBtn);
        _collectionBtn.onClick.AddListener(OnClickCollectionBtn);
        _optionBtn.onClick.AddListener(OnClickOptionBtn);
        _researchBtn.onClick.AddListener(OnClickResearchBtn);
        SetEnvironmentUI();
    }

    private void OnEnable()
    {
        UserStorage.OnUpdateExp += UserStorage_OnUpdateExp;
        UserStorage.OnUpdateDay += UserStorage_OnUpdateDay;
        UserStorage.OnUpdateLevel += UserStorage_OnUpdateLevel;
        CurrencyStorage.OnUpdateCurrency += CurrencyStorage_OnUpdateCurrency;
        UnlockSheepStorage.OnUnlockSheep += UnlockSheepStorage_OnUnlockSheep;

    }

    private void OnDisable()
    {
        UserStorage.OnUpdateExp -= UserStorage_OnUpdateExp;
        UserStorage.OnUpdateDay -= UserStorage_OnUpdateDay;
        UserStorage.OnUpdateLevel -= UserStorage_OnUpdateLevel;
        UnlockSheepStorage.OnUnlockSheep -= UnlockSheepStorage_OnUnlockSheep;
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
        base.Open();
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
        _dayText.text = $"{day}일차";
    }

    private void UserStorage_OnUpdateLevel(int level)
    {
        _levelText.text = $"레벨:{level}";
    }
    private void UserStorage_OnUpdateExp(long exp, long amount = 0)
    {
        _expText.text = $"경험치:{exp}";
    }
    private void CurrencyStorage_OnUpdateCurrency(Currency.Type code, long total, long amount = 0)
    {
        switch (code)
        {
            case Currency.Type.Wool:
                _woolText.text = $"양털: {total}";
                break;
            case Currency.Type.Gold:
                _goldText.text = $"골드: {total}";
                break;
            default:
                break;
        }
    }

    private void UnlockSheepStorage_OnUnlockSheep(int id)
    {
        _redDot.SetActive(true);
    }
    private void OnClickCollectionBtn()
    {
        UIManager.Instance.OpenCollectionPanel();
        _redDot.SetActive(false);
    }
    private void OnClickShopBtn()
    {
        UIManager.Instance.OpenSellPanel();
    }
    private void OnClickOptionBtn()
    {
        UIManager.Instance.OpenOptionPanel();
    }
    private void OnClickResearchBtn()
    {
        UIManager.Instance.OpenResearchPanel();
    }

}
