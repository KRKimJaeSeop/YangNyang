using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static StandardSheep;

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

    [SerializeField]
    private Dictionary<int, BaseFieldObject> _managedObjects = new Dictionary<int, BaseFieldObject>();
    private Coroutine _sheepSpawnCoroutine;
    private Coroutine _buffSheepSpawnIntervalCoroutine;

    // ���� �� ��Ȳ�� ���� �ٲ�� �� �������� ����.
    private float _variableSheepSpawnInterval;
    // �� �������̺� ĳ��.
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
    /// �� ���� �ð� ���ݿ� ���� ��ȭ�� �ش�.
    /// </summary>
    /// <param name="increasePercent">�ۼ�Ʈ�� ���´�.(ex:30 , 50 , 90)</param>
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
    /// �� ������ �����Ѵ�.
    /// </summary>
    /// <param name="isInitValue">���� ���� �ʱ�ȭ ����</param>
    public void StartSheepSpawn(bool isInitValue)
    {
        if (_sheepSpawnCoroutine != null)
            StopCoroutine(_sheepSpawnCoroutine);
        if (isInitValue)
            _variableSheepSpawnInterval = GameDataManager.Instance.Tables.SheepSpawnRateTable.spawnInterval;

        _sheepSpawnCoroutine = StartCoroutine(SpawnSheepCoroutine());
    }

    /// <summary>
    /// �� ������ �ߴ��Ѵ�.
    /// </summary>
    public void StopSheepSpawn()
    {
        if (_sheepSpawnCoroutine != null)
            StopCoroutine(_sheepSpawnCoroutine);
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
    /// ������ ID�� ���� �����Ѵ�.
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