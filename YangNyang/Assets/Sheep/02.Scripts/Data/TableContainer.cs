using UnityEngine;

public class TableContainer : MonoBehaviour
{
    [Header("Tables")]
    [SerializeField] private CurrencyTable _currency;
    [SerializeField] private SheepTable _sheep;
    [SerializeField] private SheepSpawnRateTable _sheepSpawnRate;

    public CurrencyTable Currency { get { return _currency; } }
    public SheepTable Sheep { get { return _sheep; } }

    public SheepSpawnRateTable SheepSpawnRateTable { get { return _sheepSpawnRate; } }


    public bool Initialize()
    {
        if (_currency.Initialize()
             && _sheep.Initialize()
             && _sheepSpawnRate.Initialize()
             )
            return true;

        return false;
    }

}
