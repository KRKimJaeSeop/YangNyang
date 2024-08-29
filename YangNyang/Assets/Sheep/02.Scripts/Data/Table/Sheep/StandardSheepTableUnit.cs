using UnityEditor;
using UnityEngine;

public class StandardSheepTableUnit : SheepTableUnit
{

#if UNITY_EDITOR

    static public StandardSheepTableUnit Create(int id, Sheep.Type type, string name)
    {
        StandardSheepTableUnit asset = ScriptableObject.CreateInstance<StandardSheepTableUnit>();

        AssetDatabase.CreateAsset(asset, $"{ASSET_PATH}{string.Format("{0:D3}", id)}_{name}.asset");
        asset.id = id;
        asset.type = type;
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }

#endif
}
