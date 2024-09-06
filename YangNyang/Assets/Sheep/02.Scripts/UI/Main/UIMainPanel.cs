using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIMainPanel : UIPanel
{
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
    private GameObject _redDot;


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
        _levelText.text = $"레벨:";
    }
    private void UserStorage_OnUpdateExp(ulong exp, ulong amount = 0)
    {
        _expText.text = $"경험치:exp";
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


}
