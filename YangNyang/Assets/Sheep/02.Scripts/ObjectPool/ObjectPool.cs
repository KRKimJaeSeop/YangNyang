using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    protected Transform _transform;
    protected Dictionary<string, PooledObject> _dicPool = new Dictionary<string, PooledObject>();

    private void Awake()
    {
        // ---- singleton
        if (_instance != null)
        {
            // instance �� �ִµ� �̹� �ٸ� ������Ʈ���� �ε� �Ǿ� �ִٸ� destroy.
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        // ---- singleton

        _transform = this.transform;
        Clear();
    }

    /// <summary>
    /// ��� child object�� dictionary �� �����Ѵ�.
    /// </summary>
    public void Clear()
    {
        // �켱 PooldObject Release.
        foreach (KeyValuePair<string, PooledObject> item in _dicPool)
        {
            item.Value.Release();
        }

        // Remove all child objects
        foreach (Transform trChild in _transform)
        {
            Destroy(trChild.gameObject);
        }

        _dicPool.Clear();
    }

    /// <summary>
    /// �ش� name�� pool�� �����Ѵ�.
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public bool Remove(string poolName)
    {
        var pool = GetPool(poolName);
        if (pool != null)
        {
            _dicPool.Remove(poolName);

            pool.Release();
            Destroy(pool.gameObject);

            return true;
        }
        return false;
    }

    /// <summary>
    /// poolName ���� �̸��� ���� ������Ʈ Ǯ�� �˻��Ѵ�.
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns>PooedObject. null: error</returns>
    private PooledObject GetPool(string poolName)
    {
        if (!_dicPool.ContainsKey(poolName)) // pool �� ���ٸ�
            return null;

        return _dicPool[poolName];
    }
    public bool IsLoadedAll()
    {
        // �켱 PooldObject Release.
        foreach (KeyValuePair<string, PooledObject> item in _dicPool)
        {
            if (item.Value.IsLoaded == false)
                return false;
        }

        return true;
    }
    private void CreatePool(string poolName, GameObject prefab, int preloadNumber)
    {
        // pool ����
        GameObject goPool = new GameObject(poolName);
        goPool.transform.SetParent(_transform, false);
        var pool = goPool.AddComponent<PooledObject>();
        _dicPool.Add(poolName, pool);
        pool.Initialize(poolName, prefab, preloadNumber);
    }


    /// <summary>
    /// pool�� �����ϰ� ���ϴ� ������ŭ GameObject�� ����� ���´�.
    /// �̹� pool�� �����Ǿ� �ִٸ� isAddition �÷��׷� �߰� ������ �� �ִ�.
    /// </summary>
    /// <param name="poolName">Object�� ã�� �� ���̴� ������ ��</param>
    /// <param name="prefab">Prefab</param>
    /// <param name="preloadNumber">�̸� ������ ���� GameObject ����</param>
    /// <param name="isAdditional">true: �̹� pool�� �ִ��� �߰��� ����� ���´�.</param>
    /// <returns></returns>
    public void LoadPoolItem(string poolName, GameObject prefab, int preloadNumber, bool isAdditional = false)
    {
        if (GetPool(poolName) == null) // pool �� ���ٸ�
        {
            // pool ����
            CreatePool(poolName, prefab, preloadNumber);
        }
        else // pool �� �ִٸ�
        {
            if (isAdditional == true)
            {
                // pool �� preloadCount ��ŭ �߰� object ����
                _dicPool[poolName].AddItems(preloadNumber);
            }
        }
    }



    /// <summary>
    /// ����� ��ü�� ObjectPool�� ��ȯ�Ѵ�.
    /// </summary>
    /// <param name="poolName">��ȯ�� ��ü�� pool ������Ʈ �̸�</param>
    /// <param name="item">��ȯ�� ��ü</param>
    /// <param name="setParent">true: pooledObject Ŭ������ �θ�� �����Ѵ�.</param>
    /// <returns></returns>
    public bool Push(string poolName, GameObject item, bool setParent = false)
    {
        PooledObject pool = GetPool(poolName);
        if (pool == null)
        {
            Debug.LogError($"{GetType()}::{nameof(Pop)} - pool ����. poolName={poolName}");
            return false;
        }

       // Debug.Log($"Push [{item.name}]");
        pool.Push(item, setParent);
        return true;
    }

    /// <summary>
    /// �ʿ��� ��ü�� ObjectPool�� ��û�Ѵ�.
    /// </summary>
    /// <param name="poolName">��û�� ��ü�� pool������Ʈ �̸�</param>
    /// <returns></returns>
    public GameObject Pop(string poolName)
    {
        PooledObject pool = GetPool(poolName);
        if (pool == null || pool.IsLoaded == false)
        {
            Debug.LogError($"{GetType()}::{nameof(Pop)} - pool ���ų�, �غ���� �ʾ���. poolName={poolName}");
            return null;
        }

      //  Debug.Log($"Pop [{poolName}]");
        return pool.Pop();
    }
}
