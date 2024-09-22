using TMPro;
using UnityEngine;
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
     
        //나중에 활성화
        // Addressables.LoadAssetAsync<Sprite>(textField1.text).Completed += OnSpriteLoaded;
        //AdMobAdapter.Instance.LoadRewardedAd();
        //Debug.Log(AdvertisingController.Instance.GetBannerHeightByPixel());

    }
    private void OnClick1()
    {
      
        UIManager.Instance.OpenResultPanel("테스트", $"과연");

    }
    private void OnClick2()
    {
        //eldObjectManager.Instance.StartSheepSpawn(true);
        //var texture = AddressableManager.Instance.GetAsset<Texture2D>(AddressableManager.RemoteAssetCode.OverlayBranch);
        //img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 5);
        AdvertisingController.Instance.ShowBanner();
        ////UIManager.Instance.OpenNotificationPanel(textField1.text);
    }
    private void OnClick3()
    {
        AdvertisingController.Instance.StopBanner();
        //FieldObjectManager.Instance.StopSheepSpawn();
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
        //GameManager.Instance.TestEnter();
        AdvertisingController.Instance.ShowInterstitial();

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
        //GameManager.Instance.TestExit();
        AdvertisingController.Instance.ShowRewardedAd(
            (a, b) =>
            {
                UIManager.Instance.OpenResultPanel("광고봄", $"광고 잘나오노 {a},{b}");
            });

    }

}
