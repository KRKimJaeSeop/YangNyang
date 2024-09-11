using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static AddressableManager;

public class PooledObject : MonoBehaviour
{
    protected Transform _transform;

    // 객체를 검색할 때 사용할 이름
    protected string _name = string.Empty; 
    // 오브젝트 풀에 저장할 프리팹
    protected GameObject _prefab = null; 
    // 생성된 객체의 수
    [SerializeField, ReadOnly] protected int _instantiatedNum = 0; 
    // 생성한 객체들을 저장할 리스트.
    [SerializeField, ReadOnly] protected List<GameObject> _listObject = new List<GameObject>();

    public bool IsLoaded { get; protected set; }

    protected void Awake()
    {
        _transform = this.transform;
        _listObject.Clear();
    }
    public virtual void Release()
    {
        _listObject.Clear();
        IsLoaded = false;
    }
    public void Initialize(string poolName, GameObject prefab, int preloadNumber)
    {
        _name = poolName;
        _prefab = prefab;
        _instantiatedNum = 0;
        IsLoaded = true;

        AddItems(preloadNumber);
    }
    public void Initialze(string poolName, RemoteAssetCode code, int preloadNumber)
    {
        _name = poolName;
        _prefab = AddressableManager.Instance.GetAsset<GameObject>(code);
        _instantiatedNum = 0;
        IsLoaded = true;

        AddItems(preloadNumber);
    }

    public void AddItems(int preloadNumber)
    {
        for (int i = 0; i < preloadNumber; i++)
        {
            _listObject.Add(CreateItem());
        }
    }

    protected GameObject CreateItem()
    {
        GameObject go = GameObject.Instantiate(_prefab) as GameObject;
        go.name = _name;
        go.transform.SetParent(_transform, false);
        go.transform.localPosition = Vector3.zero;
        // 생성된 아이템 내부에 따로 로딩할 것이 있다면 로드 할 수 있도록 한다.
        go.SendMessage("LoadInPool", SendMessageOptions.DontRequireReceiver);
        go.SetActive(false);
        _instantiatedNum++;

        return go;
    }

    public void Push(GameObject item, bool setParent)
    {
        if (setParent == true)
        {
            item.transform.SetParent(_transform, false);
        }
        item.transform.localPosition = Vector3.zero;
        item.SetActive(false);
        _listObject.Add(item);
    }

    public GameObject Pop()
    {
        if (_listObject.Count == 0)
        {
            //Debug.LogError($"{GetType()}::{nameof(PopFromPool)} - CreateItem. name={_name}");
            _listObject.Add(CreateItem());
        }

        GameObject item = _listObject[0];
        _listObject.RemoveAt(0);

        return item;
    }
 

}
