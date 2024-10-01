using System;
using System.Collections.Generic;
using UnityEngine;
using Type = Dialog.Type;

public class DialogManager : Singleton<DialogManager>
{
    private DialogTableUnit _tbUnit;
    private int stepIndex = 0;

    public delegate void DialogEnterEvent(bool isStart);
    public static event DialogEnterEvent OnDialogEnter;

    private Dictionary<string, int> _actors = new();
    private bool isPlaying = false;
    public bool IsPlaying { get { return isPlaying; } }

    public void EnterDialog(Type type)
    {
        var a = GameDataManager.Instance.Storages.UnlockDialog.IsUnlockDialogID(type);
        // 만약 처음 실행하는 대화라면 스토리지에 해금한다.
        if (!GameDataManager.Instance.Storages.UnlockDialog.IsUnlockDialogID(type))
        {
            GameDataManager.Instance.Storages.UnlockDialog.UnlockDialog(type);
        }
        isPlaying = true;
        _tbUnit = GameDataManager.Instance.Tables.Dialog.GetUnit(type);

        UIManager.Instance.OpenFadeOutIn(() =>
            {
                UIManager.Instance.CloseAll();
                UIManager.Instance.OpenDialog();
                OnDialogEnter?.Invoke(true);
                stepIndex = 0;
                ActionStep();
            },
            null);

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
            case DialogTableUnit.StepUnit.ActionType.Fade:
                ActionFade();
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
                spawnID = FieldObjectManager.Instance.SpawnSheep(1,_tbUnit.Steps[stepIndex].ActionPlace, StandardSheep.SheepState.Idle).InstanceID;
                break;
            case FieldObject.Type.Wool:
                spawnID = FieldObjectManager.Instance.SpawnWool(Vector2.zero).InstanceID;
                break;
            default:
                Debug.LogError("Wrong SpawnType");
                break;
        }
        if (_tbUnit.Steps[stepIndex].IsStop)
        {
            UIManager.Instance.SetActiveDialogNextBtn();
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
        characterObject.ShowSpeechBubble(_tbUnit.Steps[stepIndex].LocalText.GetLocalizedString(), _tbUnit.Steps[stepIndex].ActionTime, false, afterActionCallback);
    }
    private void ActionFade()
    {
        Action afterActionCallback = null;
        if (_tbUnit.Steps[stepIndex].IsStop)
        {
            afterActionCallback += UIManager.Instance.SetActiveDialogNextBtn;
        }
        UIManager.Instance.OpenFadeOutIn(null, afterActionCallback);
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
        isPlaying = false;
        _actors.Clear();

        UIManager.Instance.OpenFadeOutIn(() =>

            {
                UIManager.Instance.CloseDialog();
                OnDialogEnter?.Invoke(false);
            },
           null);


    }

}
