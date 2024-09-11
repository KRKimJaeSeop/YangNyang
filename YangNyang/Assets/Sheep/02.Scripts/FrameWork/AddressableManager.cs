using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class AddressableManager : Singleton<AddressableManager>
{
    public enum RemoteAssetCode
    {
        None = 0,
        OverlayGrass = 1,
        OverlayBranch = 2,
        DocBar = 3,
        Sky = 4,
        CloudShadow = 5,
        Grass = 6,
        DropShadow = 7,
        Player = 8,
        BGM = 9,
    }

    private Dictionary<string, UnityEngine.Object> loadedAssets = new Dictionary<string, UnityEngine.Object>();



    public async Task LoadAllAssetsAsync()
    {
        foreach (RemoteAssetCode code in Enum.GetValues(typeof(RemoteAssetCode)))
        {
            if (code == RemoteAssetCode.None)
                continue;

            var handle = Addressables.LoadAssetAsync<UnityEngine.Object>($"{code}");
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAssets[$"{code}"] = handle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load asset: {$"{code}"}");
            }
        }
    }

    public T GetAsset<T>(RemoteAssetCode code) where T : UnityEngine.Object
    {
        if (loadedAssets.TryGetValue($"{code}", out var asset))
        {
            // Teture2D 에셋의 경우 스프라이트로 바로 적용이 안돼서 따로 변환해준다.
            if (typeof(T) == typeof(Sprite) && asset is Texture2D texture)
            {
                return CreateSprite(texture) as T;
            }
            return asset as T;
        }
        else
        {
            Debug.LogError($"Asset not found: {$"{code}"}");
            return null;
        }
    }
    private Sprite CreateSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }
}