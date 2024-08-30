using System.Linq;
using UnityEditor;
using UnityEngine;

public class SheepSpawnRateTableUnit : BaseElementTable
{
    public const string ASSET_PATH = "Assets/Sheep/99.Data/Tables/SheepSpawnRates/";

    [Header("[SheepSpawnRateTableUnit]")]
    [Tooltip("필요 레벨")]
    public long requireLevel;
    [Tooltip("양 리스트")]
    public Sheep.Weight[] sheepList;
 

    public bool Contains(long level)
    {
        return (level >= requireLevel);
    }

#if UNITY_EDITOR
    static public SheepSpawnRateTableUnit Create(string name)
    {
        SheepSpawnRateTableUnit asset = ScriptableObject.CreateInstance<SheepSpawnRateTableUnit>();

        AssetDatabase.CreateAsset(asset, $"{ASSET_PATH}/{name}.asset");
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return asset;
    }


#endif
}
