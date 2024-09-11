using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : Singleton<AddressableManager>
{
    public List<AssetReferenceT<UnityEngine.Object>> assetReferences;
    public List<string> assetNamesString;
    public List<string> assetNamesString2;
    private Dictionary<string, UnityEngine.Object> loadedAssets = new Dictionary<string, UnityEngine.Object>();


  
    public async Task LoadAllAssetsAsync()
    {
        foreach (var assetName in assetNamesString)
        {
            var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(assetName);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAssets[assetName] = handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load asset: {assetName}");
            }
        }
    }

    public T GetAsset<T>(string assetName) where T : UnityEngine.Object
    {
        if (loadedAssets.TryGetValue(assetName, out var asset))
        {
            return asset as T;
        }
        else
        {
            Debug.LogError($"Asset not found: {assetName}");
            return null;
        }
    }
}