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
            // ����� �ð��� �� ������.
            return EndingType.Birthday;
        }

        // ��� ���� �ر���.
        if (GameDataManager.Instance.Storages.UnlockSheep.IsUnlockAllSheep())
        {
            // �ر��� �ߴµ�, ���� ������ 1�ΰ��.
            if (GameDataManager.Instance.Storages.User.ResearchLevel == 1)
            {
                return EndingType.CrazyLucky;
            }

            // �������� ������� ��� ���� �ر���.
            return EndingType.Happy;
        }
        else
        {
            if (GameDataManager.Instance.Storages.User.Day == 1)
            {
                // ����ִ� ���� �����ְ�, ù°��
                return EndingType.First;
            }
            else
            {
                // ����ִ� ���� �����ְ�, ù°�� ����
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
                UIManager.Instance.OpenResultPanel("������ �ö����!", $"{endingType}�� ������. \n �ɷ�ġ ~~ �ö�~!");
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
