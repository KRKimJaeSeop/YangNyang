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
            // instance 가 있는데 이미 다른 오브젝트에서 로드 되어 있다면 destroy.
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        // ---- singleton

        _transform = this.transform;
        Clear();
    }

    /// <summary>
    /// 모든 child object와 dictionary 를 삭제한다.
    /// </summary>
    public void Clear()
    {
        // 우선 PooldObject Release.
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
    /// 해당 name의 pool을 삭제한다.
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
    /// poolName 같은 이름을 가진 오브젝트 풀을 검색한다.
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns>PooedObject. null: error</returns>
    private PooledObject GetPool(string poolName)
    {
        if (!_dicPool.ContainsKey(poolName)) // pool 이 없다면
            return null;

        return _dicPool[poolName];
    }
    public bool IsLoadedAll()
    {
        // 우선 PooldObject Release.
        foreach (KeyValuePair<string, PooledObject> item in _dicPool)
        {
            if (item.Value.IsLoaded == false)
                return false;
        }

        return true;
    }
    private void CreatePool(string poolName, GameObject prefab, int preloadNumber)
    {
        // pool 생성
        GameObject goPool = new GameObject(poolName);
        goPool.transform.SetParent(_transform, false);
        var pool = goPool.AddComponent<PooledObject>();
        _dicPool.Add(poolName, pool);
        pool.Initialize(poolName, prefab, preloadNumber);
    }


    /// <summary>
    /// pool을 생성하고 원하는 개수만큼 GameObject를 만들어 놓는다.
    /// 이미 pool이 생성되어 있다면 isAddition 플래그로 추가 생성할 수 있다.
    /// </summary>
    /// <param name="poolName">Object를 찾을 때 쓰이는 유일한 값</param>
    /// <param name="prefab">Prefab</param>
    /// <param name="preloadNumber">미리 생성해 놓을 GameObject 개수</param>
    /// <param name="isAdditional">true: 이미 pool이 있더라도 추가로 만들어 놓는다.</param>
    /// <returns></returns>
    public void LoadPoolItem(string poolName, GameObject prefab, int preloadNumber, bool isAdditional = false)
    {
        if (GetPool(poolName) == null) // pool 이 없다면
        {
            // pool 생성
            CreatePool(poolName, prefab, preloadNumber);
        }
        else // pool 이 있다면
        {
            if (isAdditional == true)
            {
                // pool 에 preloadCount 만큼 추가 object 생성
                _dicPool[poolName].AddItems(preloadNumber);
            }
        }
    }



    /// <summary>
    /// 사용한 객체를 ObjectPool에 반환한다.
    /// </summary>
    /// <param name="poolName">반환할 객체의 pool 오브젝트 이름</param>
    /// <param name="item">반환할 객체</param>
    /// <param name="setParent">true: pooledObject 클래스를 부모로 지정한다.</param>
    /// <returns></returns>
    public bool Push(string poolName, GameObject item, bool setParent = false)
    {
        PooledObject pool = GetPool(poolName);
        if (pool == null)
        {
            Debug.LogError($"{GetType()}::{nameof(Pop)} - pool 없음. poolName={poolName}");
            return false;
        }

       // Debug.Log($"Push [{item.name}]");
        pool.Push(item, setParent);
        return true;
    }

    /// <summary>
    /// 필요한 객체를 ObjectPool에 요청한다.
    /// </summary>
    /// <param name="poolName">요청할 객체의 pool오브젝트 이름</param>
    /// <returns></returns>
    public GameObject Pop(string poolName)
    {
        PooledObject pool = GetPool(poolName);
        if (pool == null || pool.IsLoaded == false)
        {
            Debug.LogError($"{GetType()}::{nameof(Pop)} - pool 없거나, 준비되지 않았음. poolName={poolName}");
            return null;
        }

      //  Debug.Log($"Pop [{poolName}]");
        return pool.Pop();
    }
}
