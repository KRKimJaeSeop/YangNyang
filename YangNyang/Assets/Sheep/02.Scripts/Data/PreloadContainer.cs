using System;
using System.Collections.Generic;
using UnityEngine;

public class PreloadContainer : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        [Tooltip("오브젝트 풀 이름")]
        public string name;
        [Tooltip("프리팹")]
        public GameObject prefab;
        [Tooltip("미리 로딩해 놓을 갯수")]
        public int preloadNum;
    }

    [Header("[Settings]")]
    [SerializeField, Tooltip("미리 로딩해 놓을 오브젝트 리스트")]
    public List<Data> list;

  
    public void Preload()
    {
        foreach (Data data in list)
        {
            ObjectPool.Instance.LoadPoolItem(data.name, data.prefab, data.preloadNum);
        }

    }

}
