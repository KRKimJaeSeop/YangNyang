using Dialog;
using System;
using System.Collections.Generic;
using UnityEngine;
using Type = Dialog.Type;

public class DialogManager : Singleton<DialogManager>
{
    private DialogTableUnit _tbUnit;
    private int stepIndex = 0;

    private Dictionary<string, int> _actors = new();


    public void EnterDialog(Type type)
    {
        FieldObjectManager.Instance.DespawnAll();
        FieldObjectManager.Instance.StopSheepSpawn();
        UIManager.Instance.OpenDialog();
        stepIndex = 0;
        _tbUnit = GameDataManager.Instance.Tables.Dialog.GetUnit(type);
        ActionStep();
    }

    public void OnClickNext()
    {
        stepIndex++;
        ActionStep();
    }
    public void OnClickSkip()
    {
        ExitDialog();
    }

    // 하나의 스텝을 실행한다. isStop이 false라면 재귀한다.
    private void ActionStep()
    {
        // 인덱스가 초과했다면 다이얼로그를 종료한다.
        if (_tbUnit.Steps.Length - 1 < stepIndex)
        {
            ExitDialog();
            return;
        }
        switch (_tbUnit.Steps[stepIndex].UnitActionType)
        {
            case DialogTableUnit.StepUnit.ActionType.Spawn:
                ActionSpawn();
                break;
            case DialogTableUnit.StepUnit.ActionType.Move:
                ActionMove();
                break;
            case DialogTableUnit.StepUnit.ActionType.Speech:
                ActionSpeech();
                break;
            default:
                stepIndex++;
                ActionStep();
                break;
        }
        if (!_tbUnit.Steps[stepIndex].IsStop)
        {
            stepIndex++;
            ActionStep();
        }
    }

    private void ActionSpawn()
    {
        int spawnID = 0;
        switch (_tbUnit.Steps[stepIndex].SpawnType)
        {
            case FieldObject.Type.Player:
                spawnID = FieldObjectManager.Instance.SpawnPlayer(_tbUnit.Steps[stepIndex].ActionPlace).InstanceID;
                break;
            case FieldObject.Type.WorkableSheep:
                spawnID = FieldObjectManager.Instance.SpawnSheep(_tbUnit.Steps[stepIndex].ActionPlace, StandardSheep.SheepState.Idle).InstanceID;
                break;
            default:
                Debug.LogError("Wrong SpawnType");
                break;
        }
        _actors.Add(_tbUnit.Steps[stepIndex].ActorNickName, spawnID);
    }

    private void ActionMove()
    {
        var characterObject = GetCharacterObject(_tbUnit.Steps[stepIndex].ActorNickName);
        Action onActionEnd = null;
        if (_tbUnit.Steps[stepIndex].IsStop)
        {
            onActionEnd += UIManager.Instance.SetActiveDialogNextBtn;
        }
        characterObject.MoveToPosition(_tbUnit.Steps[stepIndex].ActionPlace, _tbUnit.Steps[stepIndex].ActionTime, onActionEnd);
    }

    private void ActionSpeech()
    {
        var characterObject = GetCharacterObject(_tbUnit.Steps[stepIndex].ActorNickName);
        Action afterActionCallback = null;
        if (_tbUnit.Steps[stepIndex].IsStop)
        {
            afterActionCallback += UIManager.Instance.SetActiveDialogNextBtn;
        }
        characterObject.ShowSpeechBubble(_tbUnit.Steps[stepIndex].SpeechText, _tbUnit.Steps[stepIndex].ActionTime, false, afterActionCallback);
    }


    private CharacterObject GetCharacterObject(string actorNickname)
    {
        if (_actors.TryGetValue(_tbUnit.Steps[stepIndex].ActorNickName, out int actorID))
        {
            return (FieldObjectManager.Instance.GetFieldObject<CharacterObject>(actorID));
        }
        else
        {
            Debug.LogError($"Don't Manage [{actorNickname}]");
            return null;
        }

    }

    public void ExitDialog()
    {
        _actors.Clear();
        FieldObjectManager.Instance.DespawnAll();
        UIManager.Instance.CloseDialog();
        FieldObjectManager.Instance.SpawnPlayer();
        FieldObjectManager.Instance.StartSheepSpawn(false);
    }

    //private void ActionMove()
    //{
    //    if (_actors.TryGetValue(_tbUnit.Steps[stepIndex].ActorNickName, out int actorID))
    //    {
    //        if (!_tbUnit.Steps[stepIndex].isStop)
    //        {
    //            FieldObjectManager.Instance.GetFieldObject<CharacterObject>(actorID).
    //          MoveToPosition(_tbUnit.Steps[stepIndex].ActionPlace, 1,
    //          () =>
    //          {
    //              Debug.Log("버튼 활성화 하기");
    //          });
    //        }
    //        else
    //        {
    //            FieldObjectManager.Instance.GetFieldObject<CharacterObject>(actorID).
    //          MoveToPosition(_tbUnit.Steps[stepIndex].ActionPlace, 1);
    //        }

    //    }
    //    else
    //    {
    //        Debug.LogError($"Don't Manage [{_tbUnit.Steps[stepIndex].ActorNickName}]");
    //    }
    //    Debug.Log($"{stepIndex}번째 이동했고");
    //}
    //private void ActionSpeech()
    //{

    //    if (_actors.TryGetValue(_tbUnit.Steps[stepIndex].ActorNickName, out int actorID))
    //    {
    //        if (!_tbUnit.Steps[stepIndex].isStop)
    //        {
    //            FieldObjectManager.Instance.GetFieldObject<CharacterObject>(actorID).
    //                          ShowSpeechBubble(_tbUnit.Steps[stepIndex].SpeechText, 2, false,
    //                          () =>
    //                          {
    //                              Debug.Log("버튼 활성화 하기");
    //                          });
    //        }
    //        else
    //        {
    //            FieldObjectManager.Instance.GetFieldObject<CharacterObject>(actorID).
    //                        ShowSpeechBubble(_tbUnit.Steps[stepIndex].SpeechText, 2, false);
    //        }


    //    }
    //    else
    //    {
    //        Debug.LogError($"Don't Manage [{_tbUnit.Steps[stepIndex].ActorNickName}]");
    //    }
    //    Debug.Log($"{stepIndex}번째 얘기했고");
    //}

}
