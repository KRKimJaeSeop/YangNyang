using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _wallCollider;

    private void OnEnable()
    {
        DialogManager.OnDialogEnter += DialogManager_OnDialogEnter;
    }


    private void OnDisable()
    {
        DialogManager.OnDialogEnter -= DialogManager_OnDialogEnter;
    }

    private void DialogManager_OnDialogEnter(bool isStart)
    {
        _wallCollider.SetActive(!isStart);
    }


}
