using System.Collections;
using UnityEngine;


public class FieldObjectManager : Singleton<FieldObjectManager>
{
    [SerializeField]
    private PlaceDataContainer _placeDataContainer;
    public PlaceDataContainer Places { get { return _placeDataContainer; } }

    [SerializeField]
    private PreloadContainer _preloadContainer;
    public PreloadContainer PreloadContainer { get { return _preloadContainer; } }

    private Coroutine _sheepSpawnCoroutine;
    private Coroutine _buffSheepSpawnIntervalCorouinte;

    private float _variableSheepSpawnInterval;


    private void Awake()
    {
        _preloadContainer.Preload();
        InitializeSheepSpawnCoroutine(true);
    }

    #region Spawn

    private void InitializeSheepSpawnCoroutine(bool isInitValue)
    {
        if (_sheepSpawnCoroutine != null)
            StopCoroutine(_sheepSpawnCoroutine);
        if (isInitValue)
            _variableSheepSpawnInterval = GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval;

        _sheepSpawnCoroutine = StartCoroutine(SpawnSheepCoroutine());
    }

    /// <summary>
    /// 양 스폰 시간 간격에 대해 변화를 준다.
    /// </summary>
    /// <param name="increasePercent">퍼센트로 적는다.(ex:30 , 50 , 90)</param>
    /// <param name="buffSecond"></param>
    public void SheepSpawnBuff(float increasePercent, float buffSecond)
    {
        if (_buffSheepSpawnIntervalCorouinte != null)
        {
            StopCoroutine(_buffSheepSpawnIntervalCorouinte);
        }
        _buffSheepSpawnIntervalCorouinte =
            StartCoroutine(BuffSheepSpawnIntervalCorouinte(increasePercent, buffSecond));

        InitializeSheepSpawnCoroutine(false);
    }

    IEnumerator BuffSheepSpawnIntervalCorouinte(float increasePercent, float buffSecond)
    {
        _variableSheepSpawnInterval =
            GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval -
            (GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval * (increasePercent / 100));

        yield return new WaitForSeconds(buffSecond);

        InitializeSheepSpawnCoroutine(true);
    }


    IEnumerator SpawnSheepCoroutine()
    {
        var _wfs = new WaitForSeconds(_variableSheepSpawnInterval);

        while (true)
        {
            SpawnSheep();
            yield return _wfs;
        }
    }

    private void SpawnSheep()
    {
        var level = GameDataManager.Instance.Storages.User.ResearchLevel;
        var tableUnit = GameDataManager.Instance.Tables.SheepSpawnRateTable.GetUnit(level);
        Debug.Log($"{_variableSheepSpawnInterval}초 마다 {tableUnit.name} 중에서 골라서 생성"); ;

        //var go = (ObjectPool.Instance.Pop(spawnObject)).GetComponent<StandardSheep>();
        //go.Spawn(Places.GetPlacePosition(PlaceData.Type.SheepSpawn));
    }

    public void SpawnWool(Vector2 startPosition, int amount)
    {
        for (int i = 0; i < amount; i++)
        {

            float randomX = Random.Range(Places.GetPlacePosition
                (PlaceData.Type.WoolDropZone_BottomLeftCorner).x,
                Places.GetPlacePosition(PlaceData.Type.WoolDropZone_TopRightCorner).x);

            float randomY = Random.Range(Places.GetPlacePosition
             (PlaceData.Type.WoolDropZone_BottomLeftCorner).y,
             Places.GetPlacePosition(PlaceData.Type.WoolDropZone_TopRightCorner).y);

            var go = (ObjectPool.Instance.Pop("Wool")).GetComponent<Wool>();
            go.EnableGameObject();
            go.transform.position = startPosition;
            go.MoveToPosition(new Vector2(randomX, randomY), 6);
        }

    }
    #endregion
}