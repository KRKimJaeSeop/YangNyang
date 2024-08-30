using System;
using System.Collections.Generic;
using UnityEngine;

public class PreloadContainer : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        [Tooltip("������Ʈ Ǯ �̸�")]
        public string name;
        [Tooltip("������")]
        public GameObject prefab;
        [Tooltip("�̸� �ε��� ���� ����")]
        public int preloadNum;
    }

    [Header("[Settings]")]
    [SerializeField, Tooltip("�̸� �ε��� ���� ������Ʈ ����Ʈ")]
    public List<Data> list;

  
    public void Preload()
    {
        foreach (Data data in list)
        {
            ObjectPool.Instance.LoadPoolItem(data.name, data.prefab, data.preloadNum);
        }

    }

}
