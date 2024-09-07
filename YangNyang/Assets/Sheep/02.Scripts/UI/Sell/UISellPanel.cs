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
    public override void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);
        _storageWoolAmount = GameDataManager.Instance.Storages.Currency.GetAmount(Currency.Type.Wool);
        _cachedWool = _storageWoolAmount;
        _cachedGold = GameDataManager.Instance.Storages.Currency.GetAmount(Currency.Type.Gold);
        _sellAmount = _storageWoolAmount / 100;
        if (_sellAmount == 0)
            _sellAmount = _storageWoolAmount;
        SetUI();
    }

    private void SetUI()
    {
        _currentGoldText.text = $"{_cachedGold}";
        _woolFillGauge.fillAmount = (float)((double)_cachedWool / _storageWoolAmount);

    }

    // 이 함수를 롱버튼의 UnityEvent에 등록한다.
    public void Sell()
    {
        if (_cachedWool > 0)
        {
            if (_cachedWool > _sellAmount)
            {
                _cachedGold = GameDataManager.Instance.Storages.Currency.Increase(Currency.Type.Gold, _sellAmount);
                _cachedWool = GameDataManager.Instance.Storages.Currency.Decrease(Currency.Type.Wool, _sellAmount).value;
            }
            else
            {
                _cachedGold = GameDataManager.Instance.Storages.Currency.Increase(Currency.Type.Gold, _cachedWool);
                _cachedWool = GameDataManager.Instance.Storages.Currency.Decrease(Currency.Type.Wool, _cachedWool).value;
            }
            SetUI();
        }
    }
}
