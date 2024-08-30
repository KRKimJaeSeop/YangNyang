using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EventSheepTableUnit : SheepTableUnit
{

#if UNITY_EDITOR

    static public EventSheepTableUnit Create(int id, Sheep.Type type, string name)
    {
        EventSheepTableUnit asset = ScriptableObject.CreateInstance<EventSheepTableUnit>();

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
