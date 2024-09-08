using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIResearchPanel : UIPanel
{
    [SerializeField]
    private Image _woolFillGauge;
    [SerializeField]
    private Image _expFillGauge;
    [SerializeField]
    private TextMeshProUGUI _currentLevelText;
    [SerializeField]
    private TextMeshProUGUI _expGaugeText;

    private long _storageWoolAmount;
    private long _cachedWool;
    private long _maxExp;
    private long _exp;

    // 한틱에 판매할 양(현재 양털의 0.01퍼센트)
    private long _researchAmount;

    public override void Open(Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);
        InitializeResearchData();
    }
    private void OnEnable()
    {
        UserStorage.OnUpdateLevel += UserStorage_OnUpdateLevel;
    }
    private void OnDisable()
    {
        UserStorage.OnUpdateLevel -= UserStorage_OnUpdateLevel;
    }

    private void UserStorage_OnUpdateLevel(int level)
    {
        InitializeResearchData();
    }

    private void InitializeResearchData()
    {
        _storageWoolAmount = GameDataManager.Instance.Storages.Currency.GetAmount(Currency.Type.Wool);
        _cachedWool = _storageWoolAmount;
        var currentLevel = GameDataManager.Instance.Storages.User.ResearchLevel;
        _maxExp = GameDataManager.Instance.Tables.Research.GetMaxExp(currentLevel);
        _exp = GameDataManager.Instance.Storages.User.ResearchExp;
        _researchAmount = _maxExp / 100;
        if (_researchAmount > _maxExp)
        {
            _researchAmount = _maxExp > 0 ? _maxExp : 1;
        }
        _currentLevelText.text = $"{currentLevel}LV";
        SetGauge();
    }

    private void SetGauge()
    {
        _expFillGauge.fillAmount = _maxExp > 0 ? (float)((double)_exp / _maxExp) : 0;
        _woolFillGauge.fillAmount = _storageWoolAmount > 0 ? (float)((double)_cachedWool / _storageWoolAmount) : 0;
        _expGaugeText.text = $"{_exp} / {_maxExp}";
    }

    // 이 함수를 롱버튼의 UnityEvent에 등록한다.
    public void Research()
    {
        if (_cachedWool > 0 && _researchAmount > 0)
        {
            if (_cachedWool > _researchAmount)
            {
                _exp = GameDataManager.Instance.Storages.User.IncreaseExp(_researchAmount);
                _cachedWool = GameDataManager.Instance.Storages.Currency.Decrease(Currency.Type.Wool, _researchAmount).value;
            }
            else
            {
                long remainingWool = _cachedWool;
                _exp = GameDataManager.Instance.Storages.User.IncreaseExp(remainingWool);
                _cachedWool = GameDataManager.Instance.Storages.Currency.Decrease(Currency.Type.Wool, _cachedWool).value;
            }
            SetGauge();
        }
    }
}