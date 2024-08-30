using UnityEditor;
using UnityEngine;

public class BuffSheepTableUnit : SheepTableUnit
{

#if UNITY_EDITOR

    static public BuffSheepTableUnit Create(int id, Sheep.Type type, string name)
    {
        BuffSheepTableUnit asset = ScriptableObject.CreateInstance<BuffSheepTableUnit>();

        AssetDatabase.CreateAsset(asset, $"{ASSET_PATH}{string.Format("{0:D3}", id)}_{name}.asset");
        asset.id = id;
        asset.Type = type;        
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

#endif
}
