using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using static GameManager;
using Object = UnityEngine.Object;

public class GameDataManager : Singleton<GameDataManager>
{
    [SerializeField]
    private TableContainer _tables;
    public TableContainer Tables { get { return _tables; } }

    [SerializeField]
    private StorageContainer _storage;
    public StorageContainer Storages { get { return _storage; } }
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void OnEnable()
    {
        OnGameClear += GameManager_OnGameClear;
    }
    private void OnDisable()
    {
        OnGameClear -= GameManager_OnGameClear;
    }

    public virtual bool Initialize()
    {
        IsInitialized = false;

        if (IsInitialized)
            return true;

        // table 초기화 (storage 로딩하기 전에 먼저 초기화 되어 있어야 한다.)
        if (!_tables.Initialize())
            return false;

        _storage.Initialize();

        var languageCode = Storages.Preference.GetLanguageCode();
        if (!string.IsNullOrEmpty(languageCode))
        {
            var locale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            var locale = LocalizationSettings.AvailableLocales.GetLocale("ko-KR");
            LocalizationSettings.SelectedLocale = locale;
        }

        //LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);

        IsInitialized = true;
        return true;
    }

    public async Task<T> LoadAssetAsync<T>(string assetName, Action<T> onAssetLoaded) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetName);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            onAssetLoaded?.Invoke(handle.Result);
            return handle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load asset: {assetName}");
            return null;
        }
    }


    private void GameManager_OnGameClear(EndingType type)
    {
        switch (type)
        {
            case EndingType.CrazyLucky:
                Storages.User.IncreaseDay(999);
                break;
            case EndingType.Birthday:
                Storages.User.IncreaseDay(999);
                break;
            default:
                Storages.User.IncreaseDay(1);
                Storages.Currency.Decrease
                     (Currency.Type.Gold, Storages.Currency.GetAmount(Currency.Type.Gold));
                break;
        }
        Storages.Save();

    }


}
