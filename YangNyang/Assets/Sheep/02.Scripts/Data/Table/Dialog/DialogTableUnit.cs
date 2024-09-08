using System;
using UnityEditor;
using UnityEngine;

public class DialogTableUnit : BaseElementTable
{
    [Serializable]
    public class StepUnit
    {
        public enum ActionType
        {
            Spawn,
            Move,
            Dialogue
        }
        public ActionType actionType;


    }

    public const string ASSET_PATH = "Assets/Sheep/99.Data/Tables/Dialogs/";

    public Dialog.Type type;
    [SerializeField]
    private StepUnit[] _steps;



#if UNITY_EDITOR

    static public DialogTableUnit Create(string name, int id, Dialog.Type type)
    {
        DialogTableUnit asset = ScriptableObject.CreateInstance<DialogTableUnit>();

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