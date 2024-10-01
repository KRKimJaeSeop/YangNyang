using Localization;
using System;
using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum EndingType
    {
        None = 0,
        First = 1,
        Sad = 2,
        Happy = 3,
        CrazyLucky = 4,
        Birthday = 5,

    }
    [SerializeField]
    private long _targetGoldAmount;
    public long TargetGoldAmount { get { return _targetGoldAmount; } }
    private bool isGameClear = false;
    [SerializeField]
    private EnvironmentManager _environment;

    public delegate void GameClearEvent(EndingType endingType);
    public static event GameClearEvent OnGameClear;

    private Coroutine _autoSaveCoroutine;
    [SerializeField]
    private float _autoSaveInterval;
    [SerializeField]
    private LocalizationData _replayGuide;
    [SerializeField]
    private LocalizationData _gameClearTitleLocal;
    [SerializeField]
    private LocalizationData _gameClearContentLocal;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        InitializeGame();
    }
    private void OnEnable()
    {
        CurrencyStorage.OnUpdateCurrency += CurrencyStorage_OnUpdateCurrency;
    }

    private void OnDisable()
    {
        CurrencyStorage.OnUpdateCurrency -= CurrencyStorage_OnUpdateCurrency;
    }

    private void CurrencyStorage_OnUpdateCurrency(Currency.Type code, long total, long amount)
    {
        if (code == Currency.Type.Gold && _targetGoldAmount <= total)
        {
            GameClear();
        }
    }

    private async void InitializeGame()
    {
        UIManager.Instance.OpenLoading();

        // Addressable Load
        await AddressableManager.Instance.LoadAllAssetsAsync();

        // Data
        GameDataManager.Instance.Initialize();

        // UI
        UIManager.Instance.Preload();
        UIManager.Instance.OpenMainPanel();

        // Field
        FieldObjectManager.Instance.Initialize();
        FieldObjectManager.Instance.SpawnPlayer();
        _environment.Initialize();

     
        // Audio
        AudioManager.Instance.Initialize(
           GameDataManager.Instance.Storages.Preference.GetVolume(AudioManager.MixerGroup.BGM),
           GameDataManager.Instance.Storages.Preference.GetVolume(AudioManager.MixerGroup.SFXMaster));
        AudioManager.Instance.MusicBox.PlayBGM(AddressableManager.RemoteAssetCode.BGM);

        //AD
        AdvertisingController.Instance.Initialize();
        AdvertisingController.Instance.ShowBanner();

        _autoSaveCoroutine = StartCoroutine(AutoSaveCoroutine(_autoSaveInterval));

       
        // End Loading
        UIManager.Instance.CloseLoading();

        if (!GameDataManager.Instance.Storages.UnlockDialog.IsUnlockDialogID(Dialog.Type.FirstTutorial))
        {
            DialogManager.Instance.EnterDialog(Dialog.Type.FirstTutorial, () =>
            {
                UIManager.Instance.OpenNotificationPanel(_replayGuide.GetLocalizedString());
            });
        }
    }

    private EndingType GetEndingType()
    {

        DateTime targetDateTime = new DateTime(1999, 6, 11, 0, 0, 0);
        if (DateTime.Now.Year == targetDateTime.Year && DateTime.Now.Month == targetDateTime.Month &&
            DateTime.Now.Date == targetDateTime.Date)
        {
            // 기기의 시간이 내 생일임.
            return EndingType.Birthday;
        }

        // 모든 양을 해금함.
        if (GameDataManager.Instance.Storages.UnlockSheep.IsUnlockAllSheep())
        {
            // 해금을 했는데, 연구 레벨이 1인경우.
            if (GameDataManager.Instance.Storages.User.ResearchLevel == 1)
            {
                return EndingType.CrazyLucky;
            }

            // 정석적인 방법으로 모든 양을 해금함.
            return EndingType.Happy;
        }
        else
        {
            if (GameDataManager.Instance.Storages.User.Day == 1)
            {
                // 잠겨있는 양이 남아있고, 첫째날
                return EndingType.First;
            }
            else
            {
                // 잠겨있는 양이 남아있고, 첫째날 이후
                return EndingType.Sad;
            }
        }

    }

    public void GameClear()
    {
        if (!isGameClear)
        {
            isGameClear = true;
            UIManager.Instance.CloseAll();
            UIManager.Instance.OpenResultPanel(_gameClearTitleLocal.GetLocalizedString(), _gameClearContentLocal.GetLocalizedString(),
                (cbClose) =>
                {
                    var endingType = GetEndingType();
                    OnGameClear?.Invoke(endingType);
                });

         
        }
    }

    IEnumerator OnGameClearCoroutine(Action callback)
    {
        UIManager.Instance.OpenLoading();
        yield return new WaitForSeconds(5f);
        UIManager.Instance.CloseLoading();
        callback?.Invoke();
    }


    private IEnumerator AutoSaveCoroutine(float waitSecond)
    {
        var _wfs = new WaitForSeconds(waitSecond);

        while (true)
        {
            yield return _wfs;
            GameDataManager.Instance.Storages.Save();
        }

    }

}
