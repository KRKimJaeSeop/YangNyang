using UnityEngine;

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
    public virtual bool Initialize()
    {
        IsInitialized = false;

        if (IsInitialized)
            return true;

        // table 초기화 (storage 로딩하기 전에 먼저 초기화 되어 있어야 한다.)
        if (!_tables.Initialize())
            return false;

        _storage.Initialize();

        IsInitialized = true;
        return true;
    }
    public void SetDataPassedDay()
    {
        Storages.User.IncreaseDay(1);

        Storages.Currency.Decrease
            (Currency.Type.Wool, Storages.Currency.GetAmount(Currency.Type.Wool));
        Storages.Currency.Decrease
             (Currency.Type.Gold, Storages.Currency.GetAmount(Currency.Type.Gold));
    }
}
