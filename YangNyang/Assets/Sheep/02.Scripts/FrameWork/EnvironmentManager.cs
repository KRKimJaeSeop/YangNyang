using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _wallCollider;

    [SerializeField]
    private SpriteRenderer _sky;
    [SerializeField]
    private SpriteRenderer _grass;
    [SerializeField]
    private SpriteRenderer _cloudShadow;

    public void Initialize()
    {
        _sky.sprite = 
            AddressableManager.Instance.GetAsset<Sprite>(AddressableManager.RemoteAssetCode.Sky);
        _grass.sprite =
            AddressableManager.Instance.GetAsset<Sprite>(AddressableManager.RemoteAssetCode.Grass);
        _cloudShadow.sprite =
            AddressableManager.Instance.GetAsset<Sprite>(AddressableManager.RemoteAssetCode.CloudShadow);
    }

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
