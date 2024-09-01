using System.Collections;
using System.Linq;
using UnityEngine;


public class FieldObjectManager : Singleton<FieldObjectManager>
{
    // �� ���� ���̺��� ���÷� �������� �ʱ� ���ؼ� ĳ���صд�.
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

    // ���� �� ��Ȳ�� ���� �ٲ�� �� �������� ����.
    private float _variableSheepSpawnInterval;
    // �� �������̺� ĳ��.
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
    /// �� ���� �ð� ���ݿ� ���� ��ȭ�� �ش�.
    /// </summary>
    /// <param name="increasePercent">�ۼ�Ʈ�� ���´�.(ex:30 , 50 , 90)</param>
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
    /// �� ���� �ڷ�ƾ�� �ʱ�ȭ��Ű�� �ٽ� �����Ѵ�.
    /// </summary>
    /// <param name="isInitValue">���� ���� �ʱ�ȭ ����</param>
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
        // �罺�� Ȯ�� tbUnit�� �޴´�.
        _sheepSpawnCache.tbUnit = GameDataManager.Instance.Tables.SheepSpawnRateTable.GetUnit(currentLevel);
        // ���� tbUnit���� �迭�� �����.
        _sheepSpawnCache.array = _sheepSpawnCache.tbUnit.sheepList.Select(sheep => sheep.weight).ToArray();
        // ����ġ�� ������ ����Ͽ� ĳ���Ѵ�.
        _sheepSpawnCache.totalWeight = _sheepSpawnCache.array.Sum();
        _sheepSpawnCache.id = currentLevel;

    }
    /// <summary>
    /// ������ �ð����� ���� �����Ѵ�.
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
    /// ����ġ�� ����� ������ ���� �����Ѵ�.
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
    /// ����ġ�� ����ؼ� ������ ������ �̴´�.
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