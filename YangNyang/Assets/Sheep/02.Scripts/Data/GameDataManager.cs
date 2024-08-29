using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    [SerializeField]
    private TableContainer _tables;
    public TableContainer Tables { get { return _tables; } }



    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

    }
}
