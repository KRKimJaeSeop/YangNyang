using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    [SerializeField]
    private TableContainer _tables;
    public TableContainer Tables { get { return _tables; } }

    [SerializeField]
    private StorageContainer _storage;
    public StorageContainer Storages { get { return _storage; } }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

    }
}
