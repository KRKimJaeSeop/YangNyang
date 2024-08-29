using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO; // for File.ReadAllText
#endif

public class CurrencyTableUnit : BaseElementTable
{
    public const string ASSET_PATH = "Assets/Sheep/99.Data/Tables/Currencies/";

    public Currency.Type  type;

#if UNITY_EDITOR

    static public CurrencyTableUnit Create(string name, int id, Currency.Type type)
    {
        CurrencyTableUnit asset = ScriptableObject.CreateInstance<CurrencyTableUnit>();

        AssetDatabase.CreateAsset(asset, $"{ASSET_PATH}{name}.asset");
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
