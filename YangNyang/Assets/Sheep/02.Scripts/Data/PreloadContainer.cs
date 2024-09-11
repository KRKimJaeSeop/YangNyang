using System;
using System.Collections.Generic;
using UnityEngine;
using static AddressableManager;

public class PreloadContainer : MonoBehaviour
{
    [Serializable]
    public class AssetData
    {
        [Tooltip("오브젝트 풀 이름")]
        public string name;
        [Tooltip("프리팹")]
        public GameObject prefab;
        [Tooltip("미리 로딩해 놓을 갯수")]
        public int preloadNum;
    }
    [Serializable]
    public class RemoteAssetData
    {
        [Tooltip("오브젝트 풀 이름")]
        public string name;
        [Tooltip("로드할 어드레서블 리모트 코드")]
        public RemoteAssetCode code;
        [Tooltip("미리 로딩해 놓을 갯수")]
        public int preloadNum;
    }


    [Header("[Settings]")]
    [SerializeField, Tooltip("미리 로딩해 놓을 오브젝트 리스트")]
    public List<AssetData> assetList;

    [SerializeField, Tooltip("미리 로딩해 놓을 어드레서블 오브젝트 리스트")]
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
