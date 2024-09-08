using Dialog;
using System.Collections.Generic;
using UnityEngine;

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
        switch (_tbUnit.Steps[stepIndex].actionType)
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
                ExitDialog();
                break;
        }
        if (!_tbUnit.Steps[stepIndex].isStop)
        {
            stepIndex++;
            ActionStep();
        }
    }

    private void ActionSpawn()
    {
        int spawnID = 0;
        switch (_tbUnit.Steps[stepIndex].spawnType)
        {
            case FieldObject.Type.Player:
                spawnID = FieldObjectManager.Instance.SpawnPlayer(_tbUnit.Steps[stepIndex].ActionPlace).InstanceID;
                break;
            case FieldObject.Type.WorkableSheep:
                spawnID = FieldObjectManager.Instance.SpawnSheep(_tbUnit.Steps[stepIndex].ActionPlace).InstanceID;
                break;
            default:
                OnClickNext();
                break;
        }
        _actors.Add(_tbUnit.Steps[stepIndex].ActorName, spawnID);
        Debug.Log($"{stepIndex}번째 스폰했고");
    }
    private void ActionMove()
    {
        if (_actors.TryGetValue(_tbUnit.Steps[stepIndex].ActorName, out int actorID))
        {
            FieldObjectManager.Instance.GetFieldObject<CharacterObject>(actorID).
                MoveToPosition(_tbUnit.Steps[stepIndex].ActionPlace, 1);
        }
        else
        {
            Debug.LogError($"Don't Manage [{_tbUnit.Steps[stepIndex].ActorName}]");
            OnClickNext();
        }
        Debug.Log($"{stepIndex}번째 이동했고");
    }
    private void ActionSpeech()
    {
        if (_actors.TryGetValue(_tbUnit.Steps[stepIndex].ActorName, out int actorID))
        {
            FieldObjectManager.Instance.GetFieldObject<CharacterObject>(actorID).
                ShowSpeechBubble(_tbUnit.Steps[stepIndex].SpeechText, 2);
        }
        else
        {
            Debug.LogError($"Don't Manage [{_tbUnit.Steps[stepIndex].ActorName}]");
            OnClickNext();
        }
        Debug.Log($"{stepIndex}번째 얘기했고");
    }

    
    public void ExitDialog()
    {

        _actors.Clear();
        FieldObjectManager.Instance.DespawnAll();
        UIManager.Instance.CloseDialog();
        FieldObjectManager.Instance.SpawnPlayer();
        FieldObjectManager.Instance.StartSheepSpawn(false);
    }

}
