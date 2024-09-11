using System;
using System.Collections.Generic;
using UnityEngine;
using static AddressableManager;

public class PreloadContainer : MonoBehaviour
{
    [Serializable]
    public class AssetData
    {
        [Tooltip("������Ʈ Ǯ �̸�")]
        public string name;
        [Tooltip("������")]
        public GameObject prefab;
        [Tooltip("�̸� �ε��� ���� ����")]
        public int preloadNum;
    }
    [Serializable]
    public class RemoteAssetData
    {
        [Tooltip("������Ʈ Ǯ �̸�")]
        public string name;
        [Tooltip("�ε��� ��巹���� ����Ʈ �ڵ�")]
        public RemoteAssetCode code;
        [Tooltip("�̸� �ε��� ���� ����")]
        public int preloadNum;
    }


    [Header("[Settings]")]
    [SerializeField, Tooltip("�̸� �ε��� ���� ������Ʈ ����Ʈ")]
    public List<AssetData> assetList;

    [SerializeField, Tooltip("�̸� �ε��� ���� ��巹���� ������Ʈ ����Ʈ")]
    public List<RemoteAssetData> remoteAssetList;

    public void Preload()
    {
        foreach (AssetData data in assetList)
        {
            ObjectPool.Instance.LoadPoolItem(data.name, data.prefab, data.preloadNum);
        }
        foreach (RemoteAssetData data in remoteAssetList)
        {
            ObjectPool.Instance.LoadPoolItem(data.name, data.code, data.preloadNum);
        }

    }

}
