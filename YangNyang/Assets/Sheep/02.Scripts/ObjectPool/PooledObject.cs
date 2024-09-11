using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static AddressableManager;

public class PooledObject : MonoBehaviour
{
    protected Transform _transform;

    // ��ü�� �˻��� �� ����� �̸�
    protected string _name = string.Empty; 
    // ������Ʈ Ǯ�� ������ ������
    protected GameObject _prefab = null; 
    // ������ ��ü�� ��
    [SerializeField, ReadOnly] protected int _instantiatedNum = 0; 
    // ������ ��ü���� ������ ����Ʈ.
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
        // ������ ������ ���ο� ���� �ε��� ���� �ִٸ� �ε� �� �� �ֵ��� �Ѵ�.
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
