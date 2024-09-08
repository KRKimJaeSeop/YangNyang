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
    public delegate void GameClearEvent(EndingType endingType);
    public static event GameClearEvent OnGameClear;

    [SerializeField]
    private long _targetGoldAmount;
    private bool isGameClear = false;

    private void Awake()
    {
        InitializeScene();
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

    private void InitializeScene()
    {
        Application.targetFrameRate = 60;
        GameDataManager.Instance.Initialize();
        FieldObjectManager.Instance.Initialize();

        UIManager.Instance.Preload();

        UIManager.Instance.OpenMainPanel();
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
            var endingType = GetEndingType();
            StartCoroutine(OnGameClearCoroutine(() =>
            {
                UIManager.Instance.OpenResultPanel("레벨이 올랐어요!", $"{endingType}로 엔딩봄. \n 능력치 ~~ 올라감~!");
                isGameClear = false;

            }));
            OnGameClear?.Invoke(endingType);
        }
    }

    IEnumerator OnGameClearCoroutine(Action callback)
    {
        UIManager.Instance.OpenLoading();
        yield return new WaitForSeconds(5f);
        UIManager.Instance.CloseLoading();
        callback?.Invoke();
    }

}
