using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StandardSheep;

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

    [SerializeField]
    private Dictionary<int, BaseFieldObject> _managedObjects = new Dictionary<int, BaseFieldObject>();
    private Coroutine _sheepSpawnCoroutine;
    private Coroutine _buffSheepSpawnIntervalCoroutine;

    // 버프 등 상황에 따라 바뀌는 양 스폰간격 변수.
    private float _variableSheepSpawnInterval;
    // 양 스폰테이블 캐시.
    private SheepSpawnCache _sheepSpawnCache;

    private void OnEnable()
    {
        UserStorage.OnUpdateLevel += SetSheepSpawnTableCache;
        DialogManager.OnDialogEnter += DialogManager_OnDialogEnter;
    }



    private void OnDisable()
    {
        UserStorage.OnUpdateLevel -= SetSheepSpawnTableCache;
        DialogManager.OnDialogEnter -= DialogManager_OnDialogEnter;
    }

    public void Initialize()
    {
        _preloadContainer.Preload();
        StartSheepSpawn(true);
    }

    #region Manage


    public T GetFieldObject<T>(int instanceID) where T : BaseFieldObject
    {
        if (_managedObjects.TryGetValue(instanceID, out BaseFieldObject go))
        {
            if (go is T typedObject)
            {
                return typedObject;
            }
            else
            {
                Debug.LogError($"Object with ID [{instanceID}] is not of type {typeof(T)}");
                return null;
            }
        }
        else
        {
            Debug.LogError($"Don't Manage [{instanceID}]");
            return null;
        }
    }

    private void DespawnAll()
    {
        foreach (var item in _managedObjects.ToList())
        {
            item.Value.Despawn();
        }
        Debug.Log("DespawnAll");
    }

    public void DespawnByInstanceID(int instanceID)
    {
        if (_managedObjects.TryGetValue(instanceID, out BaseFieldObject go))
        {
            go.Despawn();
            _managedObjects.Remove(instanceID);
        }
        else
        {
            Debug.LogError($"Don't Manage [{instanceID}]");
        }
    }
    private void DialogManager_OnDialogEnter(bool isStart)
    {
        DespawnAll();
        if (isStart)
        {
            StopSheepSpawn();
        }
        else
        {
            SpawnPlayer();
            StartSheepSpawn(false);
        }
    }
    #endregion

    #region SpawnPlayer

    public BaseFieldObject SpawnPlayer(Place.Type spawnPlace = Place.Type.PlayerSpawn)
    {
        var go = (ObjectPool.Instance.Pop("Player")).GetComponent<PlayerCharacter>();
        _managedObjects.TryAdd(go.InstanceID, go);
        go.Spawn(_placeDataContainer.GetPlacePosition(spawnPlace), () =>
        {
            _managedObjects.Remove(go.InstanceID);
        });
        return go;
    }
    #endregion

    #region SheepSpawnIntervalBuff

    /// <summary>
    /// 양 스폰 시간 간격에 대해 변화를 준다.
    /// </summary>
    /// <param name="increasePercent">퍼센트로 적는다.(ex:30 , 50 , 90)</param>
    /// <param name="buffSecond"></param>
    public void SheepSpawnBuff(float increasePercent, float buffSecond)
    {
        if (_buffSheepSpawnIntervalCoroutine != null)
        {
            StopCoroutine(_buffSheepSpawnIntervalCoroutine);
        }
        _buffSheepSpawnIntervalCoroutine =
            StartCoroutine(BuffSheepSpawnIntervalCoroutine(increasePercent, buffSecond));

        StartSheepSpawn(false);
    }

    IEnumerator BuffSheepSpawnIntervalCoroutine(float increasePercent, float buffSecond)
    {
        _variableSheepSpawnInterval =
            GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval -
            (GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval * (increasePercent / 100));

        yield return new WaitForSeconds(buffSecond);

        StartSheepSpawn(true);
    }
    #endregion

    #region SheepSpawn

    /// <summary>
    /// 양 스폰을 시작한다.
    /// </summary>
    /// <param name="isInitValue">스폰 간격 초기화 여부</param>
    public void StartSheepSpawn(bool isInitValue)
    {
        if (_sheepSpawnCoroutine != null)
            StopCoroutine(_sheepSpawnCoroutine);
        if (isInitValue)
            _variableSheepSpawnInterval = GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval;

        _sheepSpawnCoroutine = StartCoroutine(SpawnSheepCoroutine());
    }

    /// <summary>
    /// 양 스폰을 중단한다.
    /// </summary>
    public void StopSheepSpawn()
    {
        if (_sheepSpawnCoroutine != null)
            StopCoroutine(_sheepSpawnCoroutine);
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
    public BaseFieldObject SpawnSheep(Place.Type spawnPlace = Place.Type.SheepSpawn, SheepState initState = SheepState.Move)
    {
        if (_sheepSpawnCache.tbUnit != null)
        {
            int randomIndex =
                GameDataManager.Instance.Tables.Sheep.GetRandomSheepByWeight(_sheepSpawnCache.array, _sheepSpawnCache.totalWeight);
            var selectedSheep = _sheepSpawnCache.tbUnit.sheepList[randomIndex];
            var unit = GameDataManager.Instance.Tables.Sheep.GetUnit(selectedSheep.id);


            var go = (ObjectPool.Instance.Pop($"{unit.Type}Sheep")).GetComponent<StandardSheep>();
            _managedObjects.TryAdd(go.InstanceID, go);
            go.Spawn(unit.id, Places.GetPlacePosition(spawnPlace), initState, () =>
            {
                _managedObjects.Remove(go.InstanceID);
            });
            return go;
        }
        return null;
    }
    /// <summary>
    /// 지정된 ID의 양을 스폰한다.
    /// </summary>
    public BaseFieldObject SpawnSheep(int id, Place.Type spawnPlace = Place.Type.SheepSpawn, SheepState initState = SheepState.Move)
    {
        if (_sheepSpawnCache.tbUnit != null)
        {
            var unit = GameDataManager.Instance.Tables.Sheep.GetUnit(id);
            var go = (ObjectPool.Instance.Pop($"{unit.Type}Sheep")).GetComponent<StandardSheep>();
            _managedObjects.TryAdd(go.InstanceID, go);
            go.Spawn(unit.id, Places.GetPlacePosition(spawnPlace), initState, () =>
            {
                _managedObjects.Remove(go.InstanceID);
            });
            return go;
        }
        return null;
    }
    #endregion


    #region Wool

    public void SpawnWools(Vector2 startPosition, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var go = (ObjectPool.Instance.Pop("Wool")).GetComponent<Wool>();
            _managedObjects.TryAdd(go.InstanceID, go);
            go.Spawn(startPosition, () =>
            {
                _managedObjects.Remove(go.InstanceID);
            });
        }
    }

    public BaseFieldObject SpawnWool(Vector2 startPosition)
    {
        var go = (ObjectPool.Instance.Pop("Wool")).GetComponent<Wool>();
        _managedObjects.TryAdd(go.InstanceID, go);
        go.Spawn(startPosition, () =>
        {
            _managedObjects.Remove(go.InstanceID);
        });
        return go;
    }

    #endregion
}