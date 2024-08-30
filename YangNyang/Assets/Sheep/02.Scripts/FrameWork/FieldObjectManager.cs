using System.Collections;
using System.Linq;
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

    // ���� �� ��Ȳ�� ���� �ٲ�� �� �������� ����.
    private float _variableSheepSpawnInterval;
    // �� ���� ���̺��� ���÷� �������� �ʱ� ���ؼ� ĳ���صд�.
    private int[] _sheepSpawnWeightsCache;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        _preloadContainer.Preload();
        InitializeSheepSpawnCoroutine(true);
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

    /// <summary>
    /// ������ �ð����� ���� �����Ѵ�.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnSheepCoroutine()
    {
        var _wfs = new WaitForSeconds(_variableSheepSpawnInterval);

        while (true)
        {
            SpawnSheep();
            yield return _wfs;
        }
    }

    /// <summary>
    /// ���̺��� ��������, �� �� ����ġ�� ����� ������ ���� �����Ѵ�.
    /// </summary>
    private void SpawnSheep()
    {
        var level = GameDataManager.Instance.Storages.User.ResearchLevel;
        var sheepUnit = GameDataManager.Instance.Tables.SheepSpawnRateTable.GetUnit(level);

        if (sheepUnit != null && sheepUnit.sheepList != null && sheepUnit.sheepList.Length > 0)
        {
            // ĳ���� �ȵ��� ������ sheepList�� �迭�� ��ȯ�Ѵ�.
            if (_sheepSpawnWeightsCache == null || _sheepSpawnWeightsCache.Length == 0)
            {
                _sheepSpawnWeightsCache = sheepUnit.sheepList.Select(sheep => sheep.weight).ToArray();
            }
            int[] weights = _sheepSpawnWeightsCache;
            int randomIndex = GetRandomSheepByWeight(weights);
            var selectedSheep = sheepUnit.sheepList[randomIndex];

            var unit = GameDataManager.Instance.Tables.Sheep.GetUnit(selectedSheep.id);
            Debug.Log($"ID: {unit.id} / type : {unit.type}");            
            var go = (ObjectPool.Instance.Pop($"{unit.type}Sheep")).GetComponent<StandardSheep>();
            go.Spawn(Places.GetPlacePosition(PlaceData.Type.SheepSpawn));
        }

    }
    /// <summary>
    /// ����ġ�� ����ؼ� ������ ������ �̴´�.
    /// </summary>
    /// <param name="weights"></param>
    /// <returns></returns>
    private int GetRandomSheepByWeight(int[] weights)
    {
        if (weights == null && weights.Length <= 0)
            return -1;

        int totalActionWeight = 0;
        for (int i = 0; i < weights.Length; i++)
            totalActionWeight += weights[i];

        int randomNum = 0;
        int randomValue = UnityEngine.Random.Range(0, totalActionWeight);
        for (int i = 0; i < weights.Length; i++)
        {
            if (randomValue < weights[i])
            {
                randomNum = i;
                break;
            }
            randomValue = randomValue - weights[i];
        }
        return randomNum;
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