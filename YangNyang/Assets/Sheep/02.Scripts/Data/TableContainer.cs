using UnityEngine;

public class TableContainer : MonoBehaviour
{
    [Header("Tables")]
    [SerializeField] private CurrencyTable _currency;
    [SerializeField] private SheepTable _sheep;
    [SerializeField] private SheepSpawnRateTable _sheepSpawnRate;
    [SerializeField] private DayStatusTable _datyStatus;
    [SerializeField] private ResearchTable _research;

    public CurrencyTable Currency { get { return _currency; } }
    public SheepTable Sheep { get { return _sheep; } }
    public SheepSpawnRateTable SheepSpawnRateTable { get { return _sheepSpawnRate; } }
    public DayStatusTable DatyStatus { get { return _datyStatus; } }
    public ResearchTable Research { get { return _research; } }


    public bool Initialize()
    {
        if (_currency.Initialize()
             && _sheep.Initialize()
             && _sheepSpawnRate.Initialize()
             && _datyStatus.Initialize()
             && _research.Initialize()
             )
            return true;

        return false;
    }

}
