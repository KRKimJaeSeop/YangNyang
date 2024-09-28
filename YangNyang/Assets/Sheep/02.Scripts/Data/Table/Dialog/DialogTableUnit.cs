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
            None = 0,
            Spawn = 1,
            Move = 2,
            Speech = 3,
            Fade = 4,
        }
        public ActionType UnitActionType;
        public string ActorNickName;
        public FieldObject.Type SpawnType;
        public Place.Type ActionPlace;
        public float ActionTime;
        public string SpeechText;
        public bool IsStop;

    }

    public const string ASSET_PATH = "Assets/Sheep/99.Data/Tables/Dialogs/";

    public Dialog.Type type;
    [SerializeField]
    private StepUnit[] _steps;
    public StepUnit[] Steps { get { return _steps; } }


    public bool IsValidDialog()
    {
        var isValid = false;
        foreach (var step in _steps)
        {
            // step중 isStop이 한개라도 있다면 true.
            if (step.IsStop)
            {
                isValid = true;
            }
            // Fade step이 isStop으로 설정되지 않았을 때 false
            if (step.UnitActionType == StepUnit.ActionType.Fade && !step.IsStop)
            {
                isValid = false;
                break;
            }
            if (step.UnitActionType ==StepUnit.ActionType.Spawn)
            {
                if (step.SpawnType == FieldObject.Type.UnWorkableSheep || step.SpawnType == FieldObject.Type.None)
                {
                    isValid = false;
                    break;
                }
            }
           

        }
        return isValid;
    }

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
