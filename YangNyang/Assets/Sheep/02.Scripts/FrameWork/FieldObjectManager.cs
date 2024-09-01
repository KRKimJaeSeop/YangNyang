using System.Collections;
using System.Linq;
using UnityEngine;


public class FieldObjectManager : Singleton<FieldObjectManager>
{
    // 양 스폰 테이블을 수시로 가져오지 않기 위해서 캐싱해둔다.
    public struct SheepSpawnCache
    {
        public int id;
        public int[] array;
        public int totalWeight;
        public SheepSpawnRateTableUnit tbUnit;
    }

    [SerializeField]
    private PlaceDataContainer _placeDataContainer;
    public PlaceDataContainer Places { get { return _placeDataContainer; } }
    [SerializeField]
    private PreloadContainer _preloadContainer;
    public PreloadContainer PreloadContainer { get { return _preloadContainer; } }

    private Coroutine _sheepSpawnCoroutine;
    private Coroutine _buffSheepSpawnIntervalCorouinte;

    // 버프 등 상황에 따라 바뀌는 양 스폰간격 변수.
    private float _variableSheepSpawnInterval;
    // 양 스폰테이블 캐시.
    private SheepSpawnCache _sheepSpawnCache;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        _preloadContainer.Preload();
        InitializeSheepSpawnCoroutine(true);
    }
    private void OnEnable()
    {
        UserStorage.OnUpdateLevel += SetSheepSpawnTableCache;
    }
    private void OnDisable()
    {
        UserStorage.OnUpdateLevel -= SetSheepSpawnTableCache;
    }


    #region SheepSpawnIntervalBuff
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

    #endregion

    #region SheepSpawn
    /// <summary>
    /// 양 스폰 코루틴을 초기화시키고 다시 시작한다.
    /// </summary>
    /// <param name="isInitValue">스폰 간격 초기화 여부</param>
    private void InitializeSheepSpawnCoroutine(bool isInitValue)
    {
        if (_sheepSpawnCoroutine != null)
            StopCoroutine(_sheepSpawnCoroutine);
        if (isInitValue)
            _variableSheepSpawnInterval = GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval;

        _sheepSpawnCoroutine = StartCoroutine(SpawnSheepCoroutine());
    }
    private void SetSheepSpawnTableCache(int currentLevel)
    {
        // 양스폰 확률 tbUnit을 받는다.
        _sheepSpawnCache.tbUnit = GameDataManager.Instance.Tables.SheepSpawnRateTable.GetUnit(currentLevel);
        // 받은 tbUnit으로 배열을 만든다.
        _sheepSpawnCache.array = _sheepSpawnCache.tbUnit.sheepList.Select(sheep => sheep.weight).ToArray();
        // 가중치의 총합을 계산하여 캐싱한다.
        _sheepSpawnCache.totalWeight = _sheepSpawnCache.array.Sum();
        _sheepSpawnCache.id = currentLevel;

    }
    /// <summary>
    /// 정해진 시간마다 양을 스폰한다.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnSheepCoroutine()
    {
        var _wfs = new WaitForSeconds(_variableSheepSpawnInterval);
        SetSheepSpawnTableCache(GameDataManager.Instance.Storages.User.ResearchLevel);
        while (true)
        {
            SpawnSheep();
            yield return _wfs;
        }
    }

    /// <summary>
    /// 가중치를 사용해 랜덤한 양을 스폰한다.
    /// </summary>
    private void SpawnSheep()
    {
        if (_sheepSpawnCache.tbUnit != null)
        {
            int randomIndex = GetRandomSheepByWeight(_sheepSpawnCache.array, _sheepSpawnCache.totalWeight);
            var selectedSheep = _sheepSpawnCache.tbUnit.sheepList[randomIndex];

            var unit = GameDataManager.Instance.Tables.Sheep.GetUnit(selectedSheep.id);
            var go = (ObjectPool.Instance.Pop($"{unit.Type}Sheep")).GetComponent<StandardSheep>();
            go.Spawn(unit.id, Places.GetPlacePosition(PlaceData.Type.SheepSpawn));
        }

    }
    /// <summary>
    /// 가중치를 사용해서 랜덤한 정수를 뽑는다.
    /// </summary>
    /// <param name="weights"></param>
    /// <returns></returns>
    private int GetRandomSheepByWeight(int[] weights, int totalWeight)
    {
        if (weights == null || weights.Length <= 0)
            return -1;

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i])
            {
                return i;
            }
            randomValue -= weights[i];
        }
        return -1;
    }
    #endregion

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
}