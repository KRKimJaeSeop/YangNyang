using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class DevUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField textField1;
    [SerializeField]
    private TMP_InputField textField2;
    [SerializeField]
    private Image img;

    [SerializeField]
    private Button btn;

    [SerializeField]
    private Button btn1;
    [SerializeField]
    private Button btn2;
    [SerializeField]
    private Button btn3;
    [SerializeField]
    private Button btn4;
    [SerializeField]
    private Button btn5;


    private void Awake()
    {
        btn.onClick.AddListener(OnClickTestBtn);
        btn1.onClick.AddListener(OnClick1);
        btn2.onClick.AddListener(OnClick2);
        btn3.onClick.AddListener(OnClick3);
        btn4.onClick.AddListener(OnClick4);
        btn5.onClick.AddListener(OnClick5);

    }

    private void OnClickTestBtn()
    {
        //var value1= float.Parse(textField1.text);
        //var value2= float.Parse(textField2.text);
        //FieldObjectManager.Instance.SheepSpawnBuff(value1, value2);
        //Addressables.InstantiateAsync(textField1.text,new Vector2(-8,0), Quaternion.identity);
        //GameDataManager.Instance.Storages.User.IncreaseLevel(int.Parse(textField1.text));
        //GameManager.Instance.GameClear();
        //FieldObjectManager.Instance.DespawnByInstanceID(int.Parse(textField1.text));
        //Debug.LogWarning("=========");
        //foreach (var item in FieldObjectManager.Instance._managedObjects.ToList())
        //{
        //    Debug.Log(item.Key);
        //}
        //Debug.Break();
        //AudioManager.Instance.Initialize(
        //   GameDataManager.Instance.Storages.Preference.GetVolume(AudioManager.MixerGroup.BGM),
        //   GameDataManager.Instance.Storages.Preference.GetVolume(AudioManager.MixerGroup.SFXMaster));
        //AudioManager.Instance.MusicBox.PlayBGM();

        //나중에 활성화
        Addressables.LoadAssetAsync<Sprite>(textField1.text).Completed += OnSpriteLoaded;
    }
    private void OnSpriteLoaded(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Sprite loaded successfully");
            img.sprite = handle.Result;
            Instantiate(handle.Result);
        }
        else
        {
            Debug.LogError("Failed to load sprite: " + handle.OperationException);
        }
    }
    private void OnClick1()
    {
        //UIManager.Instance.OpenConfirmPanel(textField1.text, textField2.text);
        //FieldObjectManager.Instance.StartSheepSpawn(false);
        AudioManager.Instance.MusicBox.PlaySFX(MusicBox.SfxType.DefaultClick);
    }
    private void OnClick2()
    {
        //eldObjectManager.Instance.StartSheepSpawn(true);
        var texture = AddressableManager.Instance.GetAsset<Texture2D>(AddressableManager.RemoteAssetCode.OverlayBranch);
        img.sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.one*5);
        //UIManager.Instance.OpenNotificationPanel(textField1.text);
    }
    private void OnClick3()
    {
        FieldObjectManager.Instance.StopSheepSpawn();
        //UIManager.Instance.OpenResultPanel(textField1.text, textField2.text);
    }
    private void OnClick4()
    {
        //if (textField1.text == string.Empty)
        //{
        //    UIManager.Instance.OpenLoading();
        //}
        //else
        //{
        //    UIManager.Instance.CloseLoading();

        //}
        GameManager.Instance.TestEnter();
    }
    private void OnClick5()
    {
        //if (textField1.text == string.Empty)
        //{
        //    UIManager.Instance.OpenWaiting();
        //}
        //else
        //{
        //    UIManager.Instance.CloseWaiting();

        //}
        GameManager.Instance.TestExit();

    }

}
