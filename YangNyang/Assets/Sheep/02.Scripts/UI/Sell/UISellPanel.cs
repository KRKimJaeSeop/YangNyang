using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISellPanel : UIPanel
{
    [SerializeField]
    private Image _woolFillGauge;
    [SerializeField]
    private TextMeshProUGUI _currentGoldText;
    private long _storageWoolAmount;
    private long _cachedWool;
    private long _cachedGold;
    // 한틱에 판매할 양(현재 양털의 0.01퍼센트)
    private long _sellAmount;
    [SerializeField]
    private LocalizationData _goldCurrentLocal;
    [SerializeField]
    private LocalizationData _goldGoalLocal;

    public override void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);
        SetValues();
        SetGauge();
    }
    private void SetValues()
    {
        _storageWoolAmount = GameDataManager.Instance.Storages.Currency.GetAmount(Currency.Type.Wool);
        _cachedWool = _storageWoolAmount;
        _cachedGold = GameDataManager.Instance.Storages.Currency.GetAmount(Currency.Type.Gold);
        _sellAmount = _storageWoolAmount / 100;
        if (_sellAmount == 0)
            _sellAmount = _storageWoolAmount > 0 ? _storageWoolAmount : 1;
        if (_sellAmount > GameManager.Instance.TargetGoldAmount / 100)
        {
            _sellAmount = GameManager.Instance.TargetGoldAmount / 100;
        }
    }

    private void SetGauge()
    {
        _currentGoldText.text = $"{_goldGoalLocal.GetLocalizedString()} : {GameManager.Instance.TargetGoldAmount}\n" +
                                  $"{_goldCurrentLocal.GetLocalizedString()} : {_cachedGold}\n" +
                                  $"{string.Format("{0:F2}",((double)(_cachedGold * 100) /GameManager.Instance.TargetGoldAmount))}%";
        _woolFillGauge.fillAmount = _storageWoolAmount > 0 ? (float)((double)_cachedWool / _storageWoolAmount) : 0;
    }

    // 이 함수를 롱버튼의 UnityEvent에 등록한다.
    public void Sell()
    {
        if (_cachedWool > 0)
        {
            if (_cachedWool > _sellAmount)
            {
                _cachedWool = GameDataManager.Instance.Storages.Currency.Decrease(Currency.Type.Wool, _sellAmount).value;
                _cachedGold = GameDataManager.Instance.Storages.Currency.Increase(Currency.Type.Gold, _sellAmount);
            }
            else
            {
                long remainingWool = _cachedWool;
                _cachedWool = GameDataManager.Instance.Storages.Currency.Decrease(Currency.Type.Wool, _cachedWool).value;
                _cachedGold = GameDataManager.Instance.Storages.Currency.Increase(Currency.Type.Gold, remainingWool);
            }
            _feedback_popSound.PlayFeedbacks();
            SetGauge();
        }
    }
}