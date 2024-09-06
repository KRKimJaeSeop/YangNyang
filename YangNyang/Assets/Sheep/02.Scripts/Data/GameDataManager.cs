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

        // table �ʱ�ȭ (storage �ε��ϱ� ���� ���� �ʱ�ȭ �Ǿ� �־�� �Ѵ�.)
        if (!_tables.Initialize())
            return false;

        _storage.Initialize();

        IsInitialized = true;
        return true;
    }
}
