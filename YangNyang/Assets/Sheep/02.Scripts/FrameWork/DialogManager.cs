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

    // �ϳ��� ������ �����Ѵ�. isStop�� false��� ����Ѵ�.
    private void ActionStep()
    {
        // �ε����� �ʰ��ߴٸ� ���̾�α׸� �����Ѵ�.
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
        Debug.Log($"{stepIndex}��° �����߰�");
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
        Debug.Log($"{stepIndex}��° �̵��߰�");
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
        Debug.Log($"{stepIndex}��° ����߰�");
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
